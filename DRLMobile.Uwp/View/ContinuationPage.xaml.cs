using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DRLMobile.Uwp.Helpers;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContinuationPage : Page
    {
       //private const string path = @"//Assets//CustomerProfile//save_hover.png";

        public ContinuationPage(RichTextBlockOverflow textLinkContainer)
        {
           

            this.InitializeComponent();
            textLinkContainer.OverflowContentTarget = ContinuationPageLinkedContainer;
            //string signaturePath = DRLMobile.Uwp.Constants.Constants.signatureUrl;
            //BitmapImage sign = null;
            //RenderTargetBitmap renderTargetBitmap = null;
            //SvgImageSource svgsource = null;
            //if (!string.IsNullOrEmpty(signaturePath))
            //{

            //    Uri uri = new Uri(signaturePath);
            //    sign = new BitmapImage(uri);
            //    svgsource = new SvgImageSource(uri);
               

            //}
            //if (ScenarioImage != null)
            //{
            //    ScenarioImage.Source = sign;
            //}



        }
    }
}
