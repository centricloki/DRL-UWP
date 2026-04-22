using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;

using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.UI.Xaml;

namespace DRLMobile.Uwp.ViewModel
{
    public class AddEditRoutePageViewModel : BaseModel
    {
        private readonly App AppReference = (App)Application.Current;
        private List<CustomerPageUIModel> DbCustomerDataSource;
        private List<CustomerPageUIModel> CustomerDataSourceOnSearchItem;
        private bool IsEdit;
        private int editRouteId;
        private string editRouteDeviceId;

        #region Properties

        private string _headerTitle;
        public string HeaderTitle
        {
            get { return _headerTitle; }
            set { SetProperty(ref _headerTitle, value); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private bool _loadUI;
        public bool LoadUI
        {
            get { return _loadUI; }
            set { SetProperty(ref _loadUI, value); }
        }

        private List<UserMaster> _userDataList;
        public List<UserMaster> TerritoryManagersDataList
        {
            get { return _userDataList; }
            set { SetProperty(ref _userDataList, value); }
        }

        private ObservableCollection<string> _territoryManagersCollection;
        public ObservableCollection<string> TerritoryManagersCollection
        {
            get { return _territoryManagersCollection; }
            set { SetProperty(ref _territoryManagersCollection, value); }
        }

        private ObservableCollection<CustomerPageUIModel> _headerSearchItemSource;
        public ObservableCollection<CustomerPageUIModel> HeaderSearchItemSource
        {
            get { return _headerSearchItemSource; }
            set { SetProperty(ref _headerSearchItemSource, value); }
        }

        private ObservableCollection<CustomerPageUIModel> _customerList;
        public ObservableCollection<CustomerPageUIModel> CustomerList
        {
            get { return _customerList; }
            set { SetProperty(ref _customerList, value); }
        }

        private DateTimeOffset? _startDate;
        public DateTimeOffset? StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }

        private DateTimeOffset? _endDate;
        public DateTimeOffset? EndDate
        {
            get { return _endDate; }
            set { SetProperty(ref _endDate, value); }
        }

        private string _routeName;
        public string RouteName
        {
            get { return _routeName; }
            set { SetProperty(ref _routeName, value); }
        }

        private string _routeBrief;
        public string RouteBrief
        {
            get { return _routeBrief; }
            set { SetProperty(ref _routeBrief, value); }
        }

        private Visibility _assignToPanelVisibility = Visibility.Collapsed;
        public Visibility AssignToPanelVisibility
        {
            get { return _assignToPanelVisibility; }
            set { SetProperty(ref _assignToPanelVisibility, value); }
        }

        private string _selectedTerrManagerName;
        public string SelectedTerrManagerName
        {
            get { return _selectedTerrManagerName; }
            set { SetProperty(ref _selectedTerrManagerName, value); }
        }

        private int _assignedToTSM;
        public int AssignedToTSM
        {
            get { return _assignedToTSM; }
            set { SetProperty(ref _assignedToTSM, value); }
        }

        #endregion

        #region Command
        public ICommand OnNavigatedToCommand { get; private set; }
        public ICommand OnCheckBoxClicked { get; private set; }
        public ICommand SaveButtomCommand { get; private set; }
        public ICommand HeaderSearchTextChangedCommand { get; private set; }
        public ICommand HeaderSearchSuggestionChoosenCommand { private set; get; }

        #endregion

        #region Constructor
        public AddEditRoutePageViewModel()
        {
            RegisterCommand();
            DbCustomerDataSource = new List<CustomerPageUIModel>();
            CustomerDataSourceOnSearchItem = new List<CustomerPageUIModel>();
            CustomerList = new ObservableCollection<CustomerPageUIModel>();
            HeaderSearchItemSource = new ObservableCollection<CustomerPageUIModel>();
            TerritoryManagersCollection = new ObservableCollection<string>();
            TerritoryManagersDataList = new List<UserMaster>();
        }
        #endregion

        #region private methods
        private void RegisterCommand()
        {
            OnNavigatedToCommand = new AsyncRelayCommand<object>(OnNavigatedToCommandHandler);
            OnCheckBoxClicked = new RelayCommand<CustomerPageUIModel>(OnCheckBoxClickedHandler);
            SaveButtomCommand = new AsyncRelayCommand(SaveButtomCommandHandler);
            HeaderSearchTextChangedCommand = new RelayCommand<string>(HeaderSearchTextChangedCommandHandler);
            HeaderSearchSuggestionChoosenCommand = new RelayCommand<CustomerPageUIModel>(HeaderSearchSuggestionChoosenCommandHandler);
        }

        private async Task OnNavigatedToCommandHandler(object obj)
        {
            try
            {
                LoadUI = false;

                IsLoading = true;

                DbCustomerDataSource = await AppReference.QueryService.GetCustomerPageData(true);
                int bdRoleId = await AppReference.QueryService.GetRoleIdAsync(ApplicationConstants.BDRoleName);
                int avpRoleId = await AppReference.QueryService.GetRoleIdAsync(ApplicationConstants.AVPRoleName);

                if (AppReference.LoggedInUserRoleId == 2 || AppReference.LoggedInUserRoleId == 3
                    || AppReference.LoggedInUserRoleId == bdRoleId
                    || AppReference.LoggedInUserRoleId == avpRoleId)
                {
                    AssignToPanelVisibility = Visibility.Visible;

                    TerritoryManagersCollection.Add("None");

                    if (AppReference.LoggedInUserRoleId == 2)
                    {
                        TerritoryManagersDataList = await AppReference.QueryService.GetListOfTerritoryManagersForRegion(AppReference.LoggedInUserRegionId);
                    }
                    else if (AppReference.LoggedInUserRoleId == 3)
                    {
                        TerritoryManagersDataList = await AppReference.QueryService.GetListOfTerritoryManagersForZone(AppReference.LoggedInUserZoneId);
                    }
                    else if (AppReference.LoggedInUserRoleId == bdRoleId)
                    {
                        var loggedInUser = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
                        var BDId = loggedInUser.BDID;
                        var territories = await AppReference.QueryService.GetBDApproverTerritoriesAsync(BDId, AppReference.LoggedInUserRegionId);
                        string territoryIds = string.Join(",", territories.Select(x => x.TerritoryID));
                        TerritoryManagersDataList = await AppReference.QueryService.GetListOfTerritoryManagersForTerritories(territoryIds);
                    }
                    else if(AppReference.LoggedInUserRoleId == avpRoleId)
                    {
                        var loggedInUser = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
                        var AVPId = loggedInUser.AVPID;
                        var territories = await AppReference.QueryService.GetAVPTerritoriesAsync(AVPId);
                        string territoryIds = string.Join(",", territories.Select(x => x.TerritoryID));
                        TerritoryManagersDataList = await AppReference.QueryService.GetListOfTerritoryManagersForTerritories(territoryIds);
                    }

                    if (TerritoryManagersDataList != null && TerritoryManagersDataList.Count > 0)
                    {
                        foreach (var item in TerritoryManagersDataList)
                        {
                            var fullName = item.FirstName + " " + item.LastName;

                            TerritoryManagersCollection.Add(fullName);
                        }
                    }
                    else
                    {
                        AssignToPanelVisibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    AssignToPanelVisibility = Visibility.Collapsed;
                }

                if (obj is RouteListUIModel)
                {
                    HeaderTitle = "Edit Route";

                    var route = obj as RouteListUIModel;

                    IsEdit = true;

                    SelectedTerrManagerName = string.Empty;

                    if (route != null)
                    {
                        RouteName = route.RouteName;
                        RouteBrief = route.RouteBrief;
                        StartDate = route.RouteStartDate;
                        EndDate = route.RouteEndDate;
                        editRouteId = route.RouteId;
                        editRouteDeviceId = route.DeviceRouteId;
                        AssignedToTSM = route.idAssignToTSM;

                        if (route.idAssignToTSM != 0 && TerritoryManagersDataList != null && TerritoryManagersDataList.Count > 0)
                        {
                            var tempUserItem = TerritoryManagersDataList.FirstOrDefault(x => x.UserId == route.idAssignToTSM);

                            if (tempUserItem != null)
                            {
                                SelectedTerrManagerName = tempUserItem.FirstName + " " + tempUserItem.LastName;
                            }
                        }
                    }

                    var customerIds = await AppReference.QueryService.GetCustomerIdsFromRouteStationTableByRouteId(route.RouteId);

                    foreach (var item in DbCustomerDataSource)
                    {
                        if (customerIds.Contains(item.CustomerId))
                        {
                            item.IsSelected = true;
                            CustomerList.Insert(0, item);
                        }
                        else
                        {
                            CustomerList.Add(item);
                        }
                    }

                    LoadUI = true;

                    IsLoading = false;
                }
                else if (obj == null)
                {
                    HeaderTitle = "Create Route";

                    IsEdit = false;

                    var _tempData = DbCustomerDataSource;

                    foreach (var item in _tempData)
                    {
                        CustomerList.Add(item);
                    }

                    IsLoading = false;
                }

                LoadUI = true;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AddEditRoutePageViewModel), "OnNavigatedToCommandHandler", ex.StackTrace);
                IsLoading = false;
            }
        }

