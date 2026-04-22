using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Helpers
{
    public class AlertHelper
    {
        private static AlertHelper instance = null;
        private static readonly object _lock = new object();

        private AlertHelper()
        {

        }
        public static AlertHelper Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new AlertHelper();
                    }
                    return instance;
                }
            }
        }



        public async Task<bool> ShowConfirmationAlert(string title, string msg, string primaryButton, string secondaryButton = "")
        {
            ContentDialog userLogoutDialog = new ContentDialog
            {
                Title = title,
                Content = msg,
                PrimaryButtonText = primaryButton,
                SecondaryButtonText = secondaryButton
            };

            var result = await userLogoutDialog.ShowAsync();
            userLogoutDialog.Hide();
            if (result == ContentDialogResult.Primary)
            {
                return true;
            }
            else return false;

        }


    }
}
