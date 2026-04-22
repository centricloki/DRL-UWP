using DRLMobile.Core.Helpers;
using DRLMobile.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.Services
{
    public class BackgroundDownloadService : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        readonly BackgroundDownloader BackgroundDownloader;
        private CancellationTokenSource CancellationToken;
        private readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;


        private string _percentageString;
        public string PercentageString
        {
            get { return _percentageString; }
            set { _percentageString = value; NotifyPropertyChanged(); }
        }

        private double _percentage;
        public double Percentage
        {
            get { return _percentage; }
            set { _percentage = value; NotifyPropertyChanged(); }
        }

        public BackgroundDownloadService()
        {
            BackgroundDownloader = new BackgroundDownloader();
            CancellationToken = new CancellationTokenSource();
            PercentageString = "0.00 %";
        }

        #region private methods
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private async Task<bool> HandleDownloadAsync(DownloadOperation download)
        {
            try
            {
                Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(DownloadProgress);

                ResponseInformation response = download.GetResponseInformation();

                await download.StartAsync().AsTask(CancellationToken.Token, progressCallback).ConfigureAwait(false);

                var statusCode = response != null ? response.StatusCode.ToString() : string.Empty;

                return !string.IsNullOrWhiteSpace(statusCode);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog("BackgroundDownloadService", "HandleDownloadAsync", ex);

                return false;
            }
        }

        private bool AcceptAllCertifications(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void DownloadProgress(DownloadOperation download)
        {
            // DownloadOperation.Progress is updated in real-time while the operation is ongoing. Therefore,
            // we must make a local copy so that we can have a consistent view of that ever-changing state
            // throughout this method's lifetime.
            BackgroundDownloadProgress currentProgress = download.Progress;

            if (currentProgress.TotalBytesToReceive > 0)
            {
                Percentage = (double)currentProgress.BytesReceived * 100 / currentProgress.TotalBytesToReceive;
                PercentageString = Percentage.ToString("0.##") + " %";
            }
            if (currentProgress.HasRestarted)
            {
                Debug.WriteLine(" - Download restarted");
            }

            if (currentProgress.HasResponseChanged)
            {
                // We have received new response headers from the server.
                // Be aware that GetResponseInformation() returns null for non-HTTP transfers (e.g., FTP).
                ResponseInformation response = download.GetResponseInformation();
                int headersCount = response != null ? response.Headers.Count : 0;

                Debug.WriteLine(" - Response updated; Header count: " + headersCount);
            }
        }

        /// <summary>
        /// Open the launcher to update the app from the downloaded file
        /// </summary>
        /// <param name="destinationFile"></param>
        /// <returns></returns>
        private async Task AppUpdateProcess()
        {
            var result = await ShowConfirmationAlert("Alert", "Want to Close the application for Update?");

            if (result)
            {
                StorageFolder picturesLibrary = await KnownFolders.GetFolderForUserAsync(null /* current user */, KnownFolderId.PicturesLibrary);
                var path = Path.Combine(picturesLibrary.Path, "test.msixbundle");
                var msix = await picturesLibrary.GetFileAsync(path);
                //test.msixbundle
                var options = new Windows.System.LauncherOptions();
                options.DisplayApplicationPicker = false;
                await Windows.System.Launcher.LaunchFileAsync(msix, options);
                CoreApplication.Exit();
            }
        }

        private async Task<bool> ShowConfirmationAlert(string title, string msg)
        {
            ContentDialog userLogoutDialog = new ContentDialog
            {
                Title = title,
                Content = msg,
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No"
            };

            var result = await userLogoutDialog.ShowAsync();
            
            userLogoutDialog.Hide();

            return result == ContentDialogResult.Primary;
        }

        #endregion


        public async Task<string> DownloadFile(string uri, string fileName, Dictionary<string, string> headers = null, bool launchFile = false, bool launchFileWithOptions = false, bool isAppUpdate = false)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);

                Uri source = new Uri(uri);
                
                var destinationFile = await LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                DownloadOperation download = BackgroundDownloader.CreateDownload(source, destinationFile);
                
                var authenticationString = $"{ApplicationConstants.ProdHttpAuthUserName}:{ApplicationConstants.ProdHttpAuthPassword}";

                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

                download.SetRequestHeader("Authorization", "Basic " + base64EncodedAuthenticationString);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        download.SetRequestHeader(header.Key, header.Value);
                    }
                }

                var isDownloadComplete = await HandleDownloadAsync(download);

                if (isDownloadComplete)
                {
                    if (isAppUpdate)
                    {
                        await AppUpdateProcess();
                    }
                    else if (launchFile)
                    {
                        var options = new Windows.System.LauncherOptions();
                        options.DisplayApplicationPicker = launchFileWithOptions;
                        await Windows.System.Launcher.LaunchFileAsync(destinationFile, options);
                    }
                }

                return destinationFile.Path;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog("BackgroundDownloadService", "DownloadFile", ex.StackTrace);

                return string.Empty;
            }
        }

        public async Task<string> DownLoadCustomerDocFile(string customerDocumentPath, string customerDocumentFileName)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);

                Uri custSource = new Uri(customerDocumentPath);

                var custDestinationFile = await LocalFolder.CreateFileAsync(customerDocumentFileName, CreationCollisionOption.ReplaceExisting);

                DownloadOperation custDownload = BackgroundDownloader.CreateDownload(custSource, custDestinationFile);

                var authenticationString = $"{ApplicationConstants.ProdHttpAuthUserName}:{ApplicationConstants.ProdHttpAuthPassword}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

                custDownload.SetRequestHeader("Authorization", "Basic " + base64EncodedAuthenticationString);

                var isCustDownloadComplete = await HandleDownloadAsync(custDownload);

                return custDestinationFile.Path;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog("BackgroundDownloadService", "DownLoadCustomerDocFile", ex.StackTrace);
                return string.Empty;
            }
        }

        public async Task<string> DownLoadPartialSRCFile(string PartialSRCPath, string PartialSRCFileName)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);

                Uri custSource = new Uri(PartialSRCPath);

                var partialSRCDestinationFile = await LocalFolder.CreateFileAsync(PartialSRCFileName, CreationCollisionOption.ReplaceExisting);

                DownloadOperation partialSRCDownload = BackgroundDownloader.CreateDownload(custSource, partialSRCDestinationFile);

                var authenticationString = $"{ApplicationConstants.ProdHttpAuthUserName}:{ApplicationConstants.ProdHttpAuthPassword}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

                partialSRCDownload.SetRequestHeader("Authorization", "Basic " + base64EncodedAuthenticationString);

                var isCustDownloadComplete = await HandleDownloadAsync(partialSRCDownload);

                return partialSRCDestinationFile.Path;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog("BackgroundDownloadService", "DownLoadCustomerDocFile", ex.StackTrace);
                return string.Empty;
            }
        }

        public void CancelDownload()
        {
            try
            {
                CancellationToken.Cancel();
                CancellationToken.Dispose();
                CancellationToken = new CancellationTokenSource();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog("BackgroundDownloadService", "CancelDownload", ex.StackTrace);
            }
        }

        public void Dispose()
        {
            if (CancellationToken != null)
            {
                CancellationToken.Dispose();
                CancellationToken = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
