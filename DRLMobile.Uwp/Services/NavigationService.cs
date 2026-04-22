using System;
using System.Linq;

using DRLMobile.Uwp.View;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace DRLMobile.Uwp.Services
{
    public static class NavigationService
    {
        public static event NavigatedEventHandler Navigated;

        public static event NavigationFailedEventHandler NavigationFailed;

        private static Frame _frame;
        private static object _lastParamUsed;

        public static Frame Frame
        {
            get
            {
                if (_frame == null)
                {
                    _frame = Window.Current.Content as Frame;
                    RegisterFrameEvents();
                }

                return _frame;
            }

            set
            {
                UnregisterFrameEvents();
                _frame = value;
                RegisterFrameEvents();
            }
        }

        public static bool CanGoBack => Frame.CanGoBack;

        public static bool CanGoForward => Frame.CanGoForward;

        public static bool GoBack()
        {
            if (CanGoBack)
            {
                Frame.GoBack();
                return true;
            }

            return false;
        }

        public static void GoForward() => Frame.GoForward();

        public static bool NavigateMainFrame(Type pageType, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            if (pageType == null || !pageType.IsSubclassOf(typeof(Page)))
            {
                throw new ArgumentException($"Invalid pageType '{pageType}', please provide a valid pageType.", nameof(pageType));
            }

            // Don't open the same page multiple times
            if (Frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(_lastParamUsed)))
            {
                var navigationResult = Frame.Navigate(pageType, parameter, infoOverride);
                if (navigationResult)
                {
                    _lastParamUsed = parameter;
                }

                return navigationResult;
            }
            else
            {
                return false;
            }
        }

        public static bool NavigateShellFrame(Type pageType, object parameter = null)
        {
            if ((Window.Current.Content as Frame).Content is ShellPage)
            {
                var shell = (Window.Current.Content as Frame).Content as ShellPage;
                var frame = shell.FindName("MainFrame");
                if (frame is Frame)
                {
                    if (typeof(DashboardPage) == pageType && !(frame as Frame).BackStack.Any() && !shell.ViewModel.IsSyncFromSidePane)
                    {
                        return false;
                    }
                    else if ((frame as Frame).BackStack.Count() > 1 && (frame as Frame).Content.GetType().Name == pageType.Name)
                    {
                        if (typeof(ActivitiesPage) == pageType)
                        {
                            if ((bool)((App)Application.Current).IsCustomerActivity)
                            {
                                ((App)Application.Current).IsCustomerActivity = false;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        (frame as Frame).Navigate(pageType, parameter);
                    }
                    else
                    {
                        (frame as Frame).Navigate(pageType, parameter);
                    }
                }
                CheckBackButtonVisibility();
                return true;
            }
            else
                return false;
        }

        public static void LoadingOnShellPage(bool isLoading)
        {
            if ((Window.Current.Content as Frame).Content is ShellPage)
            {
                var shell = (Window.Current.Content as Frame).Content as ShellPage;
                var frame = shell.FindName("MainFrame");
                if (frame is Frame)
                {
                    shell.ViewModel.IsLoading = isLoading;
                }
            }
        }

        private static void CheckBackButtonVisibility()
        {
            if ((Window.Current.Content as Frame).Content is ShellPage)
            {
                var shell = (Window.Current.Content as Frame).Content as ShellPage;
                var frame = shell.FindName("MainFrame");
                if (frame is Frame)
                {
                    shell.ViewModel.IsBackEnabled = (frame as Frame).BackStack.Any();
                }
            }
        }


        public static void GoBackInShell()
        {
            if ((Window.Current.Content as Frame).Content is ShellPage)
            {
                var shell = (Window.Current.Content as Frame).Content as ShellPage;
                var frame = shell.FindName("MainFrame");
                if (frame is Frame)
                {
                    if ((frame as Frame).BackStack.Any())
                        (frame as Frame).GoBack();
                    CheckBackButtonVisibility();
                }
            }
        }

        public static void InitialShellNavigation()
        {
            if ((Window.Current.Content as Frame).Content is ShellPage)
            {
                var shell = (Window.Current.Content as Frame).Content as ShellPage;
                var frame = shell.FindName("MainFrame");
                if (frame is Frame && !(frame as Frame).BackStack.Any())
                {
                    (frame as Frame).Navigate(typeof(DashboardPage));
                    shell.ViewModel.IsBackEnabled = false;
                }
            }
        }

        private static void RegisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated += Frame_Navigated;
                _frame.NavigationFailed += Frame_NavigationFailed;
            }
        }

        private static void UnregisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated -= Frame_Navigated;
                _frame.NavigationFailed -= Frame_NavigationFailed;
            }
        }

        private static void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e) => NavigationFailed?.Invoke(sender, e);

        private static void Frame_Navigated(object sender, NavigationEventArgs e) => Navigated?.Invoke(sender, e);
    }
}
