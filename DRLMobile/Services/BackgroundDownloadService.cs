using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using System.Diagnostics;
using System.Threading;
using Windows.Storage;
using System.IO;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DRLMobile.Core.Helpers;
using System.Text;
using DRLMobile.ExceptionHandler;

namespace DRLMobile.Services
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
                ///ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);
                ///ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ///ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(DownloadProgress);
                ResponseInformation response = download.GetResponseInformation();

                ///ServicePointManager.Expect100Continue = true;
                ///ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ///ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                await download.StartAsync().AsTask(CancellationToken.Token, progressCallback);

                var statusCode = response != null ? response.StatusCode.ToString() : string.Empty;

                if (!string.IsNullOrWhiteSpace(statusCode))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
            if (result == ContentDialogResult.Primary)
            {
                return true;
            }
            else return false;

        }

        #endregion


        public async Task<string> DownloadFile(string uri, string fileName, Dictionary<string, string> headers = null, bool launchFile = false, bool launchFileWithOptions = false, bool isAppUpdate = false)
        {
            ///https://s2.q4cdn.com/175719177/files/doc_presentations/Placeholder-PDF.pdf
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);

                /// ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ///ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                Uri source = new Uri(uri);
                var destinationFile = await LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                #region commented code
                ///BackgroundDownloader.ServerCredential.UserName = "administrator";
                ///BackgroundDownloader.ServerCredential.Password =  "Pa55w0rd";

                ///using (var handler = new HttpClientHandler())
                ///{
                ///    using (HttpClient client = new HttpClient(handler))
                ///    {
                ///        HttpRequestMessage httpRequestMessage = new HttpRequestMessage();

                ///        httpRequestMessage.Method = HttpMethod.Post;
                ///        httpRequestMessage.RequestUri = new Uri(uri);
                ///        var authenticationString = $"{ApplicationConstants.DevHttpAuthUserName}:{ApplicationConstants.DevHttpAuthPassword}";
                ///        var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
                ///        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

                ///    }
                ///}
                ///
                #endregion
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
                Debug.WriteLine(ex.Message);
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
                ErrorLogger.WriteToErrorLog(nameof(BackgroundDownloadService), nameof(CancelDownload), ex.StackTrace);
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