        private void HeaderSearchSuggestionChoosenCommandHandler(CustomerPageUIModel selectedItem)
        {
            if (selectedItem.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
            {
                return;
            }

            CustomerList.Clear();

            //CustomerList = new ObservableCollection<CustomerPageUIModel>() { selectedItem };

            var _filterItem = CustomerDataSourceOnSearchItem.FirstOrDefault(x => x.CustomerName.Equals(selectedItem.CustomerName));

            CustomerList.Insert(0, _filterItem);

            CustomerDataSourceOnSearchItem.Remove(_filterItem);

            CustomerDataSourceOnSearchItem.ForEach(item =>
            {
                CustomerList.Add(item);
            });

            HeaderSearchItemSource.Clear();
            CustomerDataSourceOnSearchItem.Clear();
        }

        private void HeaderSearchTextChangedCommandHandler(string text)
        {
            HeaderSearchItemSource.Clear();

            if (string.IsNullOrWhiteSpace(text))
            {
                CustomerList = new ObservableCollection<CustomerPageUIModel>(DbCustomerDataSource);
            }
            else
            {
                CustomerDataSourceOnSearchItem.Clear();

                //var tempList = DbCustomerDataSource.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();

                //if (tempList == null || tempList.Count == 0)
                //{
                //    HeaderSearchItemSource.Add(new CustomerPageUIModel() { CustomerName = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                //}
                //else
                //{
                //    tempList.ForEach(x => HeaderSearchItemSource.Add(x));
                //}

                CustomerDataSourceOnSearchItem = DbCustomerDataSource.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();

                if (CustomerDataSourceOnSearchItem == null || CustomerDataSourceOnSearchItem.Count == 0)
                {
                    HeaderSearchItemSource.Add(new CustomerPageUIModel() { CustomerName = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                }
                else
                {
                    CustomerDataSourceOnSearchItem.ForEach(x => HeaderSearchItemSource.Add(x));
                }
            }
        }

        private async Task SaveButtomCommandHandler()
        {
            try
            {
                if (await IsValidRequest())
                {
                    IsLoading = true;
                    var loggedInUser = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);

                    if (!IsEdit)
                    {
                        if (!string.IsNullOrEmpty(SelectedTerrManagerName) && !SelectedTerrManagerName.Equals("None"))
                        {
                            AssignedToTSM = TerritoryManagersDataList.FirstOrDefault(x => SelectedTerrManagerName.Contains(x.FirstName)).UserId;
                        }
                        else
                        {
                            AssignedToTSM = 0;
                        }

                        var route = new ScheduledRoutes()
                        {
                            RouteId = null,
                            RouteName = RouteName,
                            RouteBrief = RouteBrief,
                            StartDate = Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(StartDate.Value.DateTime),
                            EndDate = Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(EndDate.Value.DateTime),
                            idAssignToTSM = AssignedToTSM,
                            UserId = loggedInUser.UserId,
                            CreatedBy = loggedInUser.UserId.ToString(),
                            DeviceRouteId = Guid.NewGuid().ToString(),
                            IsExported = 0,
                            CreatedDate = Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now),
                        };

                        var addedRoute = await AppReference.QueryService.AddRouteToDb(route);

                        if (addedRoute != null)
                        {
                            var customerIds = DbCustomerDataSource.Where(x => x.IsSelected).Select(x => x.CustomerId);

                            await AppReference.QueryService.AddRouteStationsToDb(customerIds.ToList(), addedRoute.RouteId.Value, addedRoute.DeviceRouteId, loggedInUser.UserId);
                        }

                        IsLoading = false;

                        await AlertHelper.Instance.ShowConfirmationAlert("Success", "Route successfully added to your device", "OK");

                        NavigationService.GoBackInShell();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(SelectedTerrManagerName) && !SelectedTerrManagerName.Equals("None"))
                        {
                            AssignedToTSM = TerritoryManagersDataList.FirstOrDefault(x => SelectedTerrManagerName.Contains(x.FirstName)).UserId;
                        }
                        //else
                        //{
                        //    AssignedToTSM = 0;
                        //}

                        var isUpdated = await AppReference.QueryService.UpdateScheduleRoutesWhenRouteIdIs(RouteName, RouteBrief, Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(StartDate.Value.DateTime), Core.Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(EndDate.Value.DateTime), loggedInUser.UserId, AssignedToTSM, editRouteId);

                        if (isUpdated)
                        {
                            var isDeleted = await AppReference.QueryService.DeleteAllRouteStationsForRouteId(editRouteId);

                            if (isDeleted)
                            {
                                var customerIds = DbCustomerDataSource.Where(x => x.IsSelected).Select(x => x.CustomerId);
                                await AppReference.QueryService.AddRouteStationsToDb(customerIds.ToList(), editRouteId, editRouteDeviceId, loggedInUser.UserId);
                                IsLoading = false;
                                await AlertHelper.Instance.ShowConfirmationAlert("Success", "Route has been updated successfully to your device", "OK");
                                NavigationService.GoBackInShell();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AddActivityPageViewModel), "SaveButtomCommandHandler", ex.StackTrace);
            }
            IsLoading = false;
        }

        private async Task<bool> IsValidRequest()
        {
            if (string.IsNullOrWhiteSpace(RouteName))
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Error", "Please enter Route Name", "OK");
                return false;
            }
            else if (!StartDate.HasValue)
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Error", "Please select Start Date", "OK");
                return false;
            }
            else if (!EndDate.HasValue)
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Error", "Please select End Date", "OK");
                return false;
            }
            else if (StartDate > EndDate)
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Error", "End Date should be greater than Start Date.", "OK");
                return false;
            }

            var customers = DbCustomerDataSource.Where(x => x.IsSelected);

            if (customers == null || !customers.Any())
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Error", "Please select at least one customer", "OK");
                return false;
            }

            return true;
        }

        private void OnCheckBoxClickedHandler(CustomerPageUIModel customer)
        {
            if (customer != null)
            {
                var seletedCustomer = CustomerList.FirstOrDefault(x => x.CustomerId == customer.CustomerId);

                if (seletedCustomer != null)
                {
                    seletedCustomer.IsSelected = !seletedCustomer.IsSelected;

                    var index = DbCustomerDataSource.IndexOf(seletedCustomer);

                    if (index >= 0)
                    {
                        DbCustomerDataSource[index].IsSelected = seletedCustomer.IsSelected;
                    }
                }
            }
        }
        #endregion
    }
}