using DRLMobile.Core.Models.DataModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DRLMobile.Uwp.CustomControls
{
    public sealed partial class PreviewDocumentControl : UserControl
    {
        public bool IsPrintEmailIconVisible
        {
            get { return (bool)GetValue(IsPrintEmailIconVisibleProperty); }
            set { SetValue(IsPrintEmailIconVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPrintEmailIconVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPrintEmailIconVisibleProperty = DependencyProperty.Register(nameof(IsPrintEmailIconVisible), typeof(bool),
            typeof(PreviewDocumentControl), new PropertyMetadata(defaultValue: false, propertyChangedCallback: OnEmailPrintIconVisibilityChanged));

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register(nameof(FilePath), typeof(string),
            typeof(PreviewDocumentControl), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnFilePathChanged));

        public ICommand CloseCommad
        {
            get { return (ICommand)GetValue(CloseCommadProperty); }
            set { SetValue(CloseCommadProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseCommad.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseCommadProperty = DependencyProperty.Register(nameof(CloseCommad), typeof(ICommand),
            typeof(PreviewDocumentControl), new PropertyMetadata(null));

        public ICommand PrintPictureCommand
        {
            get { return (ICommand)GetValue(PrintPictureCommandProperty); }
            set { SetValue(PrintPictureCommandProperty, value); }
        }

        public static readonly DependencyProperty PrintPictureCommandProperty = DependencyProperty.Register(nameof(PrintPictureCommand), typeof(ICommand),
             typeof(PreviewDocumentControl), new PropertyMetadata(null));

        public ObservableCollection<CustomImage> myCustomImage;

        public PreviewDocumentControl()
        {
            InitializeComponent();

            DataContext = this;

            myCustomImage = new ObservableCollection<CustomImage>();

            myCustomImage.Add(new CustomImage(FilePath));

            imageFlipView.ItemsSource = myCustomImage;
        }

        private static void OnEmailPrintIconVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PreviewDocumentControl;
            control.HandleIconVisibility();
        }

        private static void OnFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PreviewDocumentControl;
            control.ShowAppropriateUserInterface();
        }

        private async void ShowAppropriateUserInterface()
        {
            try
            {
                HideAllControls();
                var fileName = Core.Helpers.HelperMethods.GetNameFromURL(FilePath);
                TitleTextBlock.Text = fileName;
                var extension = Path.GetExtension(fileName)?.ToLower();
                if (!string.IsNullOrWhiteSpace(extension))
                {
                    switch (extension)
                    {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".bmp":
                            {
                                myCustomImage.Clear();

                                myCustomImage.Add(new CustomImage(FilePath));

                                imageFlipView.ItemsSource = myCustomImage;

                                imageFlipView.Visibility = Visibility.Visible;

                                break;
                            }
                        default:
                            var file = await StorageFile.GetFileFromPathAsync(FilePath);
                            await Windows.System.Launcher.LaunchFileAsync(file);
                            CloseCommad?.Execute(null);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(PreviewDocumentControl), nameof(ShowAppropriateUserInterface), ex);
            }
        }

        private void HideAllControls()
        {
            imageFlipView.Visibility = Visibility.Collapsed;
        }

        private void ClosePreviewer(object sender, TappedRoutedEventArgs e)
        {
            CloseCommad?.Execute(null);
        }

        private void HandleIconVisibility()
        {
            PrintIcon.Visibility = IsPrintEmailIconVisible ? Visibility.Visible : Visibility.Collapsed;
            EmailIcon.Visibility = IsPrintEmailIconVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PrintIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                var path = FilePath;
                if (!string.IsNullOrEmpty(FilePath) && PrintPictureCommand != null)
                {
                    PrintPictureCommand.Execute(FilePath);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(PreviewDocumentControl), "PrintIcon_Tapped", ex.StackTrace);
            }
        }

        private async void EmailIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if(!File.Exists(FilePath))
                {
                    await AlertHelper.Instance.ShowConfirmationAlert(ResourceExtensions.GetLocalized("ALERT"), ResourceExtensions.GetLocalized("CustomerDocumentFileEmailFailed"), ResourceExtensions.GetLocalized("OK"), string.Empty);
                    return;
                }
                EmailModel model = new EmailModel() { AttachmentListByPath = new List<string>() { FilePath } };
                await Services.EmailService.Instance.SendMailFromOutlook(model);
            }
            catch (Exception ex)
            {
                var errDescription = ex.Message + " " + ex.StackTrace + " " + ex.InnerException?.Message + " " + ex.InnerException;
                ErrorLogger.WriteToErrorLog(GetType().Name, "EmailIcon_Tapped", errDescription);
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "Something went wrong. Could not send email. Please try again later", "OK");

            }
        }

        private async void ScrollViewer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;

            var doubleTapPoint = e.GetPosition(scrollViewer);

            if (scrollViewer.ZoomFactor != 1)
            {
                scrollViewer.ChangeView(1, 1, 1);
            }
            else if (scrollViewer.ZoomFactor == 1)
            {
                scrollViewer.ChangeView(2, 2, 2);

                var dispatcher = Window.Current.CoreWindow.Dispatcher;

                await dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    scrollViewer.ChangeView(doubleTapPoint.X, doubleTapPoint.Y, 2);
                });
            }
        }
    }
}
