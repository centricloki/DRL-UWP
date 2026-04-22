using DRLMobile.Core.Enums;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Core.Services;
using DRLMobile.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DRLMobile.CustomControls
{
    public sealed partial class CustomerDocumentControl : UserControl
    {
        public CustomerDocumentControl()
        {
            this.InitializeComponent();
            DataContext = this;

        }

        #region Depencency properties

        public CustomerMaster CustomerData
        {
            get { return (CustomerMaster)GetValue(CustomerDataProperty); }
            set { SetValue(CustomerDataProperty, value); }
        }
        public static readonly DependencyProperty CustomerDataProperty =
            DependencyProperty.Register(name: nameof(CustomerData), propertyType: typeof(CustomerMaster),
               ownerType: typeof(CustomerDocumentControl),
                typeMetadata: new PropertyMetadata(defaultValue: null));


        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }
        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register(name: nameof(CancelCommand), propertyType: typeof(ICommand),
               ownerType: typeof(CustomerDocumentControl),
                typeMetadata: new PropertyMetadata(defaultValue: string.Empty));



        public ICommand PreviewCommand
        {
            get { return (ICommand)GetValue(PreviewCommandProperty); }
            set { SetValue(PreviewCommandProperty, value); }
        }
        public static readonly DependencyProperty PreviewCommandProperty =
            DependencyProperty.Register(name: nameof(PreviewCommand), propertyType: typeof(ICommand),
               ownerType: typeof(CustomerDocumentControl),
                typeMetadata: new PropertyMetadata(defaultValue: null));



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



        public ObservableCollection<string> DocumentTypeList { get { return GetDocumentTypeList(); } }


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




        #endregion

        #region fields
        public bool IsPublishToChildChecked { get; private set; }
        private IStorageFile LocalStoredFile;
        private readonly App AppReference = (App)(Application.Current);
        #endregion


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
        }

        private void AddDocCancelButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewDocGrid.Visibility = Visibility.Collapsed;
            ResetData();
        }

        private async void SaveDocButton_Click(object sender, RoutedEventArgs e)
        {
            if (LocalStoredFile == null)
            {
                await ShowConfirmationAlert(Helpers.ResourceExtensions.GetLocalized("ERROR"), Helpers.ResourceExtensions.GetLocalized("SelectDocument"), Helpers.ResourceExtensions.GetLocalized("OK"));
            }
            else if (string.IsNullOrWhiteSpace(SelectedDocType))
            {
                await ShowConfirmationAlert(Helpers.ResourceExtensions.GetLocalized("ERROR"), Helpers.ResourceExtensions.GetLocalized("SelectDocumentType"), Helpers.ResourceExtensions.GetLocalized("OK"));
            }
            else
            {
                ///insert data into DB
                var newDoc = new CustomerDocumentUIModel() { CustomerId = CustomerData.CustomerID.Value, DocDesc = DocDescTextBlock.Text, DocName = Core.Helpers.HelperMethods.GetNameFromURL(LocalStoredFile.Name), DocUrl = LocalStoredFile.Path, DocType = SelectedDocType, IsPreviewIconVisible = true, IsPublishToChildren = IsPublishToChildChecked };
                var updatedDoc = await AppReference.QueryService.InsertOrUpdateCustomerDocument(newDoc);
                newDoc.DocumentId = updatedDoc.CustomerDocumentID;
                newDoc.CustomerDocument = updatedDoc;
                DocumentList.Add(newDoc);
                ///NewDocImageSource = LocalStoredFile.Path;
                AddNewDocGrid.Visibility = Visibility.Collapsed;
                SelectedDocType = null;
                NewDocImageSource = null;
                DocDescTextBlock.Text = string.Empty;
                LocalStoredFile = null;
                ListViewDocumentType.SelectedItem = null;
                ///ResetData();
            }
        }

        private void UploadNewButton_Click(object sender, RoutedEventArgs e)
        {
            ParentChildCheckBox.Visibility = CustomerData?.IsParent == 1 ? Visibility.Visible : Visibility.Collapsed;
            AddNewDocGrid.Visibility = Visibility.Visible;
        }

        private ObservableCollection<string> GetDocumentTypeList()
        {
            var list = new ObservableCollection<string>();
            list.Add("Authorization Letter");
            list.Add("Cash Sale");
            list.Add("Contracts");
            list.Add("Distribution Request / Pre-book");
            list.Add("Flyers");
            list.Add("Other");
            list.Add("Picture");
            list.Add("Planogram");
            list.Add("Store List");
            list.Add("Supporting Sales Documents");
            list.Add("Travel");
            list.Add("UIN List");
            list.Add("Wholesale Invoices");
            return list;
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
                    await OpenFilePicker(photoTypes);
                    break;
                case "Doc":
                    var docTyped = new List<string>() { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf" };
                    await OpenFilePicker(docTyped);
                    break;
            }

        }

        private async Task OpenCamera()
        {
            var camera = new CameraCaptureUI();
            camera.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            camera.PhotoSettings.AllowCropping = false;
            var pickedOrClickedFile = await camera.CaptureFileAsync(CameraCaptureUIMode.Photo);
            LocalStoredFile = await CopyStorageFileToLocal(pickedOrClickedFile);
            NewDocImageSource = LocalStoredFile?.Path;
        }

        private async Task OpenFilePicker(ICollection<string> types)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            foreach (var item in types)
            {
                openPicker.FileTypeFilter.Add(item);
            }

            var pickedOrClickedFile = await openPicker.PickSingleFileAsync();
            LocalStoredFile = await CopyStorageFileToLocal(pickedOrClickedFile);
            NewDocImageSource = LocalStoredFile?.Path;
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
            var isConfirmed = await ShowConfirmationAlert(Helpers.ResourceExtensions.GetLocalized("Confirmation"),
                Helpers.ResourceExtensions.GetLocalized("DocumentDeleteConfimationMsg") + " " + dataContext.DocName+ " ?",
                Helpers.ResourceExtensions.GetLocalized("YesText"), Helpers.ResourceExtensions.GetLocalized("NoText"));

            if (isConfirmed)
            {
                var result = await AppReference.QueryService.InsertOrUpdateCustomerDocument(dataContext);
                if (result != null)
                {
                    DocumentList.Remove(dataContext);
                }
            }
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var docImage = (string)Application.Current.Resources["DocumentIconImage"];
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
                var result = await InvokeWebService.DownloadDocumentAndFileFromServer(dataContext.DocUrl, fileType.ToString());

                if (!string.IsNullOrWhiteSpace(result))
                {
                    PreviewCommand?.Execute(result);
                }
                else
                {
                    await AlertHelper.Instance.ShowConfirmationAlert(ResourceExtensions.GetLocalized("ALERT"), ResourceExtensions.GetLocalized("FileDoesNotExist"), ResourceExtensions.GetLocalized("OK"), string.Empty);
                }

                dataContext.IsProgressVisible = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                dataContext.IsProgressVisible = false;
            }

            //_WholesaleInvoices_08052019_212258.jpg
        }
    }
}
