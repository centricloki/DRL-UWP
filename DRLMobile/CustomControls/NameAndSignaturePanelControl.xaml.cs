using DRLMobile.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DRLMobile.CustomControls
{
    public sealed partial class NameAndSignaturePanelControl : UserControl
    {
        private RetailTransactionPageViewModel retailTransactionVM = new RetailTransactionPageViewModel();

        public NameAndSignaturePanelControl()
        {
            this.InitializeComponent();

            DataContext = retailTransactionVM;

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
            retailTransactionVM.RetailTransacUiModel.IsSignStarted = true;
        }

        private void nameTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {

        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var signature = await retailTransactionVM.ConvertInkCanvasToWriteableBitmap(signatureCanvas);

            if (string.IsNullOrWhiteSpace(nameTextBox.Text) || signature == null)
            {
                retailTransactionVM.ShowEmptyNameSignatureMessage();
            }
            else
            {
                retailTransactionVM.SaveSignature(signature);
                signatureCanvas.InkPresenter.StrokeContainer.Clear();

                retailTransactionVM.RetailTransacUiModel.PrintName = nameTextBox.Text.Trim();

                retailTransactionVM.RetailTransacUiModel.SignaturePanelVisibility = Visibility.Collapsed;
            }           
        }

        private void ClearSignatureButton_Click(object sender, RoutedEventArgs e)
        {
            signatureCanvas.InkPresenter.StrokeContainer.Clear();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            retailTransactionVM.RetailTransacUiModel.PrintName = string.Empty;

            signatureCanvas.InkPresenter.StrokeContainer.Clear();

            retailTransactionVM.RetailTransacUiModel.SignaturePanelVisibility = Visibility.Collapsed;
        }
    }
}
