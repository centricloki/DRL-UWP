using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

using DevExpress.Mvvm.Native;

using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;

using Microsoft.Toolkit.Mvvm.Input;

using MimeKit;

using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace DRLMobile.Uwp.ViewModel
{
    public class AddActivityPageViewModel : BaseModel
    {
        #region Fields
        public bool isCustomerAddActivity { get; private set; }
        private readonly App AppReference = (App)Application.Current;
        private UserMaster loggedInUser;
        private List<TerritoryMaster> TerritoryList;
        private string selectedDeviceCustomerId;
        public string customerCity { get; set; }
        public string customerState { get; set; }
        public Dictionary<int, string> stateDict;
        private bool isUserActivity;
        #endregion

        #region Properties
        ///public ObservableCollection<CustomerMaster> CustomerList { get; set; }

        private ObservableCollection<CustomerPageUIModel> _customerList;
        public ObservableCollection<CustomerPageUIModel> CustomerList
        {
            get { return _customerList; }
            set { SetProperty(ref _customerList, value); }
        }


        private ObservableCollection<string> _activityTypeList;
        public ObservableCollection<string> ActivityTypeList
        {
            get { return _activityTypeList; }
            set { SetProperty(ref _activityTypeList, value); }
        }


        private ObservableCollection<string> _addedAttachementItemSource;
        public ObservableCollection<string> AddedAttachementItemSource
        {
            get { return _addedAttachementItemSource; }
            set { SetProperty(ref _addedAttachementItemSource, value); }
        }

        private string _selectedActivityType;
        public string SelectedActivityType
        {
            get { return _selectedActivityType; }
            set { SetProperty(ref _selectedActivityType, value); }
        }

        private string _selectedCustomerName;
        public string SelectedCustomerName
        {
            get { return _selectedCustomerName; }
            set { SetProperty(ref _selectedCustomerName, value); }
        }


        private string _selectedCustomerNo;
        public string SelectedCustomerNo
        {
            get { return _selectedCustomerNo; }
            set { SetProperty(ref _selectedCustomerNo, value); }
        }

        private DateTimeOffset? _selectedCallDate;
        public DateTimeOffset? SelectedCallDate
        {
            get { return _selectedCallDate; }
            set { SetProperty(ref _selectedCallDate, value); }
        }

        private string _selectedHours;
        public string SelectedHours
        {
            get { return _selectedHours; }
            set { SetProperty(ref _selectedHours, value); }
        }

        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set { SetProperty(ref _notes, value); }
        }

        private string _consumerActivationEngagement;
        public string ConsumerActivationEngagement
        {
            get { return _consumerActivationEngagement; }
            set { VerifyActivationEngagement(value); SetProperty(ref _consumerActivationEngagement, value); }
        }
        private bool _isInValidConsumerActivationEngagement;
        public bool IsInValidConsumerActivationEngagement
        {
            get { return _isInValidConsumerActivationEngagement; }
            set { SetProperty(ref _isInValidConsumerActivationEngagement, value); }
        }
        private string _errorConsumerActivationEngagement;
        public string ErrorConsumerActivationEngagement
        {
            get { return _errorConsumerActivationEngagement; }
            set { SetProperty(ref _errorConsumerActivationEngagement, value); }
        }

        private bool _isAttachmentIconVisible;
        public bool IsAttachmentIconVisible
        {
            get { return _isAttachmentIconVisible; }
            set { SetProperty(ref _isAttachmentIconVisible, value); }
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

        private string _marketsvisited;
        public string MarketsVisited
        {
            get { return _marketsvisited; }
            set { SetProperty(ref _marketsvisited, value); }
        }

        private string _callsmadevsgoal;
        public string CallsMadeVsGoal
        {
            get { return _callsmadevsgoal; }
            set { SetProperty(ref _callsmadevsgoal, value); }
        }
        private string _newcustomeracquisitions;
        public string NewCustomerAcquisitions
        {
            get { return _newcustomeracquisitions; }
            set { SetProperty(ref _newcustomeracquisitions, value); }
        }
        private string _keywinssummary;
        public string KeyWinsSummary
        {
            get { return _keywinssummary; }
            set { SetProperty(ref _keywinssummary, value); }
        }
        private string _challengesandfeedback;
        public string ChallengesAndFeedback
        {
            get { return _challengesandfeedback; }
            set { SetProperty(ref _challengesandfeedback, value); }
        }
        private string _nextcycleplan;
        public string NextCyclePlan
        {
            get { return _nextcycleplan; }
            set { SetProperty(ref _nextcycleplan, value); }
        }
        private string _nextweekplan;
        public string NextWeekPlan
        {
            get { return _nextweekplan; }
            set { SetProperty(ref _nextweekplan, value); }
        }

        private string loginedInUserCustomerDeviceId;
        #endregion

        #region Commands
        public ICommand OnNavigatedToCommand { get; private set; }
        public ICommand CustomerSelectionCommand { get; private set; }
        public ICommand ActivitySelectionCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand OpenPickerCommand { get; private set; }
        public ICommand RemoveAttachementCommand { get; private set; }
        public ICommand PreviewCommand { get; private set; }
        #endregion

        #region Constructor
        public AddActivityPageViewModel()
        {
            RegisterCommand();
            CustomerList = new ObservableCollection<CustomerPageUIModel>();
            //ActivityTypeList = new ObservableCollection<string>();
            TerritoryList = new List<TerritoryMaster>();
            IsAttachmentIconVisible = false;
            AddedAttachementItemSource = new ObservableCollection<string>();
        }
        #endregion

        #region Private Methods
        private void VerifyActivationEngagement(string inputValue)
        {
            IsInValidConsumerActivationEngagement = true;
            ErrorConsumerActivationEngagement = "Please enter a valid number of up to 6 digits.";

            if (!string.IsNullOrWhiteSpace(inputValue))
            {
                // Consumer Activation Engagement Max-Length should not more than 6 digit long
                if (int.TryParse(inputValue, out int result))
                {
                    if (result <= 999999)
                    {
                        IsInValidConsumerActivationEngagement = false;
                        ErrorConsumerActivationEngagement = string.Empty;
                    }
                }
            }
            else
            {
                IsInValidConsumerActivationEngagement = false;
                ErrorConsumerActivationEngagement = string.Empty;
            }
        }
        private void RegisterCommand()
        {
            PreviewCommand = new AsyncRelayCommand(PreviewCommandHandler);
            RemoveAttachementCommand = new AsyncRelayCommand<string>(RemoveAttachementCommandHandler);
            OpenPickerCommand = new AsyncRelayCommand<string>(OpenPickerCommandHandler);
            SaveCommand = new AsyncRelayCommand(SaveCommandHandler);
            CancelCommand = new AsyncRelayCommand(CancelCommandHandler);
            ActivitySelectionCommand = new RelayCommand<string>(ActivitySelectionCommandHandler);
            OnNavigatedToCommand = new AsyncRelayCommand<object>(OnNavigatedToCommandHandler);
            CustomerSelectionCommand = new AsyncRelayCommand<CustomerPageUIModel>(CustomerSelectionCommandHandlerAsync);
        }

        private async Task PreviewCommandHandler()
        {
            try
            {
                if (AddedAttachementItemSource.Any())
                {
                    IsLoading = true;
                    string path;
                    if (SelectedActivityType.Equals("Wholesale Invoice"))
                        path = await SaveAttachmentAsPdf("demo.pdf");
                    else path = AddedAttachementItemSource.First();

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        await LaunchFile(path);
                    }
                    IsLoading = false;
                }
                else
                {
                    await AlertHelper.Instance.ShowConfirmationAlert("Alert", "No attachment found", "OK");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AddActivityPageViewModel), nameof(PreviewCommandHandler), ex.StackTrace);
                IsLoading = false;
            }
        }

        private async Task LaunchFile(string path)
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            var options = new Windows.System.LauncherOptions();
            options.DisplayApplicationPicker = true;
            await Windows.System.Launcher.LaunchFileAsync(file, options);
        }

        private async Task RemoveAttachementCommandHandler(string attachmentPath)
        {
            var result = await AlertHelper.Instance.ShowConfirmationAlert("Alert"
                , "Are you sure you want to delete this attachment ?", "Yes", "No");
            if (result)
                AddedAttachementItemSource?.Remove(attachmentPath);
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
                                AddedAttachementItemSource.Add(LocalStoredFile?.Path);
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
                                { AddedAttachementItemSource.Add(item); }
                            }
                            IsLoading = false;
                        }
                    }
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
                    AddedAttachementItemSource.Add(LocalStoredFile?.Path);
                }
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
        private async Task SaveCommandHandler()
        {
            try
            {
                if (!IsInValidConsumerActivationEngagement)
                {
                    if (!string.IsNullOrWhiteSpace(SelectedActivityType) && SelectedCallDate.HasValue)
                    {
                        IsLoading = true;
                        string nowDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                        //string selectedCallDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(SelectedCallDate.Value.DateTime);
                        DateTime combinedDateTime = new DateTime(
                        SelectedCallDate.Value.DateTime.Year,
                        SelectedCallDate.Value.DateTime.Month,
                        SelectedCallDate.Value.DateTime.Day,
                        DateTime.Now.Hour,
                        DateTime.Now.Minute,
                        DateTime.Now.Second,
                        DateTime.Now.Millisecond
                        );
                        string selectedCallDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(combinedDateTime);
                        Guid deviceCallActivityGuid = Guid.NewGuid();
                        string deviceCustomerId_user = "";
                        if (selectedDeviceCustomerId.Equals("0-0"))
                        {
                            deviceCustomerId_user = await AppReference.QueryService.GetUserCustomerDeviceId(loggedInUser.FirstName + " " + loggedInUser.LastName);
                            if (string.IsNullOrEmpty(deviceCustomerId_user))
                            {
                                deviceCustomerId_user = "0-0";
                            }
                        }
                        else
                        {
                            deviceCustomerId_user = selectedDeviceCustomerId.ToString();
                        }

                        var activity = new CallActivityList()
                        {
                            OrderID = null,
                            ActivityType = SelectedActivityType,
                            Comments = Notes,
                            CallDate = selectedCallDate,
                            Hours = SelectedHours,
                            //CustomerID = selectedDeviceCustomerId.ToString(), // Changed "0-0" to user Self customer
                            CustomerID = deviceCustomerId_user,
                            UserID = loggedInUser.UserId,
                            CreatedDate = nowDate,
                            IsExported = 0,
                            isDeleted = 0,
                            TerritoryID = TerritoryList.FirstOrDefault().TerritoryID,
                            TerritoryName = TerritoryList.FirstOrDefault()?.TerritoryName,
                            CallActivityDeviceID = deviceCallActivityGuid.ToString(),
                            CallActivityID = HelperMethods.GenerateRandomNumberForGivenRange(10000, 99999),
                            ConsumerActivationEngagement = this.ConsumerActivationEngagement
                        };
                        string[] attachmentActivities = new string[] { "Wholesale Invoice", "Narrative-Weekly Report" };
                        if (attachmentActivities.Contains(SelectedActivityType))
                        {
                            if (LocalStoredFile != null)
                            {
                                if ("Wholesale Invoice".Equals(activity.ActivityType) || "Narrative-Weekly Report".Equals(activity.ActivityType))
                                {
                                    StringBuilder fileNameBuilder = new StringBuilder();
                                    fileNameBuilder.Append(deviceCallActivityGuid.ToString());
                                    fileNameBuilder.Append("_");
                                    fileNameBuilder.Append(Regex.Replace(activity.ActivityType, @"\s", ""));
                                    fileNameBuilder.Append("_");
                                    fileNameBuilder.Append(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                                    if ("Wholesale Invoice".Equals(activity.ActivityType) && AddedAttachementItemSource.Any())
                                    {
                                        fileNameBuilder.Append(".pdf");
                                        var path = await SaveAttachmentAsPdf(fileNameBuilder.ToString());
                                        activity.WholesaleInvoiceFilePath = path;
                                        DeleteSelectedImages();
                                    }
                                    else if ("Narrative-Weekly Report".Equals(activity.ActivityType) && AddedAttachementItemSource.Any())
                                    {
                                        fileNameBuilder.Append(Path.GetExtension(AddedAttachementItemSource.First()));
                                        await LocalStoredFile.RenameAsync(fileNameBuilder.ToString(), NameCollisionOption.ReplaceExisting);
                                        activity.WholesaleInvoiceFilePath = LocalStoredFile.Path;
                                    }
                                    else
                                        activity.WholesaleInvoiceFilePath = null;
                                }
                            }
                            else
                                activity.WholesaleInvoiceFilePath = null;
                        }

                        if ("Narrative-Weekly Report".Equals(activity.ActivityType))
                        {
                            activity.MarketsVisited = MarketsVisited;
                            activity.CallsMadeVsGoal = CallsMadeVsGoal;
                            activity.NewCustomerAcquisitions = NewCustomerAcquisitions;
                            activity.KeyWinsSummary = KeyWinsSummary;
                            activity.ChallengesAndFeedback = ChallengesAndFeedback;
                            activity.NextCyclePlan = NextCyclePlan;
                            activity.NextWeekPlan = NextWeekPlan;
                        }
                        else
                        {
                            activity.MarketsVisited = null;
                            activity.CallsMadeVsGoal = null;
                            activity.NewCustomerAcquisitions = null;
                            activity.KeyWinsSummary = null;
                            activity.ChallengesAndFeedback = null;
                            activity.NextCyclePlan = null;
                            activity.NextWeekPlan = null;

                        }


                            await InfoLogger.GetInstance.WriteToLogAsync(SourceName: $"{nameof(AddActivityPageViewModel)}:{nameof(SaveCommandHandler)}"
                                , CustomeMessage: "Clicked SaveButton To AddCallActivity");
                        activity = await AppReference.QueryService.AddCallActivityToDb(activity);

                        if (!selectedDeviceCustomerId.Equals("0-0"))
                        {
                            await AppReference.QueryService.UpdateDateFromActivityCustomerMaster(selectedDeviceCustomerId
                                , SelectedActivityType
                                , selectedCallDate);

                            if (AppReference.UpdatedCustomerIds == null)
                                AppReference.UpdatedCustomerIds = new List<string> { selectedDeviceCustomerId };
                            else if (!AppReference.UpdatedCustomerIds.Any(x => x == selectedDeviceCustomerId))
                            {
                                AppReference.UpdatedCustomerIds.Append(selectedDeviceCustomerId);
                            }
                        }

                        if (activity != null)
                        {
                            if (AddedAttachementItemSource?.Count > 0 && !attachmentActivities.Contains(SelectedActivityType)) DeleteSelectedImages();

                            activity.CallDate = (SelectedCallDate.HasValue ? selectedCallDate : nowDate);

                            IsLoading = false;

                            await AlertHelper.Instance.ShowConfirmationAlert("Success", "Activity successfully added", "OK");

                            ActivitiesPageViewModel.AddedActivity = new AddedActivityBackNavigationModel() { Activity = activity, LoggedInUser = loggedInUser, TerritoryList = TerritoryList, CustomerName = SelectedCustomerName, CustomerNo = SelectedCustomerNo, City = customerCity, State = customerState, IsUserActivity = isUserActivity };

                            SelectedHours = string.Empty;

                            NavigationService.GoBackInShell();
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(SelectedActivityType))
                        {
                            await AlertHelper.Instance.ShowConfirmationAlert("Error", "Please select Activity Type", "OK");
                        }
                        else if (!SelectedCallDate.HasValue)
                        {
                            await AlertHelper.Instance.ShowConfirmationAlert("Error", "Please select call date", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AddActivityPageViewModel), nameof(SaveCommandHandler), ex.StackTrace);
                IsLoading = false;
            }
            finally { LocalStoredFile = null; }
        }

        private async Task CancelCommandHandler()
        {
            var result = await AlertHelper.Instance.ShowConfirmationAlert("Cancel Activity", "Are you sure you want to discard the current activity?", "OK", "Cancel");

            if (result)
            {
                NavigationService.GoBackInShell();
            }
        }

        private void ActivitySelectionCommandHandler(string activity)
        {
            SelectedActivityType = activity;
            IsAttachmentIconVisible = ("Wholesale Invoice".Equals(activity) || "Narrative-Weekly Report".Equals(activity));
            IsNarrativeWeeklyReport = "Narrative-Weekly Report".Equals(activity);
            CustomerPageUIModel customer = null;
            if (string.IsNullOrWhiteSpace(SelectedCustomerName) && string.IsNullOrWhiteSpace(SelectedCustomerNo))
            {
                /// user activity               
                customer = CustomerList.FirstOrDefault(x => (x.DeviceCustomerId != "0-0")
                && x.CustomerName.Equals($"{loggedInUser.FirstName} {loggedInUser.LastName}",StringComparison.OrdinalIgnoreCase));
                if (customer != null)
                {
                    SelectedCustomerNo = customer?.CustomerNumber;
                    SelectedCustomerName = customer?.CustomerName;
                    selectedDeviceCustomerId = customer.DeviceCustomerId;
                    customerState = HelperMethods.GetValueFromIdNameDictionary(stateDict, customer.StateId);
                    customerCity = customer?.City;
                }
                else
                {
                    SelectedCustomerName = loggedInUser.FirstName + " " + loggedInUser.LastName;
                    SelectedCustomerNo = Convert.ToString(loggedInUser.UserId);
                    selectedDeviceCustomerId = "0-0";
                }
            }
            isUserActivity = selectedDeviceCustomerId.Equals(customer?.DeviceCustomerId) || selectedDeviceCustomerId.Equals("0-0");
        }

        private async Task CustomerSelectionCommandHandlerAsync(CustomerPageUIModel customer)
        {
            if (customer != null)
            {
                if ("None".Equals(customer.CustomerName))
                {
                    await LoadUserActivityAsync();
                    SelectedCustomerNo = string.Empty;
                    SelectedCustomerName = string.Empty;
                    selectedDeviceCustomerId = string.Empty;
                    customerState = string.Empty;
                    customerCity = string.Empty;
                    SelectedActivityType = string.Empty;
                    isUserActivity = true;
                }
                else if (customer.DeviceCustomerId.Equals(loginedInUserCustomerDeviceId))
                {
                    await LoadUserActivityAsync();
                    SelectedCustomerNo = customer?.CustomerNumber;
                    SelectedCustomerName = customer?.CustomerName;
                    selectedDeviceCustomerId = customer.DeviceCustomerId;
                    customerState = HelperMethods.GetValueFromIdNameDictionary(stateDict, customer.StateId);
                    customerCity = customer?.City;
                    isUserActivity = true;
                    SelectedActivityType = "Narrative-Weekly Report";
                }
                else
                {
                    await LoadCustomerActivityAsync();
                    SelectedCustomerNo = customer?.CustomerNumber;
                    SelectedCustomerName = customer?.CustomerName;
                    selectedDeviceCustomerId = customer.DeviceCustomerId;
                    customerState = HelperMethods.GetValueFromIdNameDictionary(stateDict, customer.StateId);
                    customerCity = customer?.City;
                    isUserActivity = false;
                    SelectedActivityType = customer.AccountType == 1 ? ActivityTypeList.FirstOrDefault(x => x.ToLower().Contains("Distributor Sales Call".ToLower()))
                        : ActivityTypeList.FirstOrDefault(x => x.ToLower().Contains("Retail Sales Call".ToLower()));
                }

                IsNarrativeWeeklyReport = "Narrative-Weekly Report".Equals(SelectedActivityType);
                IsAttachmentIconVisible = ("Wholesale Invoice".Equals(SelectedActivityType) || "Narrative-Weekly Report".Equals(SelectedActivityType));
            }
        }

        private async Task OnNavigatedToCommandHandler(object obj)
        {
            Notes = "";
            loggedInUser = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
            if (loggedInUser.RoleID == await AppReference.QueryService.GetRoleIdAsync(ApplicationConstants.BDRoleName))
            {
                TerritoryList = (await AppReference.QueryService.GetBDApproverTerritoriesAsync(loggedInUser.BDID,loggedInUser.RegionId)).ToList();
            }
            else if(loggedInUser.RoleID == await AppReference.QueryService.GetRoleIdAsync(ApplicationConstants.AVPRoleName))
            {
                TerritoryList = (await AppReference.QueryService.GetAVPTerritoriesAsync(loggedInUser.AVPID)).ToList();
            }
            else
            {
                TerritoryList = await AppReference.QueryService.GetTerritoryFromIds(loggedInUser.TerritoryID);
            }

            stateDict = await AppReference.QueryService.GetStateDict();

            loginedInUserCustomerDeviceId = await AppReference.QueryService.GetUserCustomerDeviceId(loggedInUser.FirstName + " " + loggedInUser.LastName);

            try
            {
                //if (SelectedCallDate != null || !SelectedCallDate.HasValue)
                //{
                SelectedCallDate = DateTime.Now;
                //}

                if (obj is CustomerMaster)
                {
                    await LoadCustomerActivityAsync();

                    var customer = obj as CustomerMaster;

                    isCustomerAddActivity = true;

                    selectedDeviceCustomerId = customer.DeviceCustomerID;

                    SelectedActivityType = customer.AccountType == 1 ? ActivityTypeList.FirstOrDefault(x => x.ToLower().Contains("Distributor Sales Call".ToLower()))
                        : ActivityTypeList.FirstOrDefault(x => x.ToLower().Contains("Retail Sales Call".ToLower()));

                    SelectedCustomerName = customer?.CustomerName;

                    SelectedCustomerNo = customer?.CustomerNumber;

                    customerCity = customer?.City;

                    customerState = HelperMethods.GetValueFromIdNameDictionary(stateDict, customer.PhysicalAddressStateID);
                }
                else
                {
                    isCustomerAddActivity = false;
                    var _tempList = await AppReference.QueryService.GetCustomerListForActivity();
                    CustomerList = new ObservableCollection<CustomerPageUIModel>(_tempList.OrderBy(x => x.CustomerName).Select(x => x.CopyToUIModel()));
                    CustomerList.Insert(0, new CustomerPageUIModel() { CustomerName = "None", CustomerNumber = "-" });

                    await LoadUserActivityAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AddActivityPageViewModel), "OnNavigatedToCommand", ex.StackTrace);
            }
        }

        private async Task LoadUserActivityAsync() => ActivityTypeList = new ObservableCollection<string>(await AppReference.QueryService.GetUserActivityTypeAsync());
        private async Task LoadCustomerActivityAsync() => ActivityTypeList = new ObservableCollection<string>(await AppReference.QueryService.GetCustomerActivityTypeAsync());

        //private void LoadCustomerActivity()
        //{
        //    ActivityTypeList=new ObservableCollection<string>();
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

        private async Task<string> SaveAttachmentAsPdf(string fileName)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var newFolder = await folder.CreateFolderAsync(ApplicationConstants.ACTIVITY_BASE_FOLDER, CreationCollisionOption.OpenIfExists);
                var path = Path.Combine(newFolder.Path, fileName);
                return EmailAndPrintOrder.PdfCreator.Instance.CreatePdfFromImages(path, AddedAttachementItemSource) ? path : string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AddActivityPageViewModel), nameof(SaveAttachmentAsPdf), ex.Message);
                return string.Empty;
            }
        }

        private void DeleteSelectedImages()
        {
            try
            {
                foreach (var item in AddedAttachementItemSource)
                {
                    if (File.Exists(item))
                    {
                        File.Delete(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AddActivityPageViewModel), nameof(DeleteSelectedImages), ex.Message);
            }
        }

        #endregion
    }
}