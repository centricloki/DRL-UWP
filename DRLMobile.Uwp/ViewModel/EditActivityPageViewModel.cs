using DRLMobile.Core.Enums;
using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;

using Microsoft.Toolkit.Mvvm.Input;

using RestSharp.Extensions;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.ApplicationModel.UserActivities;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace DRLMobile.Uwp.ViewModel
{
    public class EditActivityPageViewModel : BaseModel
    {
        #region fields
        public CallActivityList CurrentActivity { get; set; }
        private readonly App AppReference = (App)(Application.Current);
        private readonly Object thisLock = new Object();
        private bool isOrderActivityEdit;
        private CallActivityList currentActivity;
        private object onNavigationObject;
        private bool isCustomerSeleted = false;
        private UserMaster loggedInUser;
        private Dictionary<int, string> stateDict;
        private List<TerritoryMaster> territories;
        #endregion

        #region properties
        private bool _isSaveIconVisible;
        public bool IsSaveIconVisible
        {
            get { return _isSaveIconVisible; }
            set { SetProperty(ref _isSaveIconVisible, value); }
        }


        private bool _isOrderDetailsVisible;
        public bool IsOrderDetailsVisible
        {
            get { return _isOrderDetailsVisible; }
            set { SetProperty(ref _isOrderDetailsVisible, value); }
        }

        private EditActivityUIModel _uiModel;
        public EditActivityUIModel UIModel
        {
            get { return _uiModel; }
            set { SetProperty(ref _uiModel, value); }
        }

        private ObservableCollection<OrderHistoryDetailsGridUIModel> _rackPosPopOrderDetailsItemSource;
        public ObservableCollection<OrderHistoryDetailsGridUIModel> RackPosPopOrderDetailsItemSource
        {
            get { return _rackPosPopOrderDetailsItemSource; }
            set { SetProperty(ref _rackPosPopOrderDetailsItemSource, value); }
        }

        private bool _loadRackPosPopGrid;
        public bool LoadRackPosPopGrid
        {
            get { return _loadRackPosPopGrid; }
            set { SetProperty(ref _loadRackPosPopGrid, value); }
        }

        private ObservableCollection<OrderHistoryDetailsGridUIModel> _tobaccoGridItemSource;
        public ObservableCollection<OrderHistoryDetailsGridUIModel> TobaccoGridItemSource
        {
            get { return _tobaccoGridItemSource; }
            set { SetProperty(ref _tobaccoGridItemSource, value); }
        }


        private ObservableCollection<OrderHistoryDetailsGridUIModel> _nonTobaccoGridItemSource;
        public ObservableCollection<OrderHistoryDetailsGridUIModel> NonTobaccoGridItemSource
        {
            get { return _nonTobaccoGridItemSource; }
            set { SetProperty(ref _nonTobaccoGridItemSource, value); }
        }

        private ObservableCollection<OrderHistoryDetailsGridUIModel> _rtnGridItemSource;
        public ObservableCollection<OrderHistoryDetailsGridUIModel> RtnGridItemSource
        {
            get { return _rtnGridItemSource; }
            set { SetProperty(ref _rtnGridItemSource, value); }
        }

        private ObservableCollection<OrderHistoryDetailsGridUIModel> _difGridItemSource;
        public ObservableCollection<OrderHistoryDetailsGridUIModel> DifGridItemSource
        {
            get { return _difGridItemSource; }
            set { SetProperty(ref _difGridItemSource, value); }
        }

        private ObservableCollection<string> _activityTypeList;
        public ObservableCollection<string> ActivityTypeList
        {
            get { return _activityTypeList; }
            set { SetProperty(ref _activityTypeList, value); }
        }

        private ObservableCollection<CustomerPageUIModel> _customerList;
        public ObservableCollection<CustomerPageUIModel> CustomerList
        {
            get { return _customerList; }
            set { SetProperty(ref _customerList, value); }
        }

        private bool _loadNonTobaccoGrid;
        public bool LoadNonTobaccoGrid
        {
            get { return _loadNonTobaccoGrid; }
            set { SetProperty(ref _loadNonTobaccoGrid, value); }
        }

        private bool _loadRtnGrid;
        public bool LoadRtnGrid
        {
            get { return _loadRtnGrid; }
            set { SetProperty(ref _loadRtnGrid, value); }
        }

        private bool _loadDifGrid;
        public bool LoadDifGrid
        {
            get { return _loadDifGrid; }
            set { SetProperty(ref _loadDifGrid, value); }
        }

        private bool _loadTobaccoGrid;
        public bool LoadTobaccoGrid
        {
            get { return _loadTobaccoGrid; }
            set { SetProperty(ref _loadTobaccoGrid, value); }
        }

        private bool _loadTotalStackPanel;
        public bool LoadTotalStackPanel
        {
            get { return _loadTotalStackPanel; }
            set { SetProperty(ref _loadTotalStackPanel, value); }
        }

        private string _totalTobacco;
        public string TotalTobacco
        {
            get { return _totalTobacco; }
            set { SetProperty(ref _totalTobacco, value); }
        }

        private string _totalNonTobacco;
        public string TotalNonTobacco
        {
            get { return _totalNonTobacco; }
            set { SetProperty(ref _totalNonTobacco, value); }
        }

        private string _totalRtnTobacco;
        public string TotalRtnTobacco
        {
            get { return _totalRtnTobacco; }
            set { SetProperty(ref _totalRtnTobacco, value); }
        }


        private string _totalDifTobacco;
        public string TotalDifTobacco
        {
            get { return _totalDifTobacco; }
            set { SetProperty(ref _totalDifTobacco, value); }
        }

        private string _combineTotal;
        public string CombineTotal
        {
            get { return _combineTotal; }
            set { SetProperty(ref _combineTotal, value); }
        }


        private bool _iscallDateEditable;
        public bool IscallDateEditable
        {
            get { return _iscallDateEditable; }
            set { SetProperty(ref _iscallDateEditable, value); }
        }

        private bool _isAccountTypeEditable;
        public bool IsAccountTypeEditable
        {
            get { return _isAccountTypeEditable; }
            set { SetProperty(ref _isAccountTypeEditable, value); }
        }


        private bool _isHoursEditable;
        public bool IsHoursEditable
        {
            get { return _isHoursEditable; }
            set { SetProperty(ref _isHoursEditable, value); }
        }

        private bool _isNotesReadOnly;
        public bool IsNotesReadOnly
        {
            get { return _isNotesReadOnly; }
            set { SetProperty(ref _isNotesReadOnly, value); }
        }


        private bool _isAccountNameEditable;
        public bool IsAccountNameEditable
        {
            get { return _isAccountNameEditable; }
            set { SetProperty(ref _isAccountNameEditable, value); }
        }

        private ObservableCollection<AttachmentView> _addedAttachementItemSource;
        public ObservableCollection<AttachmentView> AddedAttachementItemSource
        {
            get { return _addedAttachementItemSource; }
            set { SetProperty(ref _addedAttachementItemSource, value); }
        }

        private bool _isAttachmentIconVisible;
        public bool IsAttachmentIconVisible
        {
            get { return _isAttachmentIconVisible; }
            set { SetProperty(ref _isAttachmentIconVisible, value); }
        }

        private bool _isPreviewIconVisible;
        public bool IsPreviewIconVisible
        {
            get { return _isPreviewIconVisible; }
            set { SetProperty(ref _isPreviewIconVisible, value); IsAnyAttachmentAvailable(value); }
        }

        private bool _isAttachmentListVisible;
        public bool IsAttachmentListVisible
        {
            get { return _isAttachmentListVisible; }
            set { SetProperty(ref _isAttachmentListVisible, value); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private bool _isNarrativeWeeklyReport;
        public bool IsNarrativeWeeklyReport
        {
            get { return _isNarrativeWeeklyReport; }
            set { SetProperty(ref _isNarrativeWeeklyReport, value); }
        }
        private IStorageFile LocalStoredFile;

        private bool _isDirtyConsumerActivationEnagegment;
        public bool IsDirtyConsumerActivationEnagegment
        {
            get { return _isDirtyConsumerActivationEnagegment; }
            set { SetProperty(ref _isDirtyConsumerActivationEnagegment, value); }
        }

        private string loginedInUserCustomerDeviceId;
        #endregion

        #region command
        public ICommand OnNavigatedToCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand HoursSelectionCommand { get; private set; }
        public ICommand ActivityTypeCommand { get; private set; }
        public ICommand AccountNameCommand { get; private set; }
        public ICommand OpenPickerCommand { get; private set; }
        public ICommand PreviewCommand { get; private set; }
        public ICommand RemoveAttachementCommand { get; private set; }
        #endregion

        #region constructor
        public EditActivityPageViewModel()
        {
            IsAttachmentIconVisible = false;
            IsNarrativeWeeklyReport = false;
            IsPreviewIconVisible = false;
            IsAccountNameEditable = false;
            IsAccountTypeEditable = false;
            IsHoursEditable = false;
            IsNotesReadOnly = true;
            IscallDateEditable = false;
            isOrderActivityEdit = false;
            IsSaveIconVisible = false;
            IsOrderDetailsVisible = false;
            UIModel = new EditActivityUIModel();
            RackPosPopOrderDetailsItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>();
            NonTobaccoGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>();
            TobaccoGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>();
            RtnGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>();
            DifGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>();
            //ActivityTypeList = new ObservableCollection<string>();
            CustomerList = new ObservableCollection<CustomerPageUIModel>();
            AddedAttachementItemSource = new ObservableCollection<AttachmentView>();
            RegisterCommands();
        }
        #endregion

        #region private methods

        private void IsAnyAttachmentAvailable(bool value)
        {
            if (value)
            {
                IsAttachmentListVisible = AddedAttachementItemSource?.Count > 0;
            }
            else IsAttachmentListVisible = value;
        }

        private void RegisterCommands()
        {
            RemoveAttachementCommand = new AsyncRelayCommand<string>(RemoveAttachementCommandHandler);
            PreviewCommand = new AsyncRelayCommand(PreviewCommandHandler);
            OpenPickerCommand = new AsyncRelayCommand<string>(OpenPickerCommandHandler);
            OnNavigatedToCommand = new AsyncRelayCommand<object>(OnNavigatedToCommandHandler);
            SaveCommand = new AsyncRelayCommand(SaveCommandHandler);
            CancelCommand = new AsyncRelayCommand(CancelCommandHandler);
            HoursSelectionCommand = new RelayCommand<string>(HoursSelectionCommandHandler);
            ActivityTypeCommand = new RelayCommand<string>(ActivityTypeCommandHandler);
            AccountNameCommand = new AsyncRelayCommand<CustomerPageUIModel>(AccountNameCommandHandlerAsync);
        }

        private async Task RemoveAttachementCommandHandler(string attachmentPath)
        {
            var result = await AlertHelper.Instance.ShowConfirmationAlert("Alert"
                , "Are you sure you want to delete this attachment ?", "Yes", "No");
            if (result)
                AddedAttachementItemSource?.Remove(AddedAttachementItemSource.FirstOrDefault(x => x.Path == attachmentPath));
        }

        private async Task PreviewCommandHandler()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(currentActivity?.WholesaleInvoiceFilePath) && !AddedAttachementItemSource.Any())
                {
                    await AlertHelper.Instance.ShowConfirmationAlert("Alert", "No attachment found", "OK");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(currentActivity?.WholesaleInvoiceFilePath))
                {
                    if (!currentActivity.WholesaleInvoiceFilePath.ToUpper().StartsWith("HTTP") && !currentActivity.WholesaleInvoiceFilePath.StartsWith("CallActivity"))
                    {
                        await LaunchFile(currentActivity?.WholesaleInvoiceFilePath);
                    }
                    else
                    {
                        // FileType
                        /// download and show the file
                        IsLoading = true;

                        //check if in local exists
                        string fileName = HelperMethods.GetNameFromURL(currentActivity.WholesaleInvoiceFilePath);
                        string path = Path.Combine($"{ApplicationData.Current.LocalFolder.Path}\\{ApplicationConstants.ACTIVITY_BASE_FOLDER}\\{fileName}");
                        if (File.Exists(path))
                        {
                            await LaunchFile(path);
                        }
                        else
                        {
                            path = await InvokeWebService.DownloadDocumentAndFileFromServer(HelperMethods.GetNameFromURL(currentActivity.WholesaleInvoiceFilePath)
                                , Convert.ToString((int)FileType.CallActivity));
                            if (!string.IsNullOrEmpty(path))
                            {
                                await LaunchFile(path);
                            }
                            else
                            {
                                await AlertHelper.Instance.ShowConfirmationAlert("Error", "File not found", "OK");
                            }
                        }
                        IsLoading = false;
                    }
                }
                else if (AddedAttachementItemSource.Any())
                {
                    string path;
                    if (currentActivity.ActivityType.Equals("Wholesale Invoice"))
                        path = await SaveAttachmentAsPdf("demo.pdf");
                    else path = AddedAttachementItemSource.First().Path;
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        await LaunchFile(path);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(EditActivityPageViewModel), nameof(PreviewCommandHandler), ex);
            }
        }

        private async Task LaunchFile(string path)
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            var options = new Windows.System.LauncherOptions();
            options.DisplayApplicationPicker = true;
            await Windows.System.Launcher.LaunchFileAsync(file, options);
        }

        private async Task<string> SaveAttachmentAsPdf(string fileName)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var newFolder = await folder.CreateFolderAsync(ApplicationConstants.ACTIVITY_BASE_FOLDER, CreationCollisionOption.OpenIfExists);
                var path = Path.Combine(newFolder.Path, fileName);
                return EmailAndPrintOrder.PdfCreator.Instance.CreatePdfFromImages(path, AddedAttachementItemSource.Select(x => x.Path).ToList()) ? path : string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(EditActivityPageViewModel), nameof(SaveAttachmentAsPdf), ex.Message);
                return string.Empty;
            }
        }

        private async Task OpenPickerCommandHandler(string arg)
        {
            switch (arg)
            {
                case "camera":
                    await OpenCamera();
                    break;
                case "picker":
                    var photoTypes = new List<string>() { ".jpg", ".jpeg", ".png", ".bmp" };
                    await OpenFilePicker(photoTypes, PickerLocationId.PicturesLibrary);
                    break;
                case "doc":
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
            if (pickedOrClickedFile != null)
            {
                LocalStoredFile = await CopyStorageFileToLocal(pickedOrClickedFile);
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
                    AddedAttachementItemSource?.Clear();
                    AddedAttachementItemSource.Add(new AttachmentView { Path = LocalStoredFile?.Path, IsDelete = true });
                    IsAttachmentListVisible = true;
                }
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
            switch (suggestionLocation)
            {
                case PickerLocationId.PicturesLibrary:
                case PickerLocationId.DocumentsLibrary:
                    if (IsNarrativeWeeklyReport)
                    {
                        var pickerSingleFileClicked = await openPicker.PickSingleFileAsync();
                        if (pickerSingleFileClicked != null)
                        {
                            LocalStoredFile = await CopyStorageFileToLocal(pickerSingleFileClicked);
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
                                AddedAttachementItemSource?.Clear();
                                AddedAttachementItemSource.Add(new AttachmentView { Path = LocalStoredFile?.Path, IsDelete = true });
                                IsAttachmentListVisible = true;
                            }
                        }
                    }
                    else
                    {
                        IReadOnlyList<StorageFile> pickerMultipleFileClicked = await openPicker.PickMultipleFilesAsync();
                        if (pickerMultipleFileClicked?.Count > 0)
                        {
                            IsLoading = true;
                            ulong totalSize = 0;
                            // Define a size limit (e.g., 10 MB)
                            ulong maxSize = 10 * 1024 * 1024;  // 10 MB in bytes
                            ICollection<string> filePaths = new List<string>(pickerMultipleFileClicked.Count);

                            foreach (var file in pickerMultipleFileClicked)
                            {
                                LocalStoredFile = await CopyStorageFileToLocal(file);
                                // Get the basic properties to retrieve the size
                                var basicProperties = await file.GetBasicPropertiesAsync();
                                ulong fileSize = basicProperties.Size;  // Size in bytes

                                // Add to the total size (optional)
                                totalSize += fileSize;
                                filePaths.Add(LocalStoredFile?.Path);
                            }

                            if (totalSize > maxSize)
                            {
                                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "The file size exceeds the 10 MB limit. Please upload a smaller file.", "OK");
                                LocalStoredFile = null;
                            }
                            else
                            {
                                AddedAttachementItemSource?.Clear();
                                foreach (string item in filePaths)
                                { AddedAttachementItemSource.Add(new AttachmentView { Path = item, IsDelete = true }); }
                                IsAttachmentListVisible = true;
                            }
                            IsLoading = false;
                        }
                    }
                    break;
            }
        }

        private async Task<IStorageFile> CopyStorageFileToLocal(IStorageFile file)
        {
            try
            {
                if (file != null)
                {
                    var folder = ApplicationData.Current.LocalFolder;
                    var newFolder = await folder.CreateFolderAsync(ApplicationConstants.ACTIVITY_BASE_FOLDER, CreationCollisionOption.OpenIfExists);
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



        private async Task AccountNameCommandHandlerAsync(CustomerPageUIModel customer)
        {
            if (customer != null)
            {
                if (customer.CustomerName.ToLower().Contains("none"))
                {
                    UIModel.AccountName = string.Empty;
                    UIModel.AccountNo = string.Empty;
                    UIModel.SelectedActivityType = string.Empty;
                    UIModel.City = string.Empty;
                    UIModel.State = string.Empty;
                    await LoadUserActivityAsync();
                    isCustomerSeleted = false;
                }
                else if (loginedInUserCustomerDeviceId.Equals(customer?.DeviceCustomerId))
                {
                    UIModel.AccountNo = customer?.CustomerNumber;
                    UIModel.AccountName = customer?.CustomerName;
                    UIModel.CustomerDeviceId = customer?.DeviceCustomerId;
                    UIModel.City = customer?.City;
                    UIModel.State = customer.State;
                    await LoadUserActivityAsync();
                    isCustomerSeleted = false;
                    UIModel.SelectedActivityType = "Narrative-Weekly Report";
                }
                else
                {
                    await LoadCustomerActivityAsync();
                    UIModel.AccountNo = customer?.CustomerNumber;
                    UIModel.AccountName = customer?.CustomerName;
                    UIModel.CustomerDeviceId = customer?.DeviceCustomerId;
                    UIModel.City = customer?.City;
                    UIModel.State = customer.State;
                    UIModel.SelectedActivityType = customer.AccountType == 1 ? ActivityTypeList.FirstOrDefault(x => x.ToLower().Contains("Distributor Sales Call".ToLower())) : ActivityTypeList.FirstOrDefault(x => x.ToLower().Contains("Retail Sales Call".ToLower()));
                    isCustomerSeleted = true;
                }

                IsNarrativeWeeklyReport = "Narrative-Weekly Report".Equals(UIModel.SelectedActivityType);
                if ("Wholesale Invoice".Equals(UIModel.SelectedActivityType) || "Narrative-Weekly Report".Equals(UIModel.SelectedActivityType))
                {
                    IsPreviewIconVisible = true;
                    IsAttachmentIconVisible = true;
                }
                else
                {
                    IsPreviewIconVisible = false;
                    IsAttachmentIconVisible = false;
                }
            }
        }

        private void ActivityTypeCommandHandler(string type)
        {
            UIModel.SelectedActivityType = type;
            //AddedAttachementItemSource.Clear();
            //DeleteSelectedImages();

            if (!isCustomerSeleted)
            {
                /// user activity
                UIModel.AccountName = loggedInUser.FirstName + " " + loggedInUser.LastName;
                ///UIModel.AccountNo = loggedInUser.UserId.ToString();
                UIModel.CustomerDeviceId = "0-0";
            }

            if ("Wholesale Invoice".Equals(type) || "Narrative-Weekly Report".Equals(type))
            {
                //if (!string.IsNullOrWhiteSpace(currentActivity.WholesaleInvoiceFilePath))
                //{
                //    IsPreviewIconVisible = true;
                //    IsAttachmentIconVisible = false;
                //}
                //else
                //{
                if ("Narrative-Weekly Report".Equals(type)) IsNarrativeWeeklyReport = true;

                IsPreviewIconVisible = true;
                IsAttachmentIconVisible = true;
                //}
            }
            else
            {
                IsNarrativeWeeklyReport = false;
                IsAttachmentIconVisible = false;
                IsPreviewIconVisible = false;
            }
        }

        private void HoursSelectionCommandHandler(string hours)
        {
            UIModel.Hours = hours;
        }

        private async Task SaveCommandHandler()
        {
            await InfoLogger.GetInstance.WriteToLogAsync(SourceName: $"{nameof(EditActivityPageViewModel)}:{nameof(SaveCommandHandler)}"
                         , CustomeMessage: "Clicked SaveButton To UpdateCallActivity");

            if (UIModel.IsInValidConsumerActivationEngagement)
            {
                return;
            }
            else
            {
                currentActivity.ConsumerActivationEngagement = UIModel.ConsumerActivationEngagement;
            }

            if (isOrderActivityEdit)
            {
                currentActivity.Comments = UIModel.Notes;
                currentActivity.IsExported = 0;
                currentActivity.UpdateDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                var editedActivity = await AppReference.QueryService.AddCallActivityToDb(currentActivity);

                if (UIModel.CustomerDeviceId != "0-0")
                {
                    if (AppReference.UpdatedCustomerIds == null)
                        AppReference.UpdatedCustomerIds = new List<string> { UIModel.CustomerDeviceId };
                    else if (!AppReference.UpdatedCustomerIds.Any(x => x == UIModel.CustomerDeviceId))
                    {
                        AppReference.UpdatedCustomerIds.Append(UIModel.CustomerDeviceId);
                    }
                }


                if (editedActivity != null)
                {
                    await AlertHelper.Instance.ShowConfirmationAlert("Alert", "The activity has been successfully updated for this customer", "OK");

                    SetBackNavObject(editedActivity);

                    NavigationService.GoBackInShell();
                }
            }
            else
            {

                if (string.IsNullOrWhiteSpace(UIModel?.SelectedActivityType))
                {
                    await AlertHelper.Instance.ShowConfirmationAlert("Error", "Please select Activity Type", "OK");
                    return;
                }
                else if (!UIModel.CallDate.HasValue)
                {
                    await AlertHelper.Instance.ShowConfirmationAlert("Error", "Please select call date", "OK");
                    return;
                }
                string deviceCustomerId_user = "";
                if (UIModel.CustomerDeviceId.Equals("0-0"))
                {
                    deviceCustomerId_user = await AppReference.QueryService.GetUserCustomerDeviceId(UIModel.CreatedBy);
                    if (string.IsNullOrEmpty(deviceCustomerId_user))
                    {
                        deviceCustomerId_user = "0-0";
                    }
                }
                else
                {
                    deviceCustomerId_user = UIModel.CustomerDeviceId;
                }

                DateTime combinedDateTime = new DateTime(
                UIModel.CallDate.Value.DateTime.Year,
                UIModel.CallDate.Value.DateTime.Month,
                UIModel.CallDate.Value.DateTime.Day,
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond
                );
                currentActivity.CallDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(combinedDateTime);
                currentActivity.Hours = UIModel?.Hours;
                currentActivity.Comments = UIModel?.Notes;
                currentActivity.ActivityType = UIModel?.SelectedActivityType;
                //currentActivity.CustomerID = UIModel.CustomerDeviceId;
                currentActivity.CustomerID = deviceCustomerId_user;
                //currentActivity.ConsumerActivationEngagementNumber = UIModel.ConsumerActivationEngagement;

                currentActivity.IsExported = 0;
                currentActivity.UpdateDate = Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                string[] attachmentActivities = new string[] { "Wholesale Invoice", "Narrative-Weekly Report" };

                if (!attachmentActivities.Contains(UIModel?.SelectedActivityType))
                {
                    currentActivity.WholesaleInvoiceFilePath = null;
                }
                if (IsAttachmentIconVisible)
                {
                    if (attachmentActivities.Contains(UIModel?.SelectedActivityType))
                    {
                        //User select any file
                        if (LocalStoredFile != null)
                        {
                            StringBuilder fileNameBuilder = new StringBuilder();
                            fileNameBuilder.Append(currentActivity.CallActivityDeviceID);
                            fileNameBuilder.Append("_");
                            fileNameBuilder.Append(Regex.Replace(currentActivity.ActivityType, @"\s", ""));
                            fileNameBuilder.Append("_");
                            fileNameBuilder.Append(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                            if ("Wholesale Invoice".Equals(UIModel?.SelectedActivityType) && AddedAttachementItemSource.Any())
                            {
                                fileNameBuilder.Append(".pdf");
                                var path = await SaveAttachmentAsPdf(fileNameBuilder.ToString());
                                currentActivity.WholesaleInvoiceFilePath = path;
                                DeleteSelectedImages();
                            }
                            else if ("Narrative-Weekly Report".Equals(UIModel?.SelectedActivityType) && AddedAttachementItemSource.Any())
                            {
                                fileNameBuilder.Append(Path.GetExtension(AddedAttachementItemSource.First().Path));
                                await LocalStoredFile.RenameAsync(fileNameBuilder.ToString(), NameCollisionOption.ReplaceExisting);
                                currentActivity.WholesaleInvoiceFilePath = LocalStoredFile.Path;
                            }
                            else currentActivity.WholesaleInvoiceFilePath = null;
                        }
                        else
                        {
                            // User don't make any changes in the attachment skip .if remove attachment than null the path
                            if (AddedAttachementItemSource?.Count == 0)
                                currentActivity.WholesaleInvoiceFilePath = null;
                        }
                    }
                }

                if ("Narrative-Weekly Report".Equals(UIModel?.SelectedActivityType))
                {
                    currentActivity.MarketsVisited = UIModel?.MarketsVisited;
                    currentActivity.CallsMadeVsGoal = UIModel?.CallsMadeVsGoal;
                    currentActivity.NewCustomerAcquisitions = UIModel?.NewCustomerAcquisitions;
                    currentActivity.KeyWinsSummary = UIModel?.KeyWinsSummary;
                    currentActivity.ChallengesAndFeedback = UIModel?.ChallengesAndFeedback;
                    currentActivity.NextCyclePlan = UIModel?.NextCyclePlan;
                    currentActivity.NextWeekPlan = UIModel?.NextWeekPlan;
                }
                else
                {
                    currentActivity.MarketsVisited = null;
                    currentActivity.CallsMadeVsGoal = null;
                    currentActivity.NewCustomerAcquisitions = null;
                    currentActivity.KeyWinsSummary = null;
                    currentActivity.ChallengesAndFeedback = null;
                    currentActivity.NextCyclePlan = null;
                    currentActivity.NextWeekPlan = null;
                }


                var editedActivity = await AppReference.QueryService.AddCallActivityToDb(currentActivity);

                if (UIModel.CustomerDeviceId != "0-0")
                {
                    await AppReference.QueryService.UpdateDateFromActivityCustomerMaster(UIModel.CustomerDeviceId
                        , UIModel.SelectedActivityType
                        , currentActivity.CallDate);

                    if (AppReference.UpdatedCustomerIds == null)
                        AppReference.UpdatedCustomerIds = new List<string> { UIModel.CustomerDeviceId };
                    else if (!AppReference.UpdatedCustomerIds.Any(x => x == UIModel.CustomerDeviceId))
                    {
                        AppReference.UpdatedCustomerIds.Append(UIModel.CustomerDeviceId);
                    }
                }

                if (editedActivity != null)
                {
                    if (AddedAttachementItemSource?.Count > 0 && !attachmentActivities.Contains(UIModel?.SelectedActivityType))
                    {
                        await DeleteFileAsync(currentActivity.CallActivityDeviceID);
                    }
                    await AlertHelper.Instance.ShowConfirmationAlert("Update Activity", "The activity has been successfully updated for this customer", "OK");
                    SetBackNavObject(editedActivity);
                    NavigationService.GoBackInShell();
                }
            }
        }

        private async Task CancelCommandHandler()
        {
            var result = await AlertHelper.Instance.ShowConfirmationAlert("Cancel Activity", "Are you sure you want to discard the changes you made to the current activity?", "OK", "Cancel");

            if (result)
            {
                NavigationService.GoBackInShell();
            }
        }

        private void DeleteSelectedImages()
        {
            try
            {
                foreach (var item in AddedAttachementItemSource)
                {
                    if (File.Exists(item.Path))
                    {
                        File.Delete(item.Path);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AddActivityPageViewModel), nameof(DeleteSelectedImages), ex.Message);
            }
        }

        private async Task DeleteFileAsync(string prefixFileName)
        {
            try
            {
                // Get the folder where the file is located
                StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Activity");

                // Retrieve all files in the folder
                var files = await folder.GetFilesAsync();

                // Find the file irrespective of the extension
                foreach (StorageFile file in files)
                {
                    // Check if the file name (without extension) matches
                    //if (System.IO.Path.GetFileNameWithoutExtension(file.Name).Equals(fileNameWithoutExtension, StringComparison.OrdinalIgnoreCase))
                    if (file.Name.StartsWith(prefixFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        // Delete the file
                        await file.DeleteAsync();
                        Debug.WriteLine($"{file.Name} deleted successfully.");
                        return;
                    }
                }
                Debug.WriteLine("File not found.");
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(EditActivityPageViewModel), nameof(DeleteFileAsync), $"Error deleting file: {ex.Message}");
            }
        }

        private void SetBackNavObject(CallActivityList editedActivity)
        {
            if (onNavigationObject is ActivityForAllCustomerUIModel)
            {
                var allCustomerActivity = onNavigationObject as ActivityForAllCustomerUIModel;
                allCustomerActivity.CallDate = editedActivity?.CallDate;
                allCustomerActivity.Hours = editedActivity?.Hours;
                allCustomerActivity.Comments = editedActivity?.Comments;
                allCustomerActivity.ActivityType = editedActivity?.ActivityType;
                allCustomerActivity.CustomerID = editedActivity?.CustomerID;
                allCustomerActivity.CustomerName = UIModel?.AccountName;
                allCustomerActivity.CustomerNumber = UIModel?.AccountNo;
                allCustomerActivity.StateName = UIModel?.State;
                allCustomerActivity.PhysicalAddressCityID = UIModel?.City;
            }
            else if (onNavigationObject is ActivityForIndividualCustomerUIModel)
            {
                var individualCustomerActivity = onNavigationObject as ActivityForIndividualCustomerUIModel;
                individualCustomerActivity.CallDate = editedActivity?.CallDate;
                individualCustomerActivity.Hours = editedActivity?.Hours;
                individualCustomerActivity.Comments = editedActivity?.Comments;
                individualCustomerActivity.ActivityType = editedActivity?.ActivityType;
                individualCustomerActivity.CustomerID = editedActivity?.CustomerID;
                individualCustomerActivity.CustomerName = UIModel?.AccountName;
                individualCustomerActivity.CustomerNumber = UIModel?.AccountNo;
                individualCustomerActivity.PhysicalAddressCityID = UIModel?.City;
                individualCustomerActivity.DisplayTerritoryName = territories?.FirstOrDefault()?.TerritoryNumber;
                individualCustomerActivity.PhysicalAddressStateID = HelperMethods.GetKeyFromIdNameDictionary(stateDict, UIModel?.State).ToString();
            }
            ActivitiesPageViewModel.EditedActivity = onNavigationObject;
        }

        private async Task OnNavigatedToCommandHandler(object navObject)
        {
            try
            {
                IsLoading = true;
                if (navObject is ActivityForAllCustomerUIModel)
                {
                    var activityByAllCustomerModel = navObject as ActivityForAllCustomerUIModel;
                    onNavigationObject = activityByAllCustomerModel;
                    currentActivity = await AppReference.QueryService.GetIndidualActivityById(activityByAllCustomerModel?.CallActivityDeviceID);
                    var user = await AppReference.QueryService.GetUserFromUserId(activityByAllCustomerModel.UserID);
                    if (user != null)
                    {
                        UIModel.UserId = user.UserId;
                        UIModel.CreatedBy = user?.FirstName + " " + user.LastName;
                    }
                    DateTime.TryParse(currentActivity.CallDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime callDate);
                    UIModel.CallDate = callDate;
                    UIModel.SelectedActivityType = activityByAllCustomerModel?.ActivityType;
                    UIModel.Hours = currentActivity?.Hours;
                    UIModel.Notes = currentActivity?.Comments;                    
                    //UIModel.Team = territories?.FirstOrDefault()?.TerritoryName;
                    UIModel.Team = currentActivity.TerritoryName;
                    UIModel.AccountNo = activityByAllCustomerModel?.CustomerNumber;
                    UIModel.AccountName = activityByAllCustomerModel?.CustomerName;
                    UIModel.City = activityByAllCustomerModel?.PhysicalAddressCityID;
                    UIModel.State = activityByAllCustomerModel?.StateName;
                    UIModel.DateCreated = DateTime.TryParse(currentActivity?.CreatedDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime createdDate) ? createdDate.ToString("MMM dd,yyyy", new CultureInfo("en-US")) : string.Empty;
                    UIModel.DateModified = DateTime.TryParse(currentActivity?.UpdateDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime updatedDate) ? updatedDate.ToString("MMM dd,yyyy", new CultureInfo("en-US")) : string.Empty;
                    //if (currentActivity?.ConsumerActivationEngagementNumber != null)
                    //UIModel.ConsumerActivationEngagement = currentActivity.ConsumerActivationEngagementNumber;
                    if (currentActivity?.ConsumerActivationEngagement != null)
                        UIModel.ConsumerActivationEngagement = currentActivity.ConsumerActivationEngagement;

                    //if ((DateTime.Compare(UIModel.CallDate.Value.Date, DateTime.Now.Date) >= 0) || (activityByAllCustomerModel?.ActivityType.Equals("Narrative-Weekly Report") ?? true))
                    //{
                    //    UIModel.CustomerDeviceId = activityByAllCustomerModel?.CustomerID;
                    //    await SetPropertiesWhichCanBeEdited(activityByAllCustomerModel?.OrderID, activityByAllCustomerModel?.CustomerNumber, false);
                    //}
                    if ((DateTime.Compare(UIModel.CallDate.Value.Date, DateTime.Now.Date) >= 0))
                    {
                        UIModel.CustomerDeviceId = activityByAllCustomerModel?.CustomerID;
                        await SetPropertiesWhichCanBeEdited(activityByAllCustomerModel?.OrderID, activityByAllCustomerModel?.CustomerNumber, false);
                    }
                    if ("Wholesale Invoice".Equals(currentActivity?.ActivityType) || "Narrative-Weekly Report".Equals(currentActivity?.ActivityType))
                    {
                        if (IsSaveIconVisible)
                        {
                            if (!string.IsNullOrWhiteSpace(currentActivity?.WholesaleInvoiceFilePath))
                                AddedAttachementItemSource.Add(new AttachmentView { Path = currentActivity.WholesaleInvoiceFilePath, IsDelete = true });

                            IsPreviewIconVisible = true;
                            IsAttachmentIconVisible = true;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(currentActivity?.WholesaleInvoiceFilePath))
                            {
                                AddedAttachementItemSource.Add(new AttachmentView { Path = currentActivity.WholesaleInvoiceFilePath, IsDelete = false });
                                IsPreviewIconVisible = true;
                            }
                        }
                    }

                    if ("Narrative-Weekly Report".Equals(currentActivity?.ActivityType))
                    {
                        IsNarrativeWeeklyReport = true;
                        UIModel.MarketsVisited = currentActivity.MarketsVisited;
                        UIModel.CallsMadeVsGoal = currentActivity.CallsMadeVsGoal;
                        UIModel.NewCustomerAcquisitions = currentActivity.NewCustomerAcquisitions;
                        UIModel.KeyWinsSummary = currentActivity.KeyWinsSummary;
                        UIModel.ChallengesAndFeedback = currentActivity.ChallengesAndFeedback;
                        UIModel.NextCyclePlan = currentActivity.NextCyclePlan;
                        UIModel.NextWeekPlan = currentActivity.NextWeekPlan;
                    }
                    else { IsNarrativeWeeklyReport = false; }


                    if ((DateTime.Compare(UIModel.CallDate.Value.Date, DateTime.Now.Date) >= 0) || (activityByAllCustomerModel?.ActivityType.Equals("Narrative-Weekly Report") ?? true))
                    {
                        UIModel.CustomerDeviceId = activityByAllCustomerModel?.CustomerID;
                        await SetPropertiesWhichCanBeEdited(activityByAllCustomerModel?.OrderID, activityByAllCustomerModel?.CustomerNumber, false);
                    }
                    NonTobaccoGridItemSource.Clear();
                    TobaccoGridItemSource.Clear();
                    RtnGridItemSource.Clear();
                    DifGridItemSource.Clear();
                    OrderMaster order = null;
                    List<OrderHistoryDetailsGridUIModel> orderDetailsList = null;
                    if (!string.IsNullOrWhiteSpace(activityByAllCustomerModel.OrderID))
                    {
                        orderDetailsList = await AppReference.QueryService.GetOrderDetailsByDeviceOrderId(activityByAllCustomerModel.OrderID);

                        order = await AppReference.QueryService.GetOrderFromOrderMasterFromDeviceOrderId(activityByAllCustomerModel.OrderID);

                        if (order != null)
                        {
                            decimal decimalTotal = 0;

                            if (!string.IsNullOrEmpty(order.GrandTotal))
                            {
                                decimalTotal = Convert.ToDecimal(order.GrandTotal);
                            }

                            UIModel.OrderNo = order?.DeviceOrderID;
                            UIModel.OrderType = HelperMethods.GetSalesTypeString(order?.SalesType);
                            IsOrderDetailsVisible = true;
                            UIModel.Amount = string.IsNullOrEmpty(order?.GrandTotal) ? "$0.00" : string.Format("${0}", decimalTotal.ToString("0.00"));
                        }
                    }

                    if (("5".Equals(order?.SalesType) || "10".Equals(order?.SalesType)) && orderDetailsList != null)
                    {
                        LoadRackPosPopGrid = true;
                        RackPosPopOrderDetailsItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>(orderDetailsList);
                    }
                    else if (orderDetailsList != null)
                    {
                        decimal totalTobacco = 0;
                        decimal totalNonTobacco = 0;
                        decimal rtnTotal = 0;
                        decimal difTotal = 0;
                        decimal grandTotal = 0;

                        if (!"8".Equals(order?.SalesType))
                        {
                            // changed the if condition to read the istobacco flag from product master instead of order detail
                            var nonTobaccoLists = orderDetailsList.Where(e => e.IsTobbaco == 0);
                            var tobaccoLists = orderDetailsList.Where(e => e.IsTobbaco != 0);
                            if (nonTobaccoLists.Any())
                            {
                                NonTobaccoGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>(
                                    nonTobaccoLists.ToList()
                                    );
                                totalNonTobacco = nonTobaccoLists.Sum(e => (e.ProductUnitPrice * e.ProductQty));
                                LoadNonTobaccoGrid = true;
                                TotalNonTobacco = string.Format("${0}", totalNonTobacco.ToString("0.00"));
                            }
                            if (tobaccoLists.Any())
                            {
                                TobaccoGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>(
                                    tobaccoLists.ToList()
                                    );
                                totalTobacco = tobaccoLists.Sum(e => (e.ProductUnitPrice * e.ProductQty));
                                LoadTobaccoGrid = true;
                                TotalTobacco = string.Format("${0}", totalTobacco.ToString("0.00"));
                            }
                            grandTotal = totalNonTobacco + totalTobacco;
                        }
                        else
                        {
                            var rtnCreditRequests = orderDetailsList.Where(e => e.CreditRequestType.Contains("RTN"));
                            var difCreditRequests = orderDetailsList.Where(e => e.CreditRequestType.Contains("DIF"));

                            if (rtnCreditRequests.Any())
                            {
                                RtnGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>(
                                    rtnCreditRequests.ToList()
                                    );
                                rtnTotal = rtnCreditRequests.Sum(e => (e.ProductUnitPrice * e.ProductQty));
                                LoadRtnGrid = true;
                                TotalRtnTobacco = string.Format("${0}", rtnTotal.ToString("0.00"));
                            }

                            if (difCreditRequests.Any())
                            {
                                DifGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>(
                                    difCreditRequests.ToList()
                                    );
                                difTotal = difCreditRequests.Sum(e => (e.ProductUnitPrice * e.ProductQty));
                                LoadDifGrid = true;
                                TotalDifTobacco = string.Format("${0}", difTotal.ToString("0.00"));
                            }
                            grandTotal = rtnTotal + difTotal;
                        }

                        LoadTotalStackPanel = true;
                        CombineTotal = string.Format("${0}", grandTotal.ToString("0.00"));
                    }
                }
                else if (navObject is ActivityForIndividualCustomerUIModel)
                {
                    stateDict = await AppReference.QueryService.GetStateDict();

                    var activityByIndividualCustomerModel = navObject as ActivityForIndividualCustomerUIModel;

                    onNavigationObject = activityByIndividualCustomerModel;
                    currentActivity = await AppReference.QueryService.GetIndidualActivityById(activityByIndividualCustomerModel?.CallActivityDeviceID);

                    var user = await AppReference.QueryService.GetUserFromUserId(activityByIndividualCustomerModel.UserID);
                    ///var territory = await AppReference.QueryService.GetTerritoryFromIds(user.TerritoryID);

                    if (territories != null && territories.Count > 0)
                    {
                        territories.Clear();
                    }

                    territories = new List<TerritoryMaster>();
                    if(user!= null)
                    {
                        territories = await AppReference.QueryService.GetTerritoryFromIds(user.TerritoryID);
                        UIModel.CreatedBy = user?.FirstName + " " + user.LastName;
                    }

                    IsAccountNameEditable = false;
                    DateTime.TryParse(currentActivity.CallDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime callDate);

                    UIModel.CallDate = callDate;
                    UIModel.SelectedActivityType = activityByIndividualCustomerModel?.ActivityType;
                    UIModel.Hours = currentActivity?.Hours;
                    UIModel.Notes = currentActivity?.Comments;                   
                    UIModel.Team = currentActivity.TerritoryName;
                    UIModel.AccountNo = activityByIndividualCustomerModel?.CustomerNumber;
                    UIModel.AccountName = activityByIndividualCustomerModel?.CustomerName;
                    UIModel.City = activityByIndividualCustomerModel?.PhysicalAddressCityID;
                    UIModel.State = HelperMethods.GetValueFromIdNameDictionary(stateDict, Convert.ToInt32(activityByIndividualCustomerModel?.PhysicalAddressStateID));
                    UIModel.DateCreated = DateTime.TryParse(currentActivity?.CreatedDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime createdDate) ? createdDate.ToString("MMM dd,yyyy", new CultureInfo("en-US")) : string.Empty;
                    UIModel.DateModified = DateTime.TryParse(currentActivity?.UpdateDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime updatedDate) ? updatedDate.ToString("MMM dd,yyyy", new CultureInfo("en-US")) : string.Empty;

                    //if (currentActivity?.ConsumerActivationEngagementNumber != null)
                    //    UIModel.ConsumerActivationEngagement = currentActivity.ConsumerActivationEngagementNumber;
                    if (currentActivity?.ConsumerActivationEngagement != null)
                        UIModel.ConsumerActivationEngagement = currentActivity.ConsumerActivationEngagement;

                    if ((DateTime.Compare(UIModel.CallDate.Value.Date, DateTime.Now.Date) >= 0) || (activityByIndividualCustomerModel?.ActivityType.Equals("Narrative-Weekly Report") ?? true))
                    {
                        UIModel.CustomerDeviceId = activityByIndividualCustomerModel?.CustomerID;

                        await SetPropertiesWhichCanBeEdited(activityByIndividualCustomerModel?.OrderID, activityByIndividualCustomerModel?.CustomerNumber, true);
                    }


                    if ("Wholesale Invoice".Equals(currentActivity?.ActivityType) || "Narrative-Weekly Report".Equals(currentActivity?.ActivityType))
                    {
                        if (IsSaveIconVisible)
                        {
                            if (!string.IsNullOrWhiteSpace(currentActivity?.WholesaleInvoiceFilePath))
                            {
                                AddedAttachementItemSource.Add(new AttachmentView { Path = currentActivity.WholesaleInvoiceFilePath, IsDelete = true });
                            }
                            IsPreviewIconVisible = true;
                            IsAttachmentIconVisible = true;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(currentActivity?.WholesaleInvoiceFilePath))
                            {
                                AddedAttachementItemSource.Add(new AttachmentView { Path = currentActivity.WholesaleInvoiceFilePath, IsDelete = false });
                                IsPreviewIconVisible = true;
                            }

                        }
                    }

                    if ("Narrative-Weekly Report".Equals(currentActivity?.ActivityType))
                    {
                        IsNarrativeWeeklyReport = true;
                        UIModel.MarketsVisited = currentActivity.MarketsVisited;
                        UIModel.CallsMadeVsGoal = currentActivity.CallsMadeVsGoal;
                        UIModel.NewCustomerAcquisitions = currentActivity.NewCustomerAcquisitions;
                        UIModel.KeyWinsSummary = currentActivity.KeyWinsSummary;
                        UIModel.ChallengesAndFeedback = currentActivity.ChallengesAndFeedback;
                        UIModel.NextCyclePlan = currentActivity.NextCyclePlan;
                        UIModel.NextWeekPlan = currentActivity.NextWeekPlan;
                    }
                    else { IsNarrativeWeeklyReport = false; }


                    NonTobaccoGridItemSource.Clear();
                    TobaccoGridItemSource.Clear();
                    RtnGridItemSource.Clear();
                    DifGridItemSource.Clear();
                    OrderMaster order = null;
                    List<OrderHistoryDetailsGridUIModel> orderDetailsList = null;
                    if (!string.IsNullOrWhiteSpace(activityByIndividualCustomerModel.OrderID))
                    {
                        orderDetailsList = await AppReference.QueryService.GetOrderDetailsByDeviceOrderId(activityByIndividualCustomerModel.OrderID);

                        order = await AppReference.QueryService.GetOrderFromOrderMasterFromDeviceOrderId(activityByIndividualCustomerModel.OrderID);

                        if (order != null)
                        {
                            decimal decimalTotal = 0;

                            if (!string.IsNullOrEmpty(order.GrandTotal))
                            {
                                decimalTotal = Convert.ToDecimal(order.GrandTotal);
                            }

                            UIModel.OrderNo = order?.DeviceOrderID;
                            UIModel.OrderType = HelperMethods.GetSalesTypeString(order?.SalesType);
                            IsOrderDetailsVisible = true;
                            UIModel.Amount = string.IsNullOrEmpty(order?.GrandTotal) ? "$0.00" : string.Format("${0}", decimalTotal.ToString("0.00"));
                        }
                    }
                    if (("5".Equals(order?.SalesType) || "10".Equals(order?.SalesType)) && orderDetailsList != null)
                    {
                        LoadRackPosPopGrid = true;
                        RackPosPopOrderDetailsItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>(orderDetailsList);
                    }
                    else if (orderDetailsList != null)
                    {
                        decimal totalTobacco = 0;
                        decimal totalNonTobacco = 0;
                        decimal rtnTotal = 0;
                        decimal difTotal = 0;
                        decimal grandTotal = 0;

                        if (!"8".Equals(order?.SalesType))
                        {
                            // changed the if condition to read the istobacco flag from product master instead of order detail
                            var nonTobaccoLists = orderDetailsList.Where(e => e.IsTobbaco == 0);
                            var tobaccoLists = orderDetailsList.Where(e => e.IsTobbaco != 0);
                            if (nonTobaccoLists.Any())
                            {
                                NonTobaccoGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>(
                                    nonTobaccoLists.ToList()
                                    );
                                totalNonTobacco = nonTobaccoLists.Sum(e => (e.ProductUnitPrice * e.ProductQty));
                                LoadNonTobaccoGrid = true;
                                TotalNonTobacco = string.Format("${0}", totalNonTobacco.ToString("0.00"));
                            }
                            if (tobaccoLists.Any())
                            {
                                TobaccoGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>(
                                    tobaccoLists.ToList()
                                    );
                                totalTobacco = tobaccoLists.Sum(e => (e.ProductUnitPrice * e.ProductQty));
                                LoadTobaccoGrid = true;
                                TotalTobacco = string.Format("${0}", totalTobacco.ToString("0.00"));
                            }
                            grandTotal = totalNonTobacco + totalTobacco;
                        }
                        else
                        {
                            var rtnCreditRequests = orderDetailsList.Where(e => e.CreditRequestType.Contains("RTN"));
                            var difCreditRequests = orderDetailsList.Where(e => e.CreditRequestType.Contains("DIF"));

                            if (rtnCreditRequests.Any())
                            {
                                RtnGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>(
                                    rtnCreditRequests.ToList()
                                    );
                                rtnTotal = rtnCreditRequests.Sum(e => (e.ProductUnitPrice * e.ProductQty));
                                LoadRtnGrid = true;
                                TotalRtnTobacco = string.Format("${0}", rtnTotal.ToString("0.00"));
                            }

                            if (difCreditRequests.Any())
                            {
                                DifGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>(
                                    difCreditRequests.ToList()
                                    );
                                difTotal = difCreditRequests.Sum(e => (e.ProductUnitPrice * e.ProductQty));
                                LoadDifGrid = true;
                                TotalDifTobacco = string.Format("${0}", difTotal.ToString("0.00"));
                            }
                            grandTotal = rtnTotal + difTotal;
                        }

                        LoadTotalStackPanel = true;
                        CombineTotal = string.Format("${0}", grandTotal.ToString("0.00"));
                    }

                }
                if (loggedInUser == null)
                {
                    loggedInUser = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
                }
                loginedInUserCustomerDeviceId = await AppReference.QueryService.GetUserCustomerDeviceId(loggedInUser.FirstName + " " + loggedInUser.LastName);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(EditActivityPageViewModel), "OnNavigatedToCommandHandler", ex.StackTrace);
            }

            IsLoading = false;
        }

        private async Task SetPropertiesWhichCanBeEdited(string orderID, string customerNumber, bool isIndividualCustomer)
        {
            IsSaveIconVisible = true;
            IsNotesReadOnly = false;

            if (!string.IsNullOrEmpty(orderID))
            {
                isOrderActivityEdit = true;
            }
            else
            {
                isOrderActivityEdit = false;
                IscallDateEditable = true;
                IsAccountTypeEditable = true;
                IsHoursEditable = true;

                if (isIndividualCustomer)
                {
                    IsAccountNameEditable = false;
                }
                else
                {
                    IsAccountNameEditable = true;
                }

                var _tempList = await AppReference.QueryService.GetCustomerListForActivity();

                var stateDict = await AppReference.QueryService.GetStateDict();

                foreach (var item in _tempList)
                {
                    item.StateData = HelperMethods.GetValueFromIdNameDictionary(stateDict, item.PhysicalAddressStateID);
                }

                if (IsAccountNameEditable)
                {
                    CustomerList = new ObservableCollection<CustomerPageUIModel>(_tempList.OrderBy(x => x.CustomerName).Select(x => x.CopyToUIModel()));

                    CustomerList.Insert(0, new CustomerPageUIModel() { CustomerName = "None", CustomerNumber = "-" });
                }

                //if (!string.IsNullOrWhiteSpace(customerNumber))
                if (UIModel.AccountName.Equals(UIModel.CreatedBy))
                {
                    await LoadUserActivityAsync();
                    isCustomerSeleted = false;
                }
                else
                {
                    await LoadCustomerActivityAsync();
                    isCustomerSeleted = true;
                }

                loggedInUser = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
            }
        }

        private async Task LoadUserActivityAsync() => ActivityTypeList = new ObservableCollection<string>(await AppReference.QueryService.GetUserActivityTypeAsync());

        private async Task LoadCustomerActivityAsync() => ActivityTypeList = new ObservableCollection<string>(await AppReference.QueryService.GetCustomerActivityTypeAsync());

        //private void LoadCustomerActivity()
        //{
        //    ActivityTypeList = new ObservableCollection<string>();
        //    ActivityTypeList.Clear();
        //    ActivityTypeList.Add("Phone Call");
        //    ActivityTypeList.Add("Retail Sales Call");
        //    ActivityTypeList.Add("Distributor Sales Call");
        //    ActivityTypeList.Add("Chain Sales Call");
        //    ActivityTypeList.Add("Wholesale Invoice");
        //    ActivityTypeList.Add("Other");
        //    ActivityTypeList.Add("Merchandising");
        //    ActivityTypeList.Add("Training");
        //}

        #endregion
    }

    public class AttachmentView
    {
        public string Path { get; set; }
        public bool IsDelete { get; set; }
    }
}