using DRLMobile.Uwp.Helpers;

using System;
using System.Windows.Input;

using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DRLMobile.Uwp.CustomControls
{
    public sealed partial class NameAndSignaturePanelControl : UserControl
    {
        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(name: nameof(CancelCommand), propertyType: typeof(ICommand),
            ownerType: typeof(NameAndSignaturePanelControl), typeMetadata: new PropertyMetadata(defaultValue: null));

        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        public static readonly DependencyProperty SaveSignatureCommandProperty = DependencyProperty.Register(name: nameof(SaveSignatureCommand), propertyType: typeof(ICommand),
            ownerType: typeof(NameAndSignaturePanelControl), typeMetadata: new PropertyMetadata(defaultValue: null));

        public ICommand SaveSignatureCommand
        {
            get { return (ICommand)GetValue(SaveSignatureCommandProperty); }
            set { SetValue(SaveSignatureCommandProperty, value); }
        }

        public static readonly DependencyProperty SignatureFileNameProperty = DependencyProperty.Register(name: nameof(SignatureFileName),
            propertyType: typeof(string), ownerType: typeof(NameAndSignaturePanelControl),
            typeMetadata: new PropertyMetadata(defaultValue: "OrderSignature.jpg"));

        public string SignatureFileName
        {
            get { return (string)GetValue(SignatureFileNameProperty); }
            set { SetValue(SignatureFileNameProperty, value); }
        }


        public static readonly DependencyProperty PrintNameProperty = DependencyProperty.Register(name: nameof(PrintName),
            propertyType: typeof(string), ownerType: typeof(NameAndSignaturePanelControl),
            typeMetadata: new PropertyMetadata(defaultValue: string.Empty));

        public string PrintName
        {
            get { return (string)GetValue(PrintNameProperty); }
            set { SetValue(PrintNameProperty, value); }
        }

        public static readonly DependencyProperty IsSignStartedProperty = DependencyProperty.Register(name: nameof(IsSignStarted), propertyType: typeof(bool),
            ownerType: typeof(NameAndSignaturePanelControl), typeMetadata: new PropertyMetadata(defaultValue: false));

        public bool IsSignStarted
        {
            get { return (bool)GetValue(IsSignStartedProperty); }
            set { SetValue(IsSignStartedProperty, value); }
        }

        public NameAndSignaturePanelControl()
        {
            this.InitializeComponent();

            DataContext = this;

            signatureCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse |
                Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch;

            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();

            drawingAttributes.Color = Windows.UI.Colors.Black;
            drawingAttributes.IgnorePressure = false;
            drawingAttributes.FitToCurve = true;

            signatureCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);

            signatureCanvas.InkPresenter.StrokesCollected += SignatureCanvasInkPresenter_StrokesCollected;
        }

        private void SignatureCanvasInkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            IsSignStarted = true;
        }

        private void nameTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {

        }

        private void ClearSignatureButton_Click(object sender, RoutedEventArgs e)
        {
            signatureCanvas.InkPresenter.StrokeContainer.Clear();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            signatureCanvas.InkPresenter.StrokeContainer.Clear();

            //nameTextBox.Text = PrintName = string.Empty;

            IsSignStarted = false;

            CancelCommand.Execute(null);
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "OrderSignature.jpg";
            if (!string.IsNullOrWhiteSpace(SignatureFileName) && SignatureFileName.IndexOf("LocalState") != -1)
                fileName = SignatureFileName.Substring(SignatureFileName.IndexOf("LocalState") + 11);
            else fileName = SignatureFileName;

            var signature = await CaptureSignatureHelper.SaveSignatureToStorageFile(signatureCanvas, fileName);

            if (string.IsNullOrWhiteSpace(nameTextBox.Text.Trim()) || signature == null || signature.ContentType.Length == 0)
            {
                ContentDialog emptyFieldDialog = new ContentDialog
                {
                    Title = "Confirm Order Error",
                    Content = "Please enter both name and signature to confirm the order.",
                    CloseButtonText = "OK"
                };

                await emptyFieldDialog.ShowAsync();
            }
            else
            {
                string printName = nameTextBox.Text.Trim();

                PrintName = printName;

                signatureCanvas.InkPresenter.StrokeContainer.Clear();

                SaveSignatureCommand.Execute(printName);
            }
        }
    }
}