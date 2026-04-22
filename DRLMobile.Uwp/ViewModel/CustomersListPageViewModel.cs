using DevExpress.Mvvm.Native;

using DRLMobile.Core.Enums;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.ViewModel
{
    public class CustomersListPageViewModel : BaseModel
    {
        #region Properties

        private readonly App AppRef = (App)Application.Current;
        private DispatcherQueueTimer _typeTimer = null;
        private bool IsEllipsisCommandFired = false;

        private string headerSearchSelectedCustomerName;
        private string headerSearchText;
        private bool headerSearchSelectedCustomerFlag = false;

        public List<CustomerPageUIModel> DbCustomerDataSource;

        private List<CustomerPageUIModel> CustomerDataSourceOnSearchItem;

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

        private Visibility _loadingVisiblity;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisiblity; }
            set { SetProperty(ref _loadingVisiblity, value); }
        }

        private bool _travelPopUpVisibility;
        public bool TravelPopUpVisibility
        {
            get { return _travelPopUpVisibility; }
            set { SetProperty(ref _travelPopUpVisibility, value); }
        }

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set { SetProperty(ref _customerName, value); }
        }

        public static string CustomerId = string.Empty;

        private bool _isRbDataVisible;
        public bool IsRbDataVisible
        {
            get { return _isRbDataVisible; }
            set { SetProperty(ref _isRbDataVisible, value); }
        }

        private bool _loadAllData;
        public bool LoadAllData
        {
            get { return _loadAllData; }
            set { SetProperty(ref _loadAllData, value); }
        }

        private string _sortCustomerNo;
        public string SortCustomerNo
        {
            get { return _sortCustomerNo; }
            set { SetProperty(ref _sortCustomerNo, value); }
        }
        private string _sortCustomerName;
        public string SortCustomerName
        {
            get { return _sortCustomerName; }
            set { SetProperty(ref _sortCustomerName, value); }
        }

        private string _sortStoreType;

        public string SortStoreType
        {
            get { return _sortStoreType; }
            set { SetProperty(ref _sortStoreType, value); }
        }

        private string _sortRank;
        public string SortRank
        {
            get { return _sortRank; }
            set { SetProperty(ref _sortRank, value); }
        }
        private string _sortAddress;
        public string SortAddress
        {
            get { return _sortAddress; }
            set { SetProperty(ref _sortAddress, value); }
        }

        private string _sortState;
        public string SortState
        {
            get { return _sortState; }
            set { SetProperty(ref _sortState, value); }
        }
        private string _sortCity;
        public string SortCity
        {
            get { return _sortCity; }
            set { SetProperty(ref _sortCity, value); }
        }
        private string _sortTerritory;
        public string SortTerritory
        {
            get { return _sortTerritory; }
            set { SetProperty(ref _sortTerritory, value); }
        }
        private string _sortCalldate;
        public string SortCalldate
        {
            get { return _sortCalldate; }
            set { SetProperty(ref _sortCalldate, value); }
        }
        private string _sortIsEllipsisVisible;
        public string SortIsEllipsisVisible
        {
            get { return _sortIsEllipsisVisible; }
            set { SetProperty(ref _sortIsEllipsisVisible, value); }
        }

        private string _filterCustomerNumber;
        public string FilterCustomerNumber
        {
            get { return _filterCustomerNumber; }
            set { SetProperty(ref _filterCustomerNumber, value); }
        }
        private string _filterCustomerName;
        public string FilterCustomerName
        {
            get { return _filterCustomerName; }
            set { SetProperty(ref _filterCustomerName, value); }
        }
        private string _filterCity;
        public string FilterCity
        {
            get { return _filterCity; }
            set { SetProperty(ref _filterCity, value); }
        }
        private string _filterState;
        public string FilterState
        {
            get { return _filterState; }
            set { SetProperty(ref _filterState, value); }
        }
        private string _filterStoreType;
        public string FilterStoreType
        {
            get { return _filterStoreType; }
            set { SetProperty(ref _filterStoreType, value); }
        }
        private string _filterRank;
        public string FilterRank
        {
            get { return _filterRank; }
            set { SetProperty(ref _filterRank, value); }
        }
        private string _filterAddress;
        public string FilterAddress
        {
            get { return _filterAddress; }
            set { SetProperty(ref _filterAddress, value); }
        }

        private string _filterTerritory;
        public string FilterTerritory
        {
            get { return _filterTerritory; }
            set { SetProperty(ref _filterTerritory, value); }
        }
        private string _filterCalldate;
        public string FilterCalldate
        {
            get { return _filterCalldate; }
            set { SetProperty(ref _filterCalldate, value); }
        }
        private int areaSelectedId = 0;

        private bool _assignedAccountAllData;
        public bool AssignedAccountAllData
        {
            get { return _assignedAccountAllData; }
            set { SetProperty(ref _assignedAccountAllData, value); }
        }
        private bool _teamMemberAllData;
        public bool TeamMemberAllData
        {
            get { return _teamMemberAllData; }
            set { SetProperty(ref _teamMemberAllData, value); }
        }
        private bool _bothAssignedTypeAllData;
        public bool BothAssignedTypeAllData
        {
            get { return _bothAssignedTypeAllData; }
            set { SetProperty(ref _bothAssignedTypeAllData, value); }
        }

        private Visibility _isAccountOwnerFilterVisible;
        public Visibility IsAccountOwnerFilterVisible
        {
            get { return _isAccountOwnerFilterVisible; }
            set { SetProperty(ref _isAccountOwnerFilterVisible, value); }
        }

        private Dictionary<string, string> colFilters = null;

        #endregion

        #region Commands
        public ICommand AddCustomerCommand { get; private set; }
        public ICommand OnNavigatedToCommand { private set; get; }
        public ICommand HeaderSearchTextChangeCommand { private set; get; }
        public ICommand HeaderSearchSuggestionChoosenCommand { private set; get; }
        public ICommand ItemSelectedCommand { private set; get; }
        public ICommand PlaceAnOderButtonCommand { get; private set; }
        public ICommand TravelPopUpCloseCommand { get; private set; }
        public ICommand EllipsisClickedCommand { get; private set; }
        public ICommand rbCustomerDataChangedCommand { get; private set; }
        public ICommand SortColumnClickCommand { get; private set; }
        public ICommand FilterColumnClickCommand { get; private set; }
        public IAsyncRelayCommand rbAssignmentTypeChangedCommand { get; private set; }
        #endregion

        #region Constructor
        public CustomersListPageViewModel()
        {
            DbCustomerDataSource = new List<CustomerPageUIModel>();
            CustomerDataSourceOnSearchItem = new List<CustomerPageUIModel>();
            CustomerList = new ObservableCollection<CustomerPageUIModel>();
            HeaderSearchItemSource = new ObservableCollection<CustomerPageUIModel>();

            RegisterCommand();

            TravelPopUpVisibility = false;
            CustomerName = string.Empty;            
        }
        #endregion

        #region Private Methods
        private void RegisterCommand()
        {            
            OnNavigatedToCommand = new AsyncRelayCommand<object>(OnNavigatedToCommandHandler);
            HeaderSearchTextChangeCommand = new RelayCommand<string>(HeaderSearchTextChangedCommandHandler);
            HeaderSearchSuggestionChoosenCommand = new RelayCommand<CustomerPageUIModel>(HeaderSearchSuggestionChoosenCommandHandler);
            AddCustomerCommand = new RelayCommand(AddCustomerCommandHandler);
            ItemSelectedCommand = new RelayCommand<CustomerPageUIModel>(CustomerSelectedCommandHandler);
            EllipsisClickedCommand = new RelayCommand<CustomerPageUIModel>(EllipsisClickedCommandHandler);
            TravelPopUpCloseCommand = new RelayCommand<string>(TravelPopUpCloseCommandHandler);
            rbCustomerDataChangedCommand = new AsyncRelayCommand<object>(rbCustomerDataChangedCommandHandler);
            SortColumnClickCommand = new AsyncRelayCommand<string>(SortColumnClickCommandHandler);
            FilterColumnClickCommand = new RelayCommand<string>(FilterColumnClickCommandHandler);
            rbAssignmentTypeChangedCommand = new AsyncRelayCommand<object>(rbAssignmentTypeChangedCommandHandler);
        }

        private async Task OnNavigatedToCommandHandler(object arg)
        {
            _typeTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
            ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = false;
            }

            LoadAllData = true;
            IsRbDataVisible = true;
            try
            {
                await LoadCustomerGrid();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(CustomersListPageViewModel), nameof(OnNavigatedToCommandHandler), ex);
            }
            finally
            {
                if (shellPage != null)
                {
                    shellPage.ViewModel.IsSideMenuItemClickable = true;
                }
            }
        }

        private void HeaderSearchTextChangedCommandHandler(string SearchText)
        {
            // Only executes this code after 1 seconds have elapsed since last trigger.
            _typeTimer.Debounce(async () =>
            {
                HeaderSearchItemSource.Clear();

                if (!headerSearchSelectedCustomerFlag)
                { headerSearchText = SearchText; }
                headerSearchSelectedCustomerFlag = false;

                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    headerSearchSelectedCustomerName = "";
                    headerSearchText = "";
                    if (DbCustomerDataSource != null && DbCustomerDataSource.Count > 0)
                        await LoadAllCustomerGridAsync();
                }
                else
                {

                    CustomerDataSourceOnSearchItem.Clear();
                    if (DbCustomerDataSource != null && DbCustomerDataSource.Count > 0)
                    {
                        CustomerDataSourceOnSearchItem = DbCustomerDataSource?.Where(x => x.SearchDisplayPath.ToLower().Contains(SearchText.ToLower())).ToList();

                        if (CustomerDataSourceOnSearchItem == null || CustomerDataSourceOnSearchItem.Count == 0)
                        {
                            HeaderSearchItemSource.Add(new CustomerPageUIModel() { CustomerName = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                        }
                        else
                        {
                            CustomerDataSourceOnSearchItem.ForEach(item =>
                            {
                                HeaderSearchItemSource.Add(item);
                            });
                        }
                    }
                }
            }, TimeSpan.FromSeconds(1));
        }

        private void LoadDataGridAndHeaderSearchWithInitialData()
        {
            LoadingVisibilityHandler(isLoading: true);

            //await Task.Delay(100);

            CustomerList.Clear();

            DbCustomerDataSource.ForEach(item =>
            {
                CustomerList.Add(item);
            });

            HeaderSearchItemSource.Clear();

            LoadingVisibilityHandler(isLoading: false);
        }

        private void LoadHeaderSearchWithInitialData()
        {
            DbCustomerDataSource.ForEach(item =>
            {
                HeaderSearchItemSource.Add(item);
            });
        }

        private async void HeaderSearchSuggestionChoosenCommandHandler(CustomerPageUIModel selectedItem)
        {
            if (selectedItem.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
            {
                return;
            }

            CustomerList.Clear();
            headerSearchSelectedCustomerName = selectedItem.CustomerName;
            headerSearchSelectedCustomerFlag = true;
            await LoadAllCustomerGridAsync();
        }

        private void AddCustomerCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(AddCustomerPage));
        }

        private void CustomerSelectedCommandHandler(CustomerPageUIModel selectedCustomer)
        {
            NavigationService.NavigateShellFrame(typeof(CustomerDetailsPage), selectedCustomer.CustomerId);
        }

        private void EllipsisClickedCommandHandler(CustomerPageUIModel customerPageUIModel)
        {
            CustomerName = customerPageUIModel?.CustomerName;

            CustomerId = customerPageUIModel?.CustomerId.ToString();
            bool isellipseflag = customerPageUIModel.IsEllipsisVisible;
            if (isellipseflag)
            {
                TravelPopUpVisibility = !TravelPopUpVisibility;
            }
            else
            {
                NavigationService.NavigateShellFrame(typeof(CustomerDetailsPage), CustomerId);
            }

        }
        private void TravelPopUpCloseCommandHandler(string obj)
        {
            TravelPopUpVisibility = false;
        }

        private void LoadingVisibilityHandler(bool isLoading)
        {
            if (isLoading)
            {
                LoadingVisibility = Visibility.Visible;
            }
            else
            {
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private async Task GetUpdateCustomersAsync(int areaId, int roleId)
        {
            if (!string.IsNullOrWhiteSpace(AppRef.PreviousSyncDateTimeProperty))
            {
                List<CustomerPageUIModel> dbCustomers =
                   await AppRef.QueryService.GetUpdatedCustomers(AppRef.PreviousSyncDateTimeProperty, areaId, roleId);
                if (dbCustomers != null && dbCustomers.Count > 0)
                {
                    CustomerPageUIModel uIModel;
                    foreach (var cust in dbCustomers)
                    {
                        uIModel = DbCustomerDataSource.FirstOrDefault(x => { if (x != null) return x.DeviceCustomerId == cust.DeviceCustomerId; else return false; });
                        if (uIModel != null)
                            DbCustomerDataSource.Remove(uIModel);

                        DbCustomerDataSource.Add(cust);
                    }
                }
            }
        }

        private async Task CleanDeletedCustomersAsync()
        {
            List<CustomerPageModel> dbCustomers = await AppRef.QueryService.CleanDeletedCustomers();
            if (dbCustomers != null && dbCustomers.Count > 0)
            {
                CustomerPageUIModel uIModel;
                foreach (var cust in dbCustomers)
                {
                    uIModel = DbCustomerDataSource.FirstOrDefault(x => cust.DeviceCustomerID == x.DeviceCustomerId);
                    if (uIModel != null)
                        DbCustomerDataSource.Remove(uIModel);
                }
            }
        }

        private void GetTopCustomer()
        {
            if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId))
            {
                var selectedCustomer = CustomerList.FirstOrDefault(x => AppRef.SelectedCustomerId.Equals(x.CustomerId.ToString()));
                if (selectedCustomer != null)
                {
                    CustomerList.Remove(selectedCustomer);
                    CustomerList.Insert(0, selectedCustomer);
                }
            }

        }

        private async Task rbCustomerDataChangedCommandHandler(object obj)
        {
            var rbLoadData = obj as RadioButton;
            if (rbLoadData.Name == "rbAlldata" && rbLoadData.IsChecked == true)
            {
                LoadAllData = true;
            }
            else
                LoadAllData = false;

            
            DbCustomerDataSource.Clear();
            await LoadCustomerGrid();
        }
        private async Task rbAssignmentTypeChangedCommandHandler(object obj)
        {
            var rbAssignmentType = obj as RadioButton;
            AssignedAccountAllData = false;
            TeamMemberAllData = false;
            BothAssignedTypeAllData = false;
            if (rbAssignmentType.Name == "rbAssignedAccount" && rbAssignmentType.IsChecked == true)
                AssignedAccountAllData = true;
            else if (rbAssignmentType.Name == "rbTeamMember" && rbAssignmentType.IsChecked == true)
                TeamMemberAllData = true;
            else if (rbAssignmentType.Name == "rbBothAssignment" && rbAssignmentType.IsChecked == true)
                BothAssignedTypeAllData = true;

            DbCustomerDataSource.Clear();
            await LoadCustomerGrid();
        }

        private async Task LoadCustomerGrid()
        {
            LoadingVisibilityHandler(true);

            if (AppRef.LoggedInUserRoleId == 6 || AppRef.LoggedInUserRoleId == 17) IsAccountOwnerFilterVisible = Visibility.Collapsed;
            else IsAccountOwnerFilterVisible = Visibility.Visible;

            CustomersListPage.EditedCustomer = null;

            await CleanDeletedCustomersAsync();

            bool isReloadRequired = false;
            int avpRoleId = await AppRef.QueryService.GetRoleIdAsync(ApplicationConstants.AVPRoleName);
            if ((new int[] { 3, 6, 17, avpRoleId }).Contains(AppRef.LoggedInUserRoleId))
            {
                if (areaSelectedId != AppRef.AreaUserSelectedId)
                {
                    areaSelectedId = AppRef.AreaUserSelectedId;
                    isReloadRequired = true;
                }
            }

            ApplyColumnFilter();
            if (DbCustomerDataSource.Count == 0 || colFilters?.Count() == 0) isReloadRequired = true;

            if (isReloadRequired)
            {
                DbCustomerDataSource.Clear();

                CustomerTeamType teamType = CustomerTeamType.Both;
                if (AssignedAccountAllData)
                {
                    teamType = CustomerTeamType.AssignedAccount;
                }
                else if (TeamMemberAllData)
                {
                    teamType = CustomerTeamType.TeamMember;
                }
                else
                {
                    teamType = CustomerTeamType.Both;
                }

                DbCustomerDataSource.AddRange(await AppRef.QueryService.GetCustomerPageData(AppRef.AreaUserSelectedId, AppRef.LoggedInUserRoleId, LoadAllData, AppRef.LoginUserIdProperty, teamType, colFilters));
            }
            else
            {
                await GetUpdateCustomersAsync(AppRef.AreaUserSelectedId, AppRef.LoggedInUserRoleId);
            }
            await LoadAllCustomerGridAsync();
            GetTopCustomer();
            LoadingVisibilityHandler(false);
        }


        private async Task SortColumnClickCommandHandler(string args)
        {
            string colName = args.Split('|')[0];
            string textValue = args.Split('|')[1];
            ClearSortColOrder();
            switch (colName)
            {
                case "customernumber":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortCustomerNo = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortCustomerNo = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortCustomerNo = "";
                    }
                    break;
                case "customername":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortCustomerName = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortCustomerName = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortCustomerName = "";
                    }
                    break;
                case "city":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortCity = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortCity = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortCity = "";
                    }
                    break;
                case "state":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortState = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortState = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortState = "";
                    }
                    break;
                case "storetype":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortStoreType = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortStoreType = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortStoreType = "";
                    }
                    break;
                case "rank":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortRank = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortRank = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortRank = "";
                    }
                    break;
                case "address":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortAddress = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortAddress = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortAddress = "";
                    }
                    break;
                case "territory":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortTerritory = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortTerritory = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortTerritory = "";
                    }
                    break;
                case "calldate":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortCalldate = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortCalldate = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortCalldate = "";
                    }
                    break;
                case "isellipse":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortIsEllipsisVisible = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortIsEllipsisVisible = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortIsEllipsisVisible = "";
                    }
                    break;
                default:
                    break;
            }
            await LoadAllCustomerGridAsync();
        }

        private void FilterColumnClickCommandHandler(string args)
        {
            _typeTimer.Debounce(async () =>
            {
                // Only executes this code after 2 seconds have elapsed since last trigger.
                string colName = args.Split('|')[0];
                string textValue = args.Split('|')[1];
                textValue = textValue?.Trim();
                //await LoadAllCustomerGridAsync();
                ApplyColumnFilter();
                DbCustomerDataSource.Clear();
                await LoadCustomerGrid();
            }, TimeSpan.FromSeconds(1));
        }
        private void ApplyColumnFilter()
        {
            if (!string.IsNullOrWhiteSpace(_filterCustomerNumber))
            {
                AddColumnFilter("CustomerNumber", _filterCustomerNumber);
            }
            else
            {
                RemoveColumnFilter("CustomerNumber");
            }

            if (!string.IsNullOrWhiteSpace(_filterCustomerName))
            {
                AddColumnFilter("CustomerName", _filterCustomerName);
            }
            else { RemoveColumnFilter("CustomerName"); }

            if (!string.IsNullOrWhiteSpace(_filterCity))
            {
                AddColumnFilter("City", _filterCity);
            }
            else { RemoveColumnFilter("City"); }

            if (!string.IsNullOrWhiteSpace(_filterState))
            {
                AddColumnFilter("State", _filterState);
            }
            else { RemoveColumnFilter("State"); }

            if (!string.IsNullOrWhiteSpace(_filterStoreType))
            {
                AddColumnFilter("StoreType", _filterStoreType);
            }
            else { RemoveColumnFilter("StoreType"); }

            if (!string.IsNullOrWhiteSpace(_filterRank))
            {
                AddColumnFilter("Rank", _filterRank);
            }
            else { RemoveColumnFilter("Rank"); }

            if (!string.IsNullOrWhiteSpace(_filterAddress))
            {
                AddColumnFilter("Address", _filterAddress);
            }
            else { RemoveColumnFilter("Address"); }

            if (!string.IsNullOrWhiteSpace(_filterTerritory))
            {
                AddColumnFilter("Territory", _filterTerritory);
            }
            else { RemoveColumnFilter("Territory"); }

            if (!string.IsNullOrWhiteSpace(_filterCalldate))
            {
                AddColumnFilter("Calldate", _filterCalldate);
            }
            else
            {
                RemoveColumnFilter("Calldate");
            }

        }
        private void RemoveColumnFilter(string colName)
        {
            if (colFilters != null && colFilters.ContainsKey(colName))
            {
                colFilters.Remove(colName);
            }
        }

        private void ApplySortToAllCustomerGrid(ref IEnumerable<CustomerPageUIModel> allCustomerList)
        {
            if (!string.IsNullOrWhiteSpace(SortCustomerNo))
            {
                if (SortCustomerNo == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.CustomerNumber);
                }
                else if (SortCustomerNo == "sort_up")
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.CustomerNumber);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortCustomerName))
            {
                if (SortCustomerName == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.CustomerName);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.CustomerName);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortCity))
            {
                if (SortCity == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.City);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.City);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortState))
            {
                if (SortState == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.State);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.State);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortStoreType))
            {
                if (SortStoreType == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.StoreType);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.StoreType);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortRank))
            {
                if (SortRank == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.Rank);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.Rank);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortAddress))
            {
                if (SortAddress == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.Address);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.Address);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortTerritory))
            {
                if (SortTerritory == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.TerritoryName);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.TerritoryName);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortCalldate))
            {
                if (SortCalldate == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.CallDate);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.CallDate);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortIsEllipsisVisible))
            {
                if (SortIsEllipsisVisible == "sort_down")
                {
                    allCustomerList = allCustomerList.OrderByDescending(x => x.IsEllipsisVisible);
                }
                else
                {
                    allCustomerList = allCustomerList.OrderBy(x => x.IsEllipsisVisible);
                }
            }
        }

        private void AddColumnFilter(string colName, string filterValue)
        {
            if (colFilters == null)
            {
                colFilters = new Dictionary<string, string>();
            }
            if (colFilters.ContainsKey(colName))
            {
                colFilters[colName] = filterValue;
            }
            else
            {
                colFilters.Add(colName, filterValue);
            }
        }

        private List<CustomerPageUIModel> ApplyFiltersToAllCustomerGrid(CustomerPageUIModel[] allCustomerArray)
        {
            return allCustomerArray.Where(x =>
            {
                bool isMatched = true;
                if (x != null)
                {
                    if (!string.IsNullOrWhiteSpace(_filterCustomerNumber))
                    {
                        if (!string.IsNullOrWhiteSpace(x.CustomerNumber))
                        {
                            isMatched &= x.CustomerNumber.Contains(_filterCustomerNumber, StringComparison.OrdinalIgnoreCase);
                            AddColumnFilter("CustomerNumber", _filterCustomerNumber);
                        }
                        else isMatched &= false;
                    }
                    else
                    {
                        isMatched &= true;
                        RemoveColumnFilter("CustomerNumber");
                    }

                    if (!string.IsNullOrWhiteSpace(_filterCustomerName))
                    {
                        if (!string.IsNullOrWhiteSpace(x.CustomerName))
                        {
                            isMatched &= x.CustomerName.Contains(_filterCustomerName, StringComparison.OrdinalIgnoreCase);
                            AddColumnFilter("CustomerName", _filterCustomerName);
                        }
                        else isMatched &= false;
                    }
                    else { isMatched &= true; RemoveColumnFilter("CustomerName"); }

                    if (!string.IsNullOrWhiteSpace(_filterCity))
                    {
                        if (!string.IsNullOrWhiteSpace(x.City))
                        {
                            isMatched &= x.City.Contains(_filterCity, StringComparison.OrdinalIgnoreCase);
                            AddColumnFilter("City", _filterCity);
                        }
                        else isMatched &= false;
                    }
                    else { isMatched &= true; RemoveColumnFilter("City"); }

                    if (!string.IsNullOrWhiteSpace(_filterState))
                    {
                        if (!string.IsNullOrWhiteSpace(x.State))
                        {
                            isMatched &= x.State.Contains(_filterState, StringComparison.OrdinalIgnoreCase);
                            AddColumnFilter("State", _filterState);
                        }
                        else isMatched &= false;
                    }
                    else { isMatched &= true; RemoveColumnFilter("State"); }

                    if (!string.IsNullOrWhiteSpace(_filterStoreType))
                    {
                        if (!string.IsNullOrWhiteSpace(x.StoreType))
                        {
                            isMatched &= x.StoreType.Contains(_filterStoreType, StringComparison.OrdinalIgnoreCase);
                            AddColumnFilter("StoreType", _filterStoreType);
                        }
                        else isMatched &= false;
                    }
                    else { isMatched &= true; RemoveColumnFilter("StoreType"); }

                    if (!string.IsNullOrWhiteSpace(_filterRank))
                    {
                        if (!string.IsNullOrWhiteSpace(x.Rank))
                        {
                            isMatched &= x.Rank.Contains(_filterRank, StringComparison.OrdinalIgnoreCase);
                            AddColumnFilter("Rank", _filterRank);
                        }
                        else isMatched &= false;
                    }
                    else { isMatched &= true; RemoveColumnFilter("Rank"); }

                    if (!string.IsNullOrWhiteSpace(_filterAddress))
                    {
                        if (!string.IsNullOrWhiteSpace(x.Address))
                        {
                            isMatched &= x.Address.Contains(_filterAddress, StringComparison.OrdinalIgnoreCase);
                            AddColumnFilter("Address", _filterAddress);
                        }
                        else isMatched &= false;
                    }
                    else { isMatched &= true; RemoveColumnFilter("Address"); }

                    if (!string.IsNullOrWhiteSpace(_filterTerritory))
                    {
                        if (!string.IsNullOrWhiteSpace(x.TerritoryName))
                        {
                            isMatched &= x.TerritoryName.Contains(_filterTerritory, StringComparison.OrdinalIgnoreCase);
                            AddColumnFilter("Territory", _filterTerritory);
                        }
                        else isMatched &= false;
                    }
                    else { isMatched &= true; RemoveColumnFilter("Territory"); }

                    if (!string.IsNullOrWhiteSpace(_filterCalldate))
                    {
                        //_filterCalldate = _filterCalldate.Replace('-', '/');
                        if (!string.IsNullOrWhiteSpace(x.LastCallDate))
                        {
                            isMatched &= x.CallDate.Value.ToString("MM-dd-yyyy").Contains(_filterCalldate, StringComparison.OrdinalIgnoreCase);
                            AddColumnFilter("Calldate", _filterCalldate);
                        }
                        else isMatched &= false;
                    }
                    else { isMatched &= true; RemoveColumnFilter("Calldate"); }
                }
                return isMatched;
            }).ToList();
        }
        private async Task LoadAllCustomerGridAsync()
        {
            LoadingVisibilityHandler(true);

            IEnumerable<CustomerPageUIModel> allCustomerEnumerable = null;
            List<CustomerPageUIModel> allCustomerList = null;
            if (!string.IsNullOrWhiteSpace(headerSearchSelectedCustomerName))
            {
                allCustomerList = DbCustomerDataSource.Where(x => x.CustomerName.Equals(headerSearchSelectedCustomerName)).ToList();
                if (!string.IsNullOrWhiteSpace(headerSearchText))
                {
                    List<CustomerPageUIModel> allCustomerList2 = DbCustomerDataSource.Where(x => x.CustomerName.Contains(headerSearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                    foreach (var itm in allCustomerList)
                    { allCustomerList2.Remove(itm); }

                    foreach (var itm in allCustomerList2)
                    { allCustomerList.Add(itm); }
                }
            }

            else { allCustomerList = DbCustomerDataSource; }

            CustomerPageUIModel[] allCustomerArray = new CustomerPageUIModel[allCustomerList.Count];

            allCustomerList.CopyTo(allCustomerArray, 0);
            this.CustomerList.Clear();

            await Task.Run(() =>
            {
                allCustomerEnumerable = ApplyFiltersToAllCustomerGrid(allCustomerArray);
                ApplySortToAllCustomerGrid(ref allCustomerEnumerable);
            });
            this.CustomerList = new ObservableCollection<CustomerPageUIModel>(allCustomerEnumerable.ToList());

            //GetTopCustomer();
            LoadingVisibilityHandler(false);
        }

        private void ClearFitlerColSearchKeyWords()
        {
            FilterCustomerNumber = FilterCustomerName = FilterCity = FilterState = FilterStoreType = FilterRank = FilterAddress = FilterTerritory = FilterCalldate = "";
        }
        private void ClearSortColOrder()
        {
            SortCustomerNo = SortCustomerName = SortCity = SortState = SortStoreType = SortRank = SortAddress = SortTerritory = SortCalldate = SortIsEllipsisVisible = "";
        }

        #endregion

        #region Public Methods
        public async Task AddNewlyAddedCustomerToMainList()
        {
            _typeTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
            if (!CustomersListPage.NewlyAddedCustomer.AccountClassification.Equals("38"))
            {
                var states = await AppRef.QueryService.GetStateDict();

                var classifications = await AppRef.QueryService.GetClassificationDict();

                states.TryGetValue(CustomersListPage.NewlyAddedCustomer.PhysicalAddressStateID, out string tempState);

                classifications.TryGetValue(int.Parse(CustomersListPage.NewlyAddedCustomer.AccountClassification), out Classification tempClassification);

                CustomersListPage.NewlyAddedCustomer.StateData = tempState;
                CustomersListPage.NewlyAddedCustomer.ClassificationData = tempClassification;

                DbCustomerDataSource.Insert(0, CustomersListPage.NewlyAddedCustomer.CopyToUIModel());

                CustomerList.Clear();

                if (DbCustomerDataSource != null && DbCustomerDataSource.Count > 0)
                {
                    DbCustomerDataSource.ForEach(item => CustomerList.Add(item));
                }
            }
            await LoadAllCustomerGridAsync();
        }
        public async Task EditAlreadyPesentCustomer()
        {
            _typeTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
            var customerToUpdateInMainDbSource = DbCustomerDataSource.FirstOrDefault(x => x.CustomerId == CustomersListPage.EditedCustomer.CustomerId);

            if (customerToUpdateInMainDbSource != null)
            {
                var index = DbCustomerDataSource.IndexOf(customerToUpdateInMainDbSource);

                DbCustomerDataSource.RemoveAt(index);

                if (!string.IsNullOrEmpty(CustomersListPage.EditedCustomer.StoreType) && !CustomersListPage.EditedCustomer.StoreType.Equals("Out of business"))
                {
                    DbCustomerDataSource.Insert(index, CustomersListPage.EditedCustomer);
                }

                CustomerList.Clear();

                if (DbCustomerDataSource != null && DbCustomerDataSource.Count > 0)
                {
                    DbCustomerDataSource.ForEach(item => CustomerList.Add(item));
                }
            }
            await LoadAllCustomerGridAsync();
        }

        public void DispatcherQueueTimerCleanup()
        {
            _typeTimer?.Stop();
            _typeTimer = null;
        }
        #endregion
    }
}
