using DevExpress.Mvvm.Native;
using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace DRLMobile.Uwp.ViewModel
{
    public class ActivitiesPageViewModel : BaseModel
    {
        private bool isCustomerActivity;
        private CustomerMaster currentCustomer;
        public static AddedActivityBackNavigationModel AddedActivity { get; set; }
        public static object EditedActivity { get; set; }
        private readonly App AppReference = (App)Application.Current;

        private int headerSearchSelectedCallActivityID;
        private DispatcherQueueTimer _typeTimer = null;
        private readonly Windows.UI.Core.CoreDispatcher _coreDispatcher;

        #region properties

        private bool _isRbDataVisible;
        public bool IsRbDataVisible
        {
            get { return _isRbDataVisible; }
            set { SetProperty(ref _isRbDataVisible, value); }
        }

        private bool _isDeleteActivityVisible;
        public bool IsDeleteActivityVisible
        {
            get { return _isDeleteActivityVisible; }
            set { SetProperty(ref _isDeleteActivityVisible, value); }
        }

        private bool _isNoActionVisible;
        public bool IsNoActionVisible
        {
            get { return _isNoActionVisible; }
            set { SetProperty(ref _isNoActionVisible, value); }
        }

        private bool _isSecondaryTitleVisible;
        public bool IsSecondaryTitleVisible
        {
            get { return _isSecondaryTitleVisible; }
            set { SetProperty(ref _isSecondaryTitleVisible, value); }
        }

        private string _secondaryTitleText;
        public string SecondaryTitleText
        {
            get { return _secondaryTitleText; }
            set { SetProperty(ref _secondaryTitleText, value); }
        }

        private bool _isNoActivityMsgVisible;
        public bool IsNoActivityMsgVisible
        {
            get { return _isNoActivityMsgVisible; }
            set { SetProperty(ref _isNoActivityMsgVisible, value); }
        }

        private bool _loadAllCustomerGrid;
        public bool LoadAllCustomerGrid
        {
            get { return _loadAllCustomerGrid; }
            set { SetProperty(ref _loadAllCustomerGrid, value); }
        }

        private bool _loadCustomerGrid;
        public bool LoadCustomerGrid
        {
            get { return _loadCustomerGrid; }
            set { SetProperty(ref _loadCustomerGrid, value); }
        }

        private bool _loadAllData;
        public bool LoadAllData
        {
            get { return _loadAllData; }
            set { SetProperty(ref _loadAllData, value); }
        }

        private bool _isRbActivityAllOptionChecked;
        public bool IsRbActivityAllOptionChecked
        {
            get { return _isRbActivityAllOptionChecked; }
            set { SetProperty(ref _isRbActivityAllOptionChecked, value); }
        }

        private string _territoryIds;
        public string TerritoryIds
        {
            get { return _territoryIds; }
            set { SetProperty(ref _territoryIds, value); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private bool _isNavigatingToAddActivityPage;
        public bool IsNavigatingToAddActivityPage
        {
            get { return _isNavigatingToAddActivityPage; }
            set { SetProperty(ref _isNavigatingToAddActivityPage, value); }
        }

        private ICollectionView _collectionView;
        public ICollectionView CollectionView
        {
            get { return _collectionView; }
            set { SetProperty(ref _collectionView, value); }
        }

        private int _bdId;
        public int BDId
        {
            get { return _bdId; }
            set { SetProperty(ref _bdId, value); }
        }

        private int _avpId;
        public int AVPId
        {
            get { return _avpId; }
            set { SetProperty(ref _avpId, value); }
        }

        readonly ObservableCollection<GroupInfoCollection<ActivityForIndividualCustomerUIModel>> GroupedActivity;

        public List<ActivityForAllCustomerUIModel> DbDataForAllCustomer { get; set; }

        public List<ActivityForIndividualCustomerUIModel> DbDataForIndividualCustomer { get; set; }

        private ObservableCollection<ActivityForAllCustomerUIModel> _allCustomerActivitiesItemSource;
        public ObservableCollection<ActivityForAllCustomerUIModel> AllCustomerActivitiesItemSource
        {
            get { return _allCustomerActivitiesItemSource; }
            set { SetProperty(ref _allCustomerActivitiesItemSource, value); }
        }

        private ObservableCollection<object> _headerSearchItemSource;
        public ObservableCollection<object> HeaderSearchItemSource
        {
            get { return _headerSearchItemSource; }
            set { SetProperty(ref _headerSearchItemSource, value); }
        }

        private string _pageTitleText;
        public string PageTitleText
        {
            get { return _pageTitleText; }
            set { SetProperty(ref _pageTitleText, value); }
        }

        private string _sortCustomerNoText;
        public string SortCustomerNoText
        {
            get { return _sortCustomerNoText; }
            set { SetProperty(ref _sortCustomerNoText, value); }
        }
        private string _sortCustomerNameText;
        public string SortCustomerNameText
        {
            get { return _sortCustomerNameText; }
            set { SetProperty(ref _sortCustomerNameText, value); }
        }

        private string _sortTerritoryText;
        public string SortTerritoryText
        {
            get { return _sortTerritoryText; }
            set { SetProperty(ref _sortTerritoryText, value); }
        }
        private string _sortDistributorText;
        public string SortDistributorText
        {
            get { return _sortDistributorText; }
            set { SetProperty(ref _sortDistributorText, value); }
        }
        private string _sortCreatorText;
        public string SortCreatorText
        {
            get { return _sortCreatorText; }
            set { SetProperty(ref _sortCreatorText, value); }
        }
        private string _sortActivityTypeText;
        public string SortActivityTypeText
        {
            get { return _sortActivityTypeText; }
            set { SetProperty(ref _sortActivityTypeText, value); }
        }
        private string _sortSalesText;
        public string SortSalesText
        {
            get { return _sortSalesText; }
            set { SetProperty(ref _sortSalesText, value); }
        }
        private string _sortStateText;
        public string SortStateText
        {
            get { return _sortStateText; }
            set { SetProperty(ref _sortStateText, value); }
        }
        private string _sortCityText;
        public string SortCityText
        {
            get { return _sortCityText; }
            set { SetProperty(ref _sortCityText, value); }
        }
        private string _sortCalldateText;
        public string SortCalldateText
        {
            get { return _sortCalldateText; }
            set { SetProperty(ref _sortCalldateText, value); }
        }


        private string _filterCustomerNumberText;
        public string FilterCustomerNumberText
        {
            get { return _filterCustomerNumberText; }
            set { SetProperty(ref _filterCustomerNumberText, value); }
        }
        private string _filterCustomerNameText;
        public string FilterCustomerNameText
        {
            get { return _filterCustomerNameText; }
            set { SetProperty(ref _filterCustomerNameText, value); }
        }
        private string _filterCityText;
        public string FilterCityText
        {
            get { return _filterCityText; }
            set { SetProperty(ref _filterCityText, value); }
        }
        private string _filterStateText;
        public string FilterStateText
        {
            get { return _filterStateText; }
            set { SetProperty(ref _filterStateText, value); }
        }
        private string _filterSalesText;
        public string FilterSalesText
        {
            get { return _filterSalesText; }
            set { SetProperty(ref _filterSalesText, value); }
        }
        private string _filterActivityTypeText;
        public string FilterActivityTypeText
        {
            get { return _filterActivityTypeText; }
            set { SetProperty(ref _filterActivityTypeText, value); }
        }
        private string _filterCreatorText;
        public string FilterCreatorText
        {
            get { return _filterCreatorText; }
            set { SetProperty(ref _filterCreatorText, value); }
        }
        private string _filterDistributorText;
        public string FilterDistributorText
        {
            get { return _filterDistributorText; }
            set { SetProperty(ref _filterDistributorText, value); }
        }
        private string _filterTerritoryText;
        public string FilterTerritoryText
        {
            get { return _filterTerritoryText; }
            set { SetProperty(ref _filterTerritoryText, value); }
        }
        private string _filterCalldateText;
        public string FilterCalldateText
        {
            get { return _filterCalldateText; }
            set { SetProperty(ref _filterCalldateText, value); }
        }


        #endregion

        #region Commands
        public ICommand AddActivityCommand { get; private set; }
        public ICommand OnNavigatedToCommand { get; private set; }
        public ICommand HeaderTextChangeCommand { get; private set; }
        public ICommand SuggestionChoosenCommand { get; private set; }
        public ICommand AddNewActivityCommand { get; private set; }
        public ICommand AllCustomerItemSelection { get; private set; }
        public ICommand CustomerGridItemClick { get; private set; }
        public ICommand EditedCommand { get; private set; }
        public ICommand DeleteActivityCommand { get; private set; }
        public ICommand rbActivityDataChangedCommand { get; private set; }
        public ICommand SortColumnClickCommand { get; private set; }
        public ICommand FilterColumnClickCommand { get; private set; }
        #endregion

        #region Constructor
        public ActivitiesPageViewModel()
        {
            RegisterCommands();
            DbDataForAllCustomer = new List<ActivityForAllCustomerUIModel>();
            AllCustomerActivitiesItemSource = new ObservableCollection<ActivityForAllCustomerUIModel>();
            HeaderSearchItemSource = new ObservableCollection<object>();
            DbDataForIndividualCustomer = new List<ActivityForIndividualCustomerUIModel>();
            GroupedActivity = new ObservableCollection<GroupInfoCollection<ActivityForIndividualCustomerUIModel>>();
            _coreDispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            IsRbActivityAllOptionChecked = true;
        }
        #endregion

        #region Private Methods
        private void RegisterCommands()
        {
            OnNavigatedToCommand = new AsyncRelayCommand<object>(OnNavigatedToCommandHandler);
            EditedCommand = new RelayCommand(EditedCommandHandler);
            CustomerGridItemClick = new RelayCommand<ActivityForIndividualCustomerUIModel>(CustomerGridItemClickHandler);
            AddNewActivityCommand = new RelayCommand(AddNewActivityCommandHandler);
            AddActivityCommand = new RelayCommand(AddActivityCommandHandler);
            SuggestionChoosenCommand = new AsyncRelayCommand<object>(SuggestionChoosenCommandHandler);
            HeaderTextChangeCommand = new AsyncRelayCommand<string>(HeaderTextChangeCommandHandler);
            AllCustomerItemSelection = new RelayCommand<ActivityForAllCustomerUIModel>(AllCustomerItemSelectionHandler);
            DeleteActivityCommand = new AsyncRelayCommand<object>(DeleteActivityCommandHandler);
            rbActivityDataChangedCommand = new AsyncRelayCommand<object>(rbActivityDataChangedCommandHandler);
            SortColumnClickCommand = new AsyncRelayCommand<string>(SortColumnClickCommandHandlerAsync);
            FilterColumnClickCommand = new RelayCommand<string>(FilterColumnClickCommandHandler);
        }

        private async Task DeleteActivityCommandHandler(object arg)
        {
            try
            {
                var result = await AlertHelper.Instance.ShowConfirmationAlert("Alert", "Are you sure that you want to delete this activity?", "Yes", "No");

                if (result)
                {
                    IsLoading = true;

                    if (arg is ActivityForAllCustomerUIModel)
                    {
                        var activity = arg as ActivityForAllCustomerUIModel;

                        var isDeleted = await AppReference.QueryService.DeleteActivity(activity.CallActivityDeviceID);

                        if (isDeleted)
                        {
                            await AlertHelper.Instance.ShowConfirmationAlert("Success", "Activity deleted successfully", "OK");

                            AllCustomerActivitiesItemSource.Remove(activity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ActivitiesPageViewModel), "DeleteActivityCommand", ex.StackTrace);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void EditedCommandHandler()
        {
            if (EditedActivity != null)
            {
                if (EditedActivity is ActivityForAllCustomerUIModel)
                {
                    var allCustomerActivity = EditedActivity as ActivityForAllCustomerUIModel;
                    var objetToReplace = DbDataForAllCustomer.FirstOrDefault(x => x.CallActivityDeviceID == allCustomerActivity.CallActivityDeviceID);
                    if (objetToReplace != null)
                    {
                        DbDataForAllCustomer.Remove(objetToReplace);
                        DbDataForAllCustomer.Add(allCustomerActivity);
                        var sortedData = DbDataForAllCustomer.OrderByDescending(x => x.LastcallDate);
                        AllCustomerActivitiesItemSource = new ObservableCollection<ActivityForAllCustomerUIModel>(sortedData);
                    }
                }
                else if (EditedActivity is ActivityForIndividualCustomerUIModel)
                {
                    var individualActivity = EditedActivity as ActivityForIndividualCustomerUIModel;
                    var objetToReplace = DbDataForIndividualCustomer.FirstOrDefault(x => x.CallActivityID == individualActivity.CallActivityID);
                    if (objetToReplace != null)
                    {
                        DbDataForIndividualCustomer.Remove(objetToReplace);
                        DbDataForIndividualCustomer.Add(individualActivity);
                        GroupAndShowDataGridUI(DbDataForIndividualCustomer);
                    }
                }
            }
            EditedActivity = null;
        }

        private void CustomerGridItemClickHandler(ActivityForIndividualCustomerUIModel selectedItem)
        {
            if (selectedItem != null && isAllowed && !selectedItem.ActivityType.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
            {
                NavigationService.NavigateShellFrame(typeof(EditActivityPage), selectedItem);
            }
        }

        private void AllCustomerItemSelectionHandler(ActivityForAllCustomerUIModel selectedItem)
        {
            NavigationService.NavigateShellFrame(typeof(EditActivityPage), selectedItem);
        }

        //function not in use
        private void AddNewActivityCommandHandler()
        {
            if (isCustomerActivity && AddedActivity != null)
            {
                ActivityForIndividualCustomerUIModel activity = new ActivityForIndividualCustomerUIModel()
                {
                    ActivityType = AddedActivity?.Activity?.ActivityType,
                    //CallActivityID = AddedActivity.Activity.CallActivityID.Value,
                    CallDate = AddedActivity?.Activity?.CallDate,
                    Comments = AddedActivity?.Activity?.Comments,
                    CreatedDate = AddedActivity?.Activity?.CreatedDate,
                    CustomerID = AddedActivity?.Activity?.CustomerID,
                    CallActivityDeviceID = AddedActivity?.Activity?.CallActivityDeviceID,
                    CustomerName = AddedActivity?.CustomerName,
                    CustomerNumber = AddedActivity?.CustomerNo,
                    FirstName = AddedActivity?.LoggedInUser?.FirstName,
                    Hours = AddedActivity?.Activity?.Hours,
                    LastName = AddedActivity?.LoggedInUser?.LastName,
                    UserID = AddedActivity.LoggedInUser.UserId,
                    TerritoryID = AddedActivity.TerritoryList.FirstOrDefault().TerritoryID,
                    TerritoryName = AddedActivity?.TerritoryList?.FirstOrDefault()?.TerritoryName,
                    DisplayTerritoryName = AddedActivity?.TerritoryList?.FirstOrDefault()?.TerritoryNumber,
                    UserName = AddedActivity?.LoggedInUser?.UserName,
                };
                DbDataForIndividualCustomer.Add(activity);
                GroupAndShowDataGridUI(DbDataForIndividualCustomer);
                LoadCustomerGrid = true;
                IsNoActivityMsgVisible = false;
            }
            else
            {
                if (AddedActivity != null)
                {
                    ActivityForAllCustomerUIModel activity = new ActivityForAllCustomerUIModel()
                    {
                        ActivityType = AddedActivity?.Activity?.ActivityType,
                        CallActivityDeviceID = AddedActivity?.Activity?.CallActivityDeviceID,
                        CallDate = AddedActivity?.Activity?.CallDate,
                        //CallActivityID = AddedActivity.Activity.CallActivityID.Value,
                        Comments = AddedActivity?.Activity?.Comments,
                        CreatedDate = AddedActivity?.Activity?.CreatedDate,
                        CustomerName = AddedActivity?.CustomerName,
                        CustomerNumber = AddedActivity.IsUserActivity ? string.Empty : AddedActivity?.CustomerNo,
                        DistributorNo = string.Empty,
                        Hours = AddedActivity?.Activity?.Hours,
                        PhysicalAddressCityID = AddedActivity?.City,
                        StateName = AddedActivity?.State,
                        UserName = AddedActivity?.LoggedInUser.UserName,
                        UserID = AddedActivity.LoggedInUser.UserId,
                        UserNameFull = AddedActivity?.LoggedInUser?.FirstName + " " + AddedActivity?.LoggedInUser?.LastName

                    };
                    DbDataForAllCustomer.Add(activity);
                    AllCustomerActivitiesItemSource.Insert(0, activity);
                    IsNoActivityMsgVisible = false;
                    LoadAllCustomerGrid = true;
                }
            }

            AddedActivity = null;
        }

        private void AddActivityCommandHandler()
        {
            IsNavigatingToAddActivityPage = true;
            var navObj = isCustomerActivity ? currentCustomer : null;
            NavigationService.NavigateShellFrame(typeof(AddActivityPage), navObj);
        }

        private async Task SuggestionChoosenCommandHandler(object obj)
        {
            if (obj is ActivityForAllCustomerUIModel)
            {
                var activity = obj as ActivityForAllCustomerUIModel;
                headerSearchSelectedCallActivityID = activity.CallActivityID;
                await LoadAllCustomerGridAsync();
            }
            else if (obj is ActivityForIndividualCustomerUIModel)
            {
                isAllowed = false;
                var activity = obj as ActivityForIndividualCustomerUIModel;
                var list = new List<ActivityForIndividualCustomerUIModel>() { activity };
                GroupAndShowDataGridUI(list);
                await Task.Delay(2000);
                isAllowed = true;
            }
        }

        private bool isAllowed = true;
        private async Task HeaderTextChangeCommandHandler(string text)
        {
            if (!IsNoActivityMsgVisible)
            {
                _typeTimer.Debounce(async () =>
            {
                ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
                if (shellPage != null)
                {
                    shellPage.ViewModel.IsSideMenuItemClickable = false;
                }
                try
                {
                    // Only executes this code after 0.3 seconds have elapsed since last trigger.
                    if (!LoadCustomerGrid)
                    {
                        if (HeaderSearchItemSource == null)
                        {
                            HeaderSearchItemSource = new ObservableCollection<object>();
                        }
                        HeaderSearchItemSource.Clear();
                        if (string.IsNullOrWhiteSpace(text))
                        {
                            headerSearchSelectedCallActivityID = 0;
                            await LoadAllCustomerGridAsync();
                        }
                        else
                        {
                            var tempList = DbDataForAllCustomer.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();
                            if (tempList == null || tempList.Count == 0)
                            {
                                HeaderSearchItemSource.Add(new ActivityForAllCustomerUIModel() { CustomerName = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                            }
                            else
                            {
                                //HeaderSearchItemSource=new ObservableCollection<object>(tempList);
                                tempList.ForEach(x => HeaderSearchItemSource.Add(x));
                            }
                        }
                    }
                    else if (LoadCustomerGrid && CollectionView.Any())
                    {
                        isAllowed = false;
                        HeaderSearchItemSource.Clear();
                        if (string.IsNullOrWhiteSpace(text))
                        {
                            GroupAndShowDataGridUI(DbDataForIndividualCustomer);
                        }
                        else
                        {
                            var tempList = DbDataForIndividualCustomer.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();
                            if (tempList == null || tempList.Count == 0)
                            {
                                HeaderSearchItemSource.Add(new ActivityForIndividualCustomerUIModel() { ActivityType = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                            }
                            else
                            {
                                tempList.ForEach(x => HeaderSearchItemSource.Add(x));
                            }
                        }
                        isAllowed = true;
                    }

                    if (shellPage != null)
                    {
                        shellPage.ViewModel.IsSideMenuItemClickable = true;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.WriteToErrorLog(nameof(ActivitiesPageViewModel), nameof(HeaderTextChangeCommandHandler), ex);
                }

                if (shellPage != null)
                {
                    shellPage.ViewModel.IsSideMenuItemClickable = true;
                }
            }, TimeSpan.FromSeconds(0.7));
            }
        }

        private async Task OnNavigatedToCommandHandler(object parameter)
        {
            ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = false;
            }

            try
            {
                IsLoading = true;
                IsRbDataVisible = true;
                EditedActivity = null;
                AddedActivity = null;
                SecondaryTitleText = string.Empty;
                IsNoActivityMsgVisible = false;
                LoadAllCustomerGrid = false;
                LoadCustomerGrid = false;
                DbDataForAllCustomer?.Clear();
                AllCustomerActivitiesItemSource?.Clear();
                DbDataForIndividualCustomer?.Clear();
                GroupedActivity?.Clear();
                HeaderSearchItemSource?.Clear();
                _typeTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();

                PageTitleText = string.Empty;

                var loggedInUser = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
                TerritoryIds = loggedInUser.TerritoryID;
                BDId = loggedInUser.BDID;
                AVPId = loggedInUser.AVPID;

                // Check if rbActivity is checked
                if (IsRbActivityAllOptionChecked)
                {
                    LoadAllData = true;
                }
                else
                {
                    LoadAllData = false;
                }

                if (parameter is CustomerMaster)
                {
                    isCustomerActivity = true;

                    AppReference.IsCustomerActivity = true;

                    currentCustomer = parameter as CustomerMaster;

                    SecondaryTitleText = currentCustomer?.CustomerName;

                    IsSecondaryTitleVisible = true;
                    IsRbDataVisible = false;

                    DbDataForIndividualCustomer = await AppReference.QueryService.GetCallActivitiesOfSelectedArea(currentCustomer?.DeviceCustomerID, AppReference.AreaUserSelectedId, AppReference.LoggedInUserRoleId);

                    PageTitleText = "ACTIVITY";

                    if (DbDataForIndividualCustomer != null && DbDataForIndividualCustomer.Count > 0)
                    {
                        GroupAndShowDataGridUI(DbDataForIndividualCustomer);
                        LoadCustomerGrid = true;
                    }
                    else
                    {
                        LoadCustomerGrid = false;
                        IsNoActivityMsgVisible = true;
                    }
                }
                else
                {
                    await GetActivityDataAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ActivitiesPageViewModel), nameof(OnNavigatedToCommandHandler), ex);
            }
            IsLoading = false;

            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = true;
            }
        }

        private async Task GetActivityDataAsync()
        {
            try
            {
                isCustomerActivity = false;

                AppReference.IsCustomerActivity = false;

                PageTitleText = "ACTIVITY LIST";
                DbDataForAllCustomer = new List<ActivityForAllCustomerUIModel>();

                if (AppReference.LoggedInUserRoleId == 3 || AppReference.LoggedInUserRoleId == 6 || AppReference.LoggedInUserRoleId == 2
                    || AppReference.LoggedInUserRoleId == 11 || AppReference.LoggedInUserRoleId == 12 || AppReference.LoggedInUserRoleId > 6)
                {
                    if (AppReference.AreaUserSelectedId > 0)
                    {
                        if (AppReference.LoggedInUserRoleId == 6 || AppReference.LoggedInUserRoleId == 17 || AppReference.LoggedInUserRoleId == await AppReference.QueryService.GetRoleIdAsync(ApplicationConstants.AVPRoleName))
                        {
                            var regions = await AppReference.QueryService.GetRegionsOnBasisOfZoneIdsAndPresentCustomers(new List<int> { AppReference.AreaUserSelectedId });
                            var territories = await AppReference.QueryService.GetTerritoryOnBasisOfRegionIdsAndPresentCustomers(regions.Select(x => x.RegionID));
                            TerritoryIds = string.Join(",", territories.Select(x => x.TerritoryID));
                        }
                        else if (AppReference.LoggedInUserRoleId == 3)
                        {
                            var territories = await AppReference.QueryService.GetTerritoryOnBasisOfRegionIdsAndPresentCustomers(new List<int> { AppReference.AreaUserSelectedId });
                            TerritoryIds = string.Join(",", territories.Select(x => x.TerritoryID));
                        }
                    }
                    else if (AppReference.LoggedInUserRoleId == await AppReference.QueryService.GetRoleIdAsync(ApplicationConstants.BDRoleName)) // fetch territoriy ids for BD
                    {
                        var territories = await AppReference.QueryService.GetBDApproverTerritoriesAsync(BDId, AppReference.LoggedInUserRegionId);
                        TerritoryIds = string.Join(",", territories.Select(x => x.TerritoryID));
                    }
                    else if (AppReference.LoggedInUserRoleId == await AppReference.QueryService.GetRoleIdAsync(ApplicationConstants.AVPRoleName)) // fetch territoriy ids for AVP
                    {
                        var territories = await AppReference.QueryService.GetAVPTerritoriesAsync(AVPId);
                        TerritoryIds = string.Join(",", territories.Select(x => x.TerritoryID));
                    }
                    //DbDataForAllCustomer = await AppReference.QueryService.GetCallActivitiesOfAllCustomersForNationalAndZoneAndRegionManagers(TerritoryIds, LoadAllData);
                    DbDataForAllCustomer.AddRange(await AppReference.QueryService.GetCallActivitiesOfAllCustomersForNationalAndZoneAndRegionManagers(TerritoryIds, LoadAllData));
                }
                else
                {
                    var tabelData = await AppReference.QueryService.GetCallActivitiesOfAllCustomersForLoggedInUser(TerritoryIds, LoadAllData);
                    DbDataForAllCustomer = tabelData.OrderByDescending(x => x.LastcallDate).ToList();
                }

                if (DbDataForAllCustomer != null && DbDataForAllCustomer.Any())
                {
                    await LoadAllCustomerGridAsync();
                    //AllCustomerActivitiesItemSource = new ObservableCollection<ActivityForAllCustomerUIModel>(DbDataForAllCustomer);
                    LoadAllCustomerGrid = true;
                }
                else
                {
                    IsNoActivityMsgVisible = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ActivitiesPageViewModel), nameof(GetActivityDataAsync), ex);
            }
        }
        private void GroupAndShowDataGridUI(List<ActivityForIndividualCustomerUIModel> source)
        {
            if (CollectionView != null)
            {
                CollectionView = null;
                GroupedActivity.Clear();
            }

            source = source.OrderByDescending(x => x._callActivityDate).ToList();

            var query = from item in source
                        group item by item.Date into g
                        select new { GroupName = g.Key, Items = g };

            foreach (var g in query)
            {
                GroupInfoCollection<ActivityForIndividualCustomerUIModel> info = new GroupInfoCollection<ActivityForIndividualCustomerUIModel>();

                info.Key = g.GroupName;

                foreach (var item in g.Items)
                {
                    info.Add(item);
                }

                GroupedActivity.Add(info);
            }

            CollectionViewSource groupedItems = new CollectionViewSource();

            groupedItems.IsSourceGrouped = true;
            groupedItems.Source = GroupedActivity;

            CollectionView = groupedItems.View;

            LoadCustomerGrid = true;
        }
        private async Task rbActivityDataChangedCommandHandler(object obj)
        {
            IsLoading = true;
            var rbLoadData = obj as RadioButton;
            if (rbLoadData.Name == "rbAlldata" && rbLoadData.IsChecked == true)
            {
                LoadAllData = true; IsRbActivityAllOptionChecked = true;
            }
            else
            { LoadAllData = false; IsRbActivityAllOptionChecked = false; }

            EditedActivity = null;
            AddedActivity = null;
            SecondaryTitleText = string.Empty;
            IsNoActivityMsgVisible = false;
            LoadAllCustomerGrid = false;
            LoadCustomerGrid = false;
            DbDataForAllCustomer?.Clear();
            AllCustomerActivitiesItemSource?.Clear();
            DbDataForIndividualCustomer?.Clear();
            GroupedActivity?.Clear();
            HeaderSearchItemSource?.Clear();
            //ClearFitlerColSearchKeyWords();
            //ClearSortColOrder();
            await GetActivityDataAsync();
            IsLoading = false;
        }

        private async Task SortColumnClickCommandHandlerAsync(string args)
        {
            if (!IsNoActivityMsgVisible)
            {
                ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
                if (shellPage != null)
                {
                    shellPage.ViewModel.IsSideMenuItemClickable = false;
                }
                string colName = args.Split('|')[0];
                string textValue = args.Split('|')[1];
                ClearSortColOrder();
                switch (colName)
                {
                    case "customernumber":
                        if (string.IsNullOrWhiteSpace(textValue))
                        {
                            SortCustomerNoText = "sort_up";
                        }
                        else if (textValue == "sort_up")
                        {
                            SortCustomerNoText = "sort_down";
                        }
                        else if (textValue == "sort_down")
                        {
                            SortCustomerNoText = "";
                        }
                        break;
                    case "customername":
                        if (string.IsNullOrWhiteSpace(textValue))
                        {
                            SortCustomerNameText = "sort_up";
                        }
                        else if (textValue == "sort_up")
                        {
                            SortCustomerNameText = "sort_down";
                        }
                        else if (textValue == "sort_down")
                        {
                            SortCustomerNameText = "";
                        }
                        break;
                    case "city":
                        if (string.IsNullOrWhiteSpace(textValue))
                        {
                            SortCityText = "sort_up";
                        }
                        else if (textValue == "sort_up")
                        {
                            SortCityText = "sort_down";
                        }
                        else if (textValue == "sort_down")
                        {
                            SortCityText = "";
                        }
                        break;
                    case "state":
                        if (string.IsNullOrWhiteSpace(textValue))
                        {
                            SortStateText = "sort_up";
                        }
                        else if (textValue == "sort_up")
                        {
                            SortStateText = "sort_down";
                        }
                        else if (textValue == "sort_down")
                        {
                            SortStateText = "";
                        }
                        break;
                    case "sales":
                        if (string.IsNullOrWhiteSpace(textValue))
                        {
                            SortSalesText = "sort_up";
                        }
                        else if (textValue == "sort_up")
                        {
                            SortSalesText = "sort_down";
                        }
                        else if (textValue == "sort_down")
                        {
                            SortSalesText = "";
                        }
                        break;
                    case "type":
                        if (string.IsNullOrWhiteSpace(textValue))
                        {
                            SortActivityTypeText = "sort_up";
                        }
                        else if (textValue == "sort_up")
                        {
                            SortActivityTypeText = "sort_down";
                        }
                        else if (textValue == "sort_down")
                        {
                            SortActivityTypeText = "";
                        }
                        break;
                    case "creator":
                        if (string.IsNullOrWhiteSpace(textValue))
                        {
                            SortCreatorText = "sort_up";
                        }
                        else if (textValue == "sort_up")
                        {
                            SortCreatorText = "sort_down";
                        }
                        else if (textValue == "sort_down")
                        {
                            SortCreatorText = "";
                        }
                        break;
                    case "distributor":
                        if (string.IsNullOrWhiteSpace(textValue))
                        {
                            SortDistributorText = "sort_up";
                        }
                        else if (textValue == "sort_up")
                        {
                            SortDistributorText = "sort_down";
                        }
                        else if (textValue == "sort_down")
                        {
                            SortDistributorText = "";
                        }
                        break;
                    case "territory":
                        if (string.IsNullOrWhiteSpace(textValue))
                        {
                            SortTerritoryText = "sort_up";
                        }
                        else if (textValue == "sort_up")
                        {
                            SortTerritoryText = "sort_down";
                        }
                        else if (textValue == "sort_down")
                        {
                            SortTerritoryText = "";
                        }
                        break;
                    case "calldate":
                        if (string.IsNullOrWhiteSpace(textValue))
                        {
                            SortCalldateText = "sort_up";
                        }
                        else if (textValue == "sort_up")
                        {
                            SortCalldateText = "sort_down";
                        }
                        else if (textValue == "sort_down")
                        {
                            SortCalldateText = "";
                        }
                        break;
                    default:
                        break;
                }
                try
                {
                    await LoadAllCustomerGridAsync();
                }
                catch (Exception ex)
                {
                    ErrorLogger.WriteToErrorLog(nameof(ActivitiesPageViewModel), nameof(SortColumnClickCommandHandlerAsync), ex);
                }
                if (shellPage != null)
                {
                    shellPage.ViewModel.IsSideMenuItemClickable = true;
                }
            }
        }

        private void FilterColumnClickCommandHandler(string args)
        {
            if (!IsNoActivityMsgVisible)
            {
                _typeTimer.Debounce(async () =>
                 {
                     ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
                     if (shellPage != null)
                     {
                         shellPage.ViewModel.IsSideMenuItemClickable = false;
                     }
                     // Only executes this code after 0.3 seconds have elapsed since last trigger.
                     string colName = args.Split('|')[0];
                     string textValue = args.Split('|')[1];
                     textValue = textValue?.Trim();
                     try
                     {
                         await LoadAllCustomerGridAsync();
                     }
                     catch (Exception ex)
                     {
                         ErrorLogger.WriteToErrorLog(nameof(ActivitiesPageViewModel), nameof(FilterColumnClickCommandHandler), ex);
                     }
                     if (shellPage != null)
                     {
                         shellPage.ViewModel.IsSideMenuItemClickable = true;
                     }
                 }, TimeSpan.FromSeconds(0.7));
            }
        }

        private void ApplySortToAllCustomerGrid(ref IEnumerable<ActivityForAllCustomerUIModel> allCustomerList)
        {
            if (!string.IsNullOrWhiteSpace(SortCustomerNoText))
            {
                if (SortCustomerNoText == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.CustomerNumber);
                }
                else if (SortCustomerNoText == "sort_up")
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.CustomerNumber);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortCustomerNameText))
            {
                if (SortCustomerNameText == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.CustomerName);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.CustomerName);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortCityText))
            {
                if (SortCityText == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.PhysicalAddressCityID);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.PhysicalAddressCityID);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortStateText))
            {
                if (SortStateText == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.StateName);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.StateName);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortSalesText))
            {
                if (SortSalesText == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => double.TryParse(x.GrandTotal, out double valCCS) ? valCCS : default);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => double.TryParse(x.GrandTotal, out double valCCS) ? valCCS : default);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortActivityTypeText))
            {
                if (SortActivityTypeText == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.ActivityType);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.ActivityType);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortCreatorText))
            {
                if (SortCreatorText == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.UserName);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.UserName);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortDistributorText))
            {
                if (SortDistributorText == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.DistributorNo);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.DistributorNo);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortTerritoryText))
            {
                if (SortTerritoryText == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.UserNameFull);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.UserNameFull);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortCalldateText))
            {
                if (SortCalldateText == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.LastcallDate);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.LastcallDate);
                }
            }
        }

        private void ApplyFiltersToAllCustomerGrid(ref IEnumerable<ActivityForAllCustomerUIModel> allCustomerArray)
        {
            allCustomerArray = allCustomerArray.Where(x =>
            {
                bool isMatched = true;

                if (!string.IsNullOrWhiteSpace(_filterCustomerNumberText))
                {
                    if (!string.IsNullOrWhiteSpace(x.CustomerNumber))
                        isMatched &= x.CustomerNumber.Contains(_filterCustomerNumberText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                if (!string.IsNullOrWhiteSpace(_filterCustomerNameText))
                {
                    if (!string.IsNullOrWhiteSpace(x.CustomerName))
                        isMatched &= x.CustomerName.Contains(_filterCustomerNameText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                if (!string.IsNullOrWhiteSpace(_filterCityText))
                {
                    if (!string.IsNullOrWhiteSpace(x.PhysicalAddressCityID))
                        isMatched &= x.PhysicalAddressCityID.Contains(_filterCityText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                if (!string.IsNullOrWhiteSpace(_filterStateText))
                {
                    if (!string.IsNullOrWhiteSpace(x.StateName))
                        isMatched &= x.StateName.Contains(_filterStateText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                if (!string.IsNullOrWhiteSpace(_filterSalesText))
                {
                    if (!string.IsNullOrWhiteSpace(x.Sales))
                        isMatched &= x.Sales.Contains(_filterSalesText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                if (!string.IsNullOrWhiteSpace(_filterActivityTypeText))
                {
                    if (!string.IsNullOrWhiteSpace(x.ActivityType))
                        isMatched &= x.ActivityType.Contains(_filterActivityTypeText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                if (!string.IsNullOrWhiteSpace(_filterCreatorText))
                {
                    if (!string.IsNullOrWhiteSpace(x.UserName))
                        isMatched &= x.UserName.Contains(_filterCreatorText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                if (!string.IsNullOrWhiteSpace(_filterDistributorText))
                {
                    if (!string.IsNullOrWhiteSpace(x.DistributorNo))
                        isMatched &= x.DistributorNo.Contains(_filterDistributorText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                if (!string.IsNullOrWhiteSpace(_filterTerritoryText))
                {
                    if (!string.IsNullOrWhiteSpace(x.UserNameFull))
                        isMatched &= x.UserNameFull.Contains(_filterTerritoryText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                if (!string.IsNullOrWhiteSpace(_filterCalldateText))
                {
                    if (!string.IsNullOrWhiteSpace(x.CallDate))
                        isMatched &= x.LastcallDate.ToString("MM-dd-yyyy").Contains(_filterCalldateText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                return isMatched;
            });
        }

        private async Task LoadAllCustomerGridAsync()
        {
            IsLoading = true;
            LoadAllCustomerGrid = false;
            await _coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                  () =>
                  {
                      IEnumerable<ActivityForAllCustomerUIModel> AllActivities = null;
                      if (headerSearchSelectedCallActivityID > 0)
                      {
                          AllActivities = DbDataForAllCustomer.Where(x => x.CallActivityID ==
                      headerSearchSelectedCallActivityID).ToList();
                      }
                      else { AllActivities = DbDataForAllCustomer; }
                      AllCustomerActivitiesItemSource?.Clear();
                      ApplyFiltersToAllCustomerGrid(ref AllActivities);
                      ApplySortToAllCustomerGrid(ref AllActivities);
                      this.AllCustomerActivitiesItemSource = new ObservableCollection<ActivityForAllCustomerUIModel>(AllActivities);
                  });
            LoadAllCustomerGrid = true;
            IsLoading = false;
        }

        private void ClearFitlerColSearchKeyWords()
        {
            FilterCustomerNumberText = FilterCustomerNameText = FilterCityText = FilterStateText = FilterSalesText = FilterActivityTypeText = FilterCreatorText = FilterDistributorText = FilterTerritoryText = FilterCalldateText = "";
        }
        private void ClearSortColOrder()
        {
            SortCustomerNoText = SortCustomerNameText = SortCityText = SortStateText = SortSalesText = SortActivityTypeText = SortCreatorText = SortDistributorText = SortTerritoryText = SortCalldateText = "";
        }
        public void DispatcherQueueTimerCleanup()
        {
            _typeTimer?.Stop();
            _typeTimer = null;
        }
        #endregion

    }
}
