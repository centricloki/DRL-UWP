using System;
using System.Collections.Generic;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DRLMobile.Uwp.Helpers
{
    /// <summary>
    /// Attached behavior that dynamically adjusts the width of an element based on the width of
    /// the Grid column it is assigned to (via Grid.Column), minus an optional pixel offset.
    /// Automatically handles star (*) and fixed-width columns, respects margins, and includes
    /// debouncing for optimal performance during layout or window resize events.
    /// </summary>
    public static class DynamicWidthBehavior
    {
        #region Constants and Storage

        /// <summary>
        /// Dictionary mapping parent FrameworkElements to their associated debounce timers.
        /// Prevents excessive width recalculations during rapid size changes.
        /// </summary>
        private static readonly Dictionary<FrameworkElement, DispatcherTimer> _debounceTimers =
            new Dictionary<FrameworkElement, DispatcherTimer>();

        /// <summary>
        /// List of all elements currently being tracked by this behavior.
        /// Used for global resize handling and cleanup.
        /// </summary>
        private static readonly List<FrameworkElement> _trackedElements = new List<FrameworkElement>();

        /// <summary>
        /// Flag indicating whether the global window resize handler has been attached.
        /// Ensures handler is only attached once.
        /// </summary>
        private static bool _windowHandlerAttached = false;

        /// <summary>
        /// Global debounce timer used during window resize events.
        /// </summary>
        private static DispatcherTimer _globalDebounceTimer;

        /// <summary>
        /// Delay in milliseconds before processing a resize event.
        /// Balances responsiveness and performance (150ms is a good default).
        /// </summary>
        private static readonly int DebounceDelay = 150;

        #endregion

        #region Attached Dependency Properties

        /// <summary>
        /// Attached property to specify a pixel offset to subtract from (or add to) the computed column width.
        /// A negative value (e.g., -10) reduces the element's width by 10px.
        /// Usage: helper:DynamicWidthBehavior.WidthOffset="-10"
        /// </summary>
        public static readonly DependencyProperty WidthOffsetProperty =
            DependencyProperty.RegisterAttached(
                "WidthOffset",
                typeof(double),
                typeof(DynamicWidthBehavior),
                new PropertyMetadata(0.0, OnWidthOffsetChanged));

        /// <summary>
        /// Internal attached property to store a reference to the parent element.
        /// Used during cleanup to unsubscribe from events and prevent memory leaks.
        /// </summary>
        private static readonly DependencyProperty ParentReferenceProperty =
            DependencyProperty.RegisterAttached(
                "ParentReference",
                typeof(FrameworkElement),
                typeof(DynamicWidthBehavior),
                new PropertyMetadata(null));

        #endregion

        #region Public Property Accessors

        /// <summary>
        /// Sets the WidthOffset attached property on the specified element.
        /// </summary>
        /// <param name="element">The target UI element.</param>
        /// <param name="value">The offset in pixels (e.g., -10 to reduce width by 10px).</param>
        public static void SetWidthOffset(UIElement element, double value)
        {
            element.SetValue(WidthOffsetProperty, value);
        }

        /// <summary>
        /// Gets the WidthOffset value from the specified element.
        /// </summary>
        /// <param name="element">The target UI element.</param>
        /// <returns>The current offset value in pixels.</returns>
        public static double GetWidthOffset(UIElement element)
        {
            return (double)element.GetValue(WidthOffsetProperty);
        }

        #endregion

        #region Internal Property Accessors

        /// <summary>
        /// Stores a reference to the parent FrameworkElement for cleanup purposes.
        /// </summary>
        /// <param name="element">The child element.</param>
        /// <param name="value">The parent element to reference.</param>
        private static void SetParentReference(UIElement element, FrameworkElement value)
        {
            element.SetValue(ParentReferenceProperty, value);
        }

        /// <summary>
        /// Retrieves the stored parent FrameworkElement reference.
        /// </summary>
        /// <param name="element">The child element.</param>
        /// <returns>The referenced parent element, or null if not set.</returns>
        private static FrameworkElement GetParentReference(UIElement element)
        {
            return (FrameworkElement)element.GetValue(ParentReferenceProperty);
        }

        #endregion

        #region Property Changed Handler

        /// <summary>
        /// Called when the WidthOffset property changes on a UIElement.
        /// Sets up or tears down event subscriptions based on whether the offset is non-zero.
        /// </summary>
        /// <param name="d">The dependency object (expected to be a FrameworkElement).</param>
        /// <param name="e">Event arguments containing old and new values.</param>
        private static void OnWidthOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                // Prevent duplicate event subscriptions
                element.Loaded -= OnElementLoaded;
                element.Unloaded -= OnElementUnloaded;

                if ((double)e.NewValue != 0)
                {
                    // Subscribe to lifecycle events
                    element.Loaded += OnElementLoaded;
                    element.Unloaded += OnElementUnloaded;

                    // If already in visual tree, set up tracking immediately
                    if (element.Parent != null)
                    {
                        SetupParentTracking(element);
                    }
                }
                else
                {
                    // Clean up if offset is set to zero
                    CleanupParentTracking(element);
                }
            }
        }

        #endregion

        #region Element Lifecycle Event Handlers

        /// <summary>
        /// Handles the Loaded event of the tracked element.
        /// Ensures parent tracking is initialized once the element is in the visual tree.
        /// </summary>
        /// <param name="sender">The loaded FrameworkElement.</param>
        /// <param name="e">Event data.</param>
        private static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                SetupParentTracking(element);
            }
        }

        /// <summary>
        /// Handles the Unloaded event of the tracked element.
        /// Critical for unsubscribing events and preventing memory leaks.
        /// </summary>
        /// <param name="sender">The unloaded FrameworkElement.</param>
        /// <param name="e">Event data.</param>
        private static void OnElementUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                CleanupParentTracking(element);
            }
        }

        #endregion

        #region Setup and Cleanup Logic

        /// <summary>
        /// Sets up tracking for the parent of the given element.
        /// Subscribes to parent SizeChanged and ensures global window resize handler is active.
        /// </summary>
        /// <param name="element">The child element to track.</param>
        private static void SetupParentTracking(FrameworkElement element)
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            if (parent != null)
            {
                SetParentReference(element, parent);

                if (!_trackedElements.Contains(element))
                {
                    _trackedElements.Add(element);
                }

                parent.SizeChanged -= OnParentSizeChanged;
                parent.SizeChanged += OnParentSizeChanged;

                EnsureWindowHandlerAttached();

                // Apply initial width immediately
                UpdateWidth(element, parent);
            }
        }

        /// <summary>
        /// Ensures the global window resize event handler is attached exactly once.
        /// Initializes the global debounce timer if needed.
        /// </summary>
        private static void EnsureWindowHandlerAttached()
        {
            if (!_windowHandlerAttached && Window.Current != null)
            {
                Window.Current.SizeChanged -= OnWindowSizeChanged;
                Window.Current.SizeChanged += OnWindowSizeChanged;
                _windowHandlerAttached = true;

                if (_globalDebounceTimer == null)
                {
                    _globalDebounceTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(DebounceDelay)
                    };
                    _globalDebounceTimer.Tick += OnGlobalDebounceTimerTick;
                }
            }
        }

        /// <summary>
        /// Cleans up all tracking and event subscriptions for a given element.
        /// Called when the element is unloaded or WidthOffset is set to zero.
        /// </summary>
        /// <param name="element">The element to clean up.</param>
        private static void CleanupParentTracking(FrameworkElement element)
        {
            _trackedElements.Remove(element);

            var parent = GetParentReference(element);
            if (parent != null)
            {
                parent.SizeChanged -= OnParentSizeChanged;
                SetParentReference(element, null);

                if (_debounceTimers.TryGetValue(parent, out var timer))
                {
                    timer.Stop();
                    _debounceTimers.Remove(parent);
                }
            }

            // Detach global handler if no elements remain
            if (_trackedElements.Count == 0 && _windowHandlerAttached)
            {
                if (Window.Current != null)
                {
                    Window.Current.SizeChanged -= OnWindowSizeChanged;
                }
                _windowHandlerAttached = false;
                _globalDebounceTimer?.Stop();
            }
        }

        #endregion

        #region Size Change Event Handlers

        /// <summary>
        /// Handles SizeChanged events on parent elements.
        /// Debounces updates to avoid performance issues during layout animations or rapid changes.
        /// </summary>
        /// <param name="sender">The parent FrameworkElement.</param>
        /// <param name="e">Size change event data.</param>
        private static void OnParentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is FrameworkElement parent && e.PreviousSize.Width != e.NewSize.Width)
            {
                DebounceParentUpdate(parent);
            }
        }

        /// <summary>
        /// Handles window resize events.
        /// Uses a global debounce timer to batch updates across all tracked elements.
        /// </summary>
        /// <param name="sender">The window object.</param>
        /// <param name="e">Window resize event data.</param>
        private static void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            _globalDebounceTimer?.Stop();
            _globalDebounceTimer?.Start();
        }

        /// <summary>
        /// Called when the global debounce timer fires after window resizing stops.
        /// Updates the width of all tracked elements.
        /// </summary>
        /// <param name="sender">The timer instance.</param>
        /// <param name="e">Event data.</param>
        private static void OnGlobalDebounceTimerTick(object sender, object e)
        {
            _globalDebounceTimer?.Stop();

            foreach (var element in _trackedElements.ToArray())
            {
                var parent = GetParentReference(element);
                if (parent != null)
                {
                    UpdateWidth(element, parent);
                }
            }
        }

        /// <summary>
        /// Sets up or restarts a debounce timer for a specific parent element.
        /// Prevents too-frequent updates when a single parent resizes.
        /// </summary>
        /// <param name="parent">The parent FrameworkElement triggering the update.</param>
        private static void DebounceParentUpdate(FrameworkElement parent)
        {
            if (!_debounceTimers.TryGetValue(parent, out var timer))
            {
                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(DebounceDelay)
                };
                timer.Tick += (s, _) =>
                {
                    timer.Stop();
                    UpdateChildrenWidths(parent);
                };
                _debounceTimers[parent] = timer;
            }

            timer.Stop();
            timer.Start();
        }

        /// <summary>
        /// Updates the width of all immediate children of a parent that have WidthOffset set.
        /// </summary>
        /// <param name="parent">The parent Grid or container.</param>
        private static void UpdateChildrenWidths(FrameworkElement parent)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                if (VisualTreeHelper.GetChild(parent, i) is FrameworkElement child)
                {
                    if (GetWidthOffset(child) != 0)
                    {
                        UpdateWidth(child, parent);
                    }
                }
            }
        }

        #endregion

        #region Width Calculation Logic

        /// <summary>
        /// Calculates and applies the appropriate width to the element based on its assigned Grid column.
        /// Respects star (*) sizing, fixed widths, and element margins.
        /// </summary>
        /// <param name="element">The element to resize.</param>
        /// <param name="parent">The parent element (expected to be a Grid).</param>
        private static void UpdateWidth(FrameworkElement element, FrameworkElement parent)
        {
            // Ensure parent is a Grid with column definitions
            if (!(parent is Grid grid) || grid.ColumnDefinitions.Count == 0)
                return;

            // Get the column index assigned to the element via Grid.Column
            int colIndex = Grid.GetColumn(element);

            // Validate column index
            if (colIndex < 0 || colIndex >= grid.ColumnDefinitions.Count)
                return;

            var column = grid.ColumnDefinitions[colIndex];
            double computedColumnWidth;

            if (column.Width.IsStar)
            {
                // Handle star (*) sizing: calculate proportional width
                double totalStar = 0;
                foreach (var col in grid.ColumnDefinitions)
                {
                    if (col.Width.IsStar)
                        totalStar += col.Width.Value;
                }

                if (totalStar > 0)
                {
                    double ratio = column.Width.Value / totalStar;
                    computedColumnWidth = grid.ActualWidth * ratio;
                }
                else
                {
                    // Fallback if all star values are zero
                    computedColumnWidth = column.ActualWidth;
                }
            }
            else
            {
                // Fixed, Auto, or pixel-based width
                computedColumnWidth = column.ActualWidth;
            }


            // Apply user-defined offset (e.g., -10 to reduce width by 10px)
            double offset = GetWidthOffset(element);
            double finalWidth = computedColumnWidth + offset;
            if (finalWidth > 100)
            {
                // Ensure width is positive and reasonable
                element.Width = Math.Max(100, finalWidth);

                // 🔍 DEBUG: Also log grid size
                System.Diagnostics.Debug.WriteLine(
                    $@"[Debug] Grid.Column[{colIndex}].ActualWidth={computedColumnWidth}, 
                GetOffset={offset}                
                Setting Width={finalWidth}                
                ");
            }
        }

        #endregion
    }
}