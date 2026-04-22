using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

using DRLMobile.Core.Enums;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;

using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DRLMobile.Uwp.CustomControls
{
    public sealed partial class CustomerDocumentControl : UserControl, INotifyPropertyChanged
    {

        private readonly App AppRef = (App)Application.Current;
        public CustomerDocumentControl()
        {
            this.InitializeComponent();
            DataContext = this;
            IsLoading = false;
            this.Loaded += CustomerDocumentControl_Loaded;
        }

        private async void CustomerDocumentControl_Loaded(object sender, RoutedEventArgs e)
        {
            await GetDocumentTypeListAsync();
        }

        #region Dependency properties

        public CustomerMaster CustomerData
        {
            get { return (CustomerMaster)GetValue(CustomerDataProperty); }
            set { SetValue(CustomerDataProperty, value); }
        }

        public static readonly DependencyProperty CustomerDataProperty =
            DependencyProperty.Register(name: nameof(CustomerData), propertyType: typeof(CustomerMaster),
               ownerType: typeof(CustomerDocumentControl), typeMetadata: new PropertyMetadata(defaultValue: null));

        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register(name: nameof(CancelCommand), propertyType: typeof(ICommand),
               ownerType: typeof(CustomerDocumentControl), typeMetadata: new PropertyMetadata(defaultValue: string.Empty));

        public ICommand PreviewCommand
        {
            get { return (ICommand)GetValue(PreviewCommandProperty); }
            set { SetValue(PreviewCommandProperty, value); }
        }

        public static readonly DependencyProperty PreviewCommandProperty =
            DependencyProperty.Register(name: nameof(PreviewCommand), propertyType: typeof(ICommand),
               ownerType: typeof(CustomerDocumentControl), typeMetadata: new PropertyMetadata(defaultValue: null));

        public ObservableCollection<CustomerDocumentUIModel> DocumentList
        {
            get { return (ObservableCollection<CustomerDocumentUIModel>)GetValue(DocumentListProperty); }
            set { SetValue(DocumentListProperty, value); }
        }
        public static readonly DependencyProperty DocumentListProperty =
            DependencyProperty.Register(name: nameof(DocumentList), propertyType: typeof(ObservableCollection<CustomerDocumentUIModel>),
               ownerType: typeof(CustomerDocumentControl),
                typeMetadata: new PropertyMetadata(defaultValue: null, propertyChangedCallback: ListItemSourceChanged));

        public bool ResetControl
        {
            get { return (bool)GetValue(ResetControlProperty); }
            set { SetValue(ResetControlProperty, value); }
        }
        public static readonly DependencyProperty ResetControlProperty =
            DependencyProperty.Register(name: nameof(ResetControl), propertyType: typeof(bool),
               ownerType: typeof(CustomerDocumentControl),
                typeMetadata: new PropertyMetadata(defaultValue: false, propertyChangedCallback: ResetControlHandler));

        public ObservableCollection<string> DocumentTypeList { set; get; } = new ObservableCollection<string>();

        private string _selectedDocType;
        public string SelectedDocType
        {
            get { return _selectedDocType; }
            set { _selectedDocType = value; DocTypeSelected(); }
        }

        private string _newDocImageSource;
        public string NewDocImageSource
        {
            get { return _newDocImageSource; }
            set { _newDocImageSource = value; HandleImageSourceChange(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; NotifyPropertyChanged(); }
        }

        #endregion

        #region fields
        public bool IsPublishToChildChecked { get; private set; }
        private IStorageFile LocalStoredFile;
        private readonly App AppReference = (App)(Application.Current);

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DocumentList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NoDocTextVisibilityHandler();
        }

        private static void ListItemSourceChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            CustomerDocumentControl cdc = control as CustomerDocumentControl;
            cdc.NoDocTextVisibilityHandler();
            cdc.DocumentList.CollectionChanged -= cdc.DocumentList_CollectionChanged;
            cdc.DocumentList.CollectionChanged += cdc.DocumentList_CollectionChanged;
        }

        private void NoDocTextVisibilityHandler()
        {
            if (DocumentList != null && DocumentList.Count > 0)
            {
                NoDocTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoDocTextBlock.Visibility = Visibility.Visible;
            }
        }

        private static void ResetControlHandler(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            CustomerDocumentControl cdc = control as CustomerDocumentControl;
            if (cdc.ResetControl)
            {
                cdc.ResetData();
            }
        }

        private void ResetData()
        {
            SelectedDocType = null;
            AddNewDocGrid.Visibility = Visibility.Collapsed;
            NewDocImageSource = null;
            LocalStoredFile = null;
            DocDescTextBlock.Text = string.Empty;
            ParentChildCheckBox.IsChecked = false;
            IsLoading = false;
        }

        private void AddDocCancelButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewDocGrid.Visibility = Visibility.Collapsed;
            ResetData();
        }

        private async void SaveDocButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LocalStoredFile == null)
                {
                    await ShowConfirmationAlert(ResourceExtensions.GetLocalized("ERROR"), ResourceExtensions.GetLocalized("SelectDocument"), ResourceExtensions.GetLocalized("OK"));
                }
                else if (string.IsNullOrWhiteSpace(SelectedDocType))
                {
                    await ShowConfirmationAlert(ResourceExtensions.GetLocalized("ERROR"), ResourceExtensions.GetLocalized("SelectDocumentType"), ResourceExtensions.GetLocalized("OK"));
                }
                else
                {
                    ///insert data into DB
                    IsLoading = true;
                    var datetime = DateTime.Now;
                    var extension = Path.GetExtension(LocalStoredFile.Name);
                    var fileNameWithCustomerNumber = ((!string.IsNullOrWhiteSpace(CustomerData?.CustomerNumber) ? CustomerData?.CustomerNumber : CustomerData?.DeviceCustomerID) + "_"
                        + SelectedDocType + "_" + datetime.ToString("MMddyyyy") + "_" + datetime.Ticks + extension).Replace(" ", "_").Replace("/", "");

                    await LocalStoredFile.RenameAsync(fileNameWithCustomerNumber);

                    var newDoc = new CustomerDocumentUIModel()
                    {
                        CustomerId = CustomerData.CustomerID.Value,
                        DocDesc = DocDescTextBlock.Text,
                        DocName = Core.Helpers.HelperMethods.GetNameFromURL(LocalStoredFile.Name),
                        DocUrl = LocalStoredFile.Path,
                        DisplayDocUrl = GetTheImageThumblineAsPerDocType(LocalStoredFile.Path),
                        DocType = SelectedDocType,
                        IsPreviewIconVisible = true,
                        IsPublishToChildren = IsPublishToChildChecked,
                        DocumentDateTime = datetime
                    };
                    var updatedDoc = await AppReference.QueryService.InsertOrUpdateCustomerDocument(newDoc);

                    newDoc.DocumentId = updatedDoc.CustomerDocumentID;

                    newDoc.CustomerDocument = updatedDoc;
                    DocumentList.Insert(0, newDoc);
                    //DocumentList.Add(newDoc);

                    //var orderedList = DocumentList.OrderByDescending(a => a.DocumentDateTime).ToList();

                    //DocumentList.Clear();

                    ////DocumentList = new ObservableCollection<CustomerDocumentUIModel>(orderedList);
                    //foreach (var item in orderedList)
                    //{
                    //    DocumentList.Add(item);
                    //}

                    AddNewDocGrid.Visibility = Visibility.Collapsed;

                    SelectedDocType = null;
                    NewDocImageSource = null;
                    LocalStoredFile = null;
                    ListViewDocumentType.SelectedItem = null;

                    DocDescTextBlock.Text = string.Empty;

                    IsLoading = false;

                    await AlertHelper.Instance.ShowConfirmationAlert("Success", "Document has been saved to your device", "OK");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(CustomerDocumentControl), nameof(SaveDocButton_Click), ex.StackTrace);
            }
        }

        private string GetTheImageThumblineAsPerDocType(string path)
        {
            //".jpg", ".jpeg", ".png", ".bmp"
            var returnPath = "";
            if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg") || path.ToLower().Contains(".png") || path.ToLower().Contains(".bmp"))
            {
                returnPath = path.ToUpper().StartsWith("HTTP") ? string.Empty : path;
            }
            else if (path.ToLower().Contains(".pdf"))
            {
                returnPath = (string)Application.Current.Resources["DocumentIconImage"];
            }
            else
            {
                returnPath = (string)Application.Current.Resources["DocumentIcon"];
            }

            return returnPath;
        }

        private void UploadNewButton_Click(object sender, RoutedEventArgs e)
        {
            ParentChildCheckBox.Visibility = CustomerData?.IsParent == 1 ? Visibility.Visible : Visibility.Collapsed;
            AddNewDocGrid.Visibility = Visibility.Visible;
        }

        private async Task GetDocumentTypeListAsync()
        {
            foreach (string item in await AppRef.QueryService.GetDocumentTypeAsync()) DocumentTypeList.Add(item);
        }

        private async void HandleImageSourceChange()
        {
            try
            {
                await Task.Delay(100);
                if (string.IsNullOrWhiteSpace(NewDocImageSource))
                {
                    var noProductImage = (string)Application.Current.Resources["NoProduct"];
                    NewDocImage.Source = new BitmapImage(new Uri(noProductImage));
                }
                else
                {
                    NewDocImage.Source = new BitmapImage(new Uri(NewDocImageSource, UriKind.RelativeOrAbsolute));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void OpenDocumentTypeFlyout(object sender, TappedRoutedEventArgs e)
        {
            DocTypeFlyout.ShowAt(AddDocTypeGrid);
        }

        private void DocTypeSelected()
        {
            DocTypeFlyout.Hide();
            DocTypeTextBlock.Text = string.IsNullOrWhiteSpace(SelectedDocType) ? "Add Document Type" : SelectedDocType;
        }

        private void OpenDocActionFlyout(object sender, TappedRoutedEventArgs e)
        {
            DocActionFlyout.ShowAt(DocActionGrid);
        }

        private async void OpenAddDocumentBrowser(object sender, RoutedEventArgs e)
        {
            DocActionFlyout.Hide();
            var button = sender as Button;

            switch (button.Tag)
            {
                case "Camera":
                    await OpenCamera();
                    break;
                case "Photo":
                    var photoTypes = new List<string>() { ".jpg", ".jpeg", ".png", ".bmp" };
                    await OpenFilePicker(photoTypes, PickerLocationId.PicturesLibrary);
                    break;
                case "Doc":
                    var docTyped = new List<string>() { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf" };
                    await OpenFilePicker(docTyped, PickerLocationId.DocumentsLibrary);
                    break;
            }

        }

        private async Task OpenCamera()
        {
            var camera = new CameraCaptureUI();
            camera.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            camera.PhotoSettings.AllowCropping = false;
            var pickedOrClickedFile = await camera.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (pickedOrClickedFile == null)
            {
                // User cancelled the file picker. Exit the method.
                //LocalStoredFile = null; // Ensure LocalStoredFile is reset if desired
                return;
            }
            LocalStoredFile = await CopyStorageFileToLocal(pickedOrClickedFile);
            if (LocalStoredFile == null)
            {
                // Copying failed for some reason (e.g., internal exception in CopyStorageFileToLocal)
                // You might want to show an alert here as well.
                return;
            }
            // Get file properties, including file size
            var basicProperties = await LocalStoredFile.GetBasicPropertiesAsync();
            ulong fileSize = basicProperties.Size;  // Size in bytes
            // Define a size limit (e.g., 10 MB)
            ulong maxSize = 10 * 1024 * 1024;  // 10 MB in bytes
            // Validate file size
            if (fileSize > maxSize)
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "The file size exceeds the 10 MB limit. Please upload a smaller file.", "OK");
                LocalStoredFile = null;
            }
            else
            {
                NewDocImageSource = LocalStoredFile?.Path;
            }
        }

        private async Task OpenFilePicker(ICollection<string> types, PickerLocationId suggestionLocation)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = suggestionLocation;
            foreach (var item in types)
            {
                openPicker.FileTypeFilter.Add(item);
            }

            var pickedOrClickedFile = await openPicker.PickSingleFileAsync();
            if (pickedOrClickedFile == null)
            {
                // User cancelled the file picker. Exit the method.
                //LocalStoredFile = null; // Ensure LocalStoredFile is reset if desired
                return;
            }
            LocalStoredFile = await CopyStorageFileToLocal(pickedOrClickedFile);
            if (LocalStoredFile == null)
            {
                // Copying failed for some reason (e.g., internal exception in CopyStorageFileToLocal)
                // You might want to show an alert here as well.
                return;
            }
            // Get file properties, including file size
            var basicProperties = await LocalStoredFile.GetBasicPropertiesAsync();
            ulong fileSize = basicProperties.Size;  // Size in bytes
            // Define a size limit (e.g., 10 MB)
            ulong maxSize = 10 * 1024 * 1024;  // 10 MB in bytes
            // Validate file size
            if (fileSize > maxSize)
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "The file size exceeds the 10 MB limit. Please upload a smaller file.", "OK");
                LocalStoredFile = null;
            }
            else
            {
                NewDocImageSource = LocalStoredFile?.Path;
            }
        }

        private async Task<IStorageFile> CopyStorageFileToLocal(IStorageFile file)
        {
            try
            {
                if (file != null)
                {
                    var folder = ApplicationData.Current.LocalFolder;
                    var newFolder = await folder.CreateFolderAsync("CustomerDocuments", CreationCollisionOption.OpenIfExists);
                    var newFile = await file.CopyAsync(newFolder, file.Name, NameCollisionOption.ReplaceExisting);
                    return newFile;
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<bool> ShowConfirmationAlert(string title, string msg, string primaryButton, string secondaryButton = "")
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

        private void PubishToChildCheckBoxToggled(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb.IsChecked.HasValue)
                IsPublishToChildChecked = cb.IsChecked.Value;
        }

        private async void DeleteIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dataContext = (sender as FontIcon).DataContext as CustomerDocumentUIModel;
            if (!File.Exists(dataContext.DocUrl))
            {
                await AlertHelper.Instance.ShowConfirmationAlert(ResourceExtensions.GetLocalized("ALERT"), ResourceExtensions.GetLocalized("CustomerDocumentFileDeleteFailed"), ResourceExtensions.GetLocalized("OK"), string.Empty);
                return;
            }
            var isConfirmed = await ShowConfirmationAlert(Helpers.ResourceExtensions.GetLocalized("Confirmation"),
                ResourceExtensions.GetLocalized("DocumentDeleteConfimationMsg") + " " + dataContext.DocName + " ?",
                ResourceExtensions.GetLocalized("YesText"), Helpers.ResourceExtensions.GetLocalized("NoText"));

            if (isConfirmed)
            {
                var isexported = dataContext.CustomerDocument.IsExported;
                var result = await AppReference.QueryService.InsertOrUpdateCustomerDocument(dataContext);
                if (result != null)
                {
                    DocumentList.Remove(dataContext);
                    //if(isexported == 0 ) // delete local file if deleted before sync
                    {
                        if (File.Exists(dataContext.DocUrl))
                        { File.Delete(dataContext.DocUrl); }
                    }
                }
            }
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var source = ((sender as Image).Source as BitmapImage).UriSource.AbsoluteUri;
            var docImage = GetTheImageThumblineAsPerDocType(source);
            (sender as Image).Source = new BitmapImage(new Uri(docImage));

        }

        private void PreviewIconTapped(object sender, TappedRoutedEventArgs e)
        {
            var dataContext = (sender as FontIcon).DataContext as CustomerDocumentUIModel;
            PreviewCommand?.Execute(dataContext.DocUrl);
        }

        private async void DownloadDocumentIconTapped(object sender, TappedRoutedEventArgs e)
        {
            var dataContext = (sender as FontIcon).DataContext as CustomerDocumentUIModel;
            try
            {
                dataContext.IsProgressVisible = true;
                var fileType = (int)FileType.Customer;
                string result = string.Empty;
                // Check in Local
                result = AppRef.LocalFileService.GetLocalFilePathByFileTypeForCustomerDocument(dataContext.DocUrl);
                // If not in local get from server
                if (string.IsNullOrWhiteSpace(result))
                {
                    result = await InvokeWebService.DownloadDocumentAndFileFromServer(Core.Helpers.HelperMethods.GetNameFromURL(dataContext.DocUrl), fileType.ToString());
                }

                if (!string.IsNullOrWhiteSpace(result))
                {
                    PreviewCommand?.Execute(result);
                }
                else
                {
                    await AlertHelper.Instance.ShowConfirmationAlert(ResourceExtensions.GetLocalized("ALERT"), ResourceExtensions.GetLocalized("CustomerDocumentFileDownloadFailed"), ResourceExtensions.GetLocalized("OK"), string.Empty);
                }

                dataContext.IsProgressVisible = false;
            }
            catch (Exception ex)
            {
                // Debug.WriteLine(ex.StackTrace);
                ErrorLogger.WriteToErrorLog(nameof(CustomerDocumentControl), nameof(DownloadDocumentIconTapped), ex);
                dataContext.IsProgressVisible = false;
            }

            //_WholesaleInvoices_08052019_212258.jpg
        }
    }
}
