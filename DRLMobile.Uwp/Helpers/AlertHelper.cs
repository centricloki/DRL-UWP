using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.Helpers
{
    public sealed class AlertHelper
    {
        private AlertHelper() { }
        private static readonly Lazy<AlertHelper> lazy = new Lazy<AlertHelper>(() => new AlertHelper());
        public static AlertHelper Instance => lazy.Value;
        private bool isDialogOpen = false;
        public async Task<bool> ShowConfirmationAlert(string title, string msg, string primaryButton, string secondaryButton = "")
        {
            bool result = false;
            if (!isDialogOpen)
            {
                isDialogOpen = true;
                ContentDialog userLogoutDialog = new ContentDialog
                {
                    Title = title,
                    //Content = msg,
                    PrimaryButtonText = primaryButton,
                    SecondaryButtonText = secondaryButton
                };
                var scrollViewer = new ScrollViewer
                {
                    MaxHeight = 300, // Optional: limit scrollable area
                    Content = new TextBlock
                    {
                        Text = msg,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(12, 0, 12, 0)
                    }
                };
                userLogoutDialog.Content = scrollViewer;
                var dialogResult = await userLogoutDialog.ShowAsync();
                if (dialogResult == ContentDialogResult.Primary)
                    result = true;
                else result = false;

                userLogoutDialog.Hide();
                isDialogOpen = false;
            }
            return result;
        }
    }
}
