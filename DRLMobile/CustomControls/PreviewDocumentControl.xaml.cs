using DRLMobile.ExceptionHandler;
using System;
using System.IO;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DRLMobile.CustomControls
{
    public sealed partial class PreviewDocumentControl : UserControl
    {
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register(nameof(FilePath), typeof(string), typeof(PreviewDocumentControl), new PropertyMetadata(defaultValue: string.Empty, propertyChangedCallback: OnFilePathChanged));

        public ICommand CloseCommad
        {
            get { return (ICommand)GetValue(CloseCommadProperty); }
            set { SetValue(CloseCommadProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseCommad.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseCommadProperty =
            DependencyProperty.Register(nameof(CloseCommad), typeof(ICommand), typeof(PreviewDocumentControl), new PropertyMetadata(null));

        public PreviewDocumentControl()
        {
            this.InitializeComponent();
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
                        //case ".pdf":
                        //    PdfViewer.DocumentSource = FilePath;
                        //    PdfViewer.Visibility = Visibility.Visible;
                        //    break;
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".bmp":
                            ImageViewer.Source = new BitmapImage(new Uri(FilePath));
                            ImageViewer.Visibility = Visibility.Visible;
                            break;
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
                ErrorLogger.WriteToErrorLog(nameof(PreviewDocumentControl), nameof(ShowAppropriateUserInterface), ex.StackTrace);
            }

        }

        private void HideAllControls()
        {
            ImageViewer.Visibility = Visibility.Collapsed;
            //PdfViewer.Visibility = Visibility.Collapsed;
        }

        private void ClosePreviewer(object sender, TappedRoutedEventArgs e)
        {
            CloseCommad?.Execute(null);
        }

        
    }




}
