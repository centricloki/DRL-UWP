using DevExpress.Data;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Helpers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using System.Windows.Input;
using DRLMobile.Helpers.CustomerPageGridHelper;
using System.Diagnostics;
using DRLMobile.Views;
using DRLMobile.Core.Models.DataModels;

/// <summary>
/// Customer page view model class
/// </summary>
namespace DRLMobile.ViewModels
{
    public class CustomerPageViewModel : ObservableObject
    {
        #region Properties

        private bool isEllipsisCommandFired = false;

        public static event EventHandler<int> NavigationEventHandler;

        private List<CustomerPageUIModel> DbCustomerDataSource;


        private ObservableCollection<CustomerPageUIModel> _headerSearchItemSource;
        public ObservableCollection<CustomerPageUIModel> HeaderSearchItemSource
        {
            get { return _headerSearchItemSource; }
            set { SetProperty(ref _headerSearchItemSource, value); }
        }

        InfiniteAsyncSource _items;
        public InfiniteAsyncSource Items
        {
            get { return _items; }
            private set { SetProperty(ref _items, value); }
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

        private readonly App AppRef = ((App)Application.Current);

        #endregion

        #region commands
        public ICommand AddCustomerCommand { get; private set; }
        public ICommand OnNavigatedTo { private set; get; }
        public ICommand HeaderSearchTextChangeCommand { private set; get; }
        public ICommand HeaderSearchSuggestionChoosenCommand { private set; get; }
        public ICommand ItemSelectedCommand { private set; get; }
        public ICommand PlaceAnOderButtonCommand { get; private set; }
        public ICommand TravelPopUpCloseCommand { get; private set; }
        public ICommand EllipsisClickedCommand { get; private set; }


        #endregion

        #region constructor

        public CustomerPageViewModel()
        {
            InitSource();
            LoadingVisibilityHandler(isLoading: true);
            DbCustomerDataSource = new List<CustomerPageUIModel>();
            HeaderSearchItemSource = new ObservableCollection<CustomerPageUIModel>();
            OnNavigatedTo = new AsyncRelayCommand(LoadInitialPageData);
            HeaderSearchTextChangeCommand = new RelayCommand<string>(HandleTextChangeHeaderCommand);
            HeaderSearchSuggestionChoosenCommand = new RelayCommand<CustomerPageUIModel>(SuggestionChoosen);
            ItemSelectedCommand = new RelayCommand<CustomerPageUIModel>(CustomerSelected);
            AddCustomerCommand = new RelayCommand(AddCustomerCommandHandler);
            TravelPopUpCloseCommand = new RelayCommand<string>(TravelPopUpCloseCommandHandler);
            EllipsisClickedCommand = new RelayCommand<CustomerPageUIModel>(EllipsisClicked);
            TravelPopUpVisibility = false;
            CustomerName = string.Empty;
        }

        #endregion

        #region private methods

        void InitSource()
        {
            Items = new InfiniteAsyncSource()
            {
                ElementType = typeof(CustomerPageUIModel)
            };
            Items.FetchRows += (o, e) =>
            {
                e.Result = FetchRowsAsync(e);
            };
        }

        async Task<FetchRowsResult> FetchRowsAsync(FetchRowsAsyncEventArgs e)
        {
            CustomerSortOrder sortOrder = CustomerPageDataSourceHelper.GetCustomerSortOrder(e);
            CustomerFilter filter = CustomerPageDataSourceHelper.MakeCustomerFilter(e.Filter);
            const int pageSize = 30;
            var customerList = await CustomerFetchService.GetCustomerAsync(
                page: e.Skip / pageSize,
                pageSize: pageSize,
                sortOrder: sortOrder,
                filter: filter
                );
            return new FetchRowsResult(customerList, hasMoreRows: customerList?.Length == pageSize);
        }


        private void CustomerSelected(CustomerPageUIModel selectedCustomer)
        {
            if (!isEllipsisCommandFired)
            {
                NavigationEventHandler?.Invoke(null, selectedCustomer.CustomerId);
            }
            else
            {
                isEllipsisCommandFired = false;
            }
        }

        private void AddCustomerCommandHandler()
        {
            NavigationEventHandler?.Invoke(null, 0);
        }


        private async Task LoadInitialPageData()
        {
            try
            {
                DbCustomerDataSource = await ((App)Application.Current).QueryService.GetCustomerPageData();
                GetTopCustomer();
                CustomerFetchService.CustomerListMain = new Lazy<List<CustomerPageUIModel>>(DbCustomerDataSource);
                Items.RefreshRows();
                LoadingVisibilityHandler(isLoading: false);

                ///parallel thread
                //Parallel.ForEach(
                //    DbCustomerDataSource, async customer =>
                //     {
                //         if (customer.VripOrTravel == 1)
                //         {
                //             var isEllipsVisible = await AppRef.QueryService.GetTravelVripPromotionContactDataForCustomer(customer.CustomerId.ToString());
                //             customer.IsEllipsisVisible = isEllipsVisible;
                //         }
                //     });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                LoadingVisibilityHandler(isLoading: false);
            }
        }

        private void HandleTextChangeHeaderCommand(string text)
        {
            HeaderSearchItemSource.Clear();
            if (string.IsNullOrWhiteSpace(text))
            {
                CustomerFetchService.CustomerListMain = new Lazy<List<CustomerPageUIModel>>(DbCustomerDataSource);
                Items.RefreshRows();
            }
            else
            {
                var tempList = DbCustomerDataSource.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();
                if (tempList == null || tempList.Count == 0)
                {
                    HeaderSearchItemSource.Add(new CustomerPageUIModel() { CustomerName = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                }
                else
                {
                    tempList.ForEach(x => HeaderSearchItemSource.Add(x));
                }
            }
        }

        private void SuggestionChoosen(CustomerPageUIModel selectedItem)
        {
            if (selectedItem.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
                return;
            var tempList = new List<CustomerPageUIModel>() { selectedItem };
            CustomerFetchService.CustomerListMain = new Lazy<List<CustomerPageUIModel>>(tempList);
            Items.RefreshRows();
            HeaderSearchItemSource.Clear();
        }

        private void LoadingVisibilityHandler(bool isLoading)
        {
            if (isLoading)
            { LoadingVisibility = Visibility.Visible; }
            else
            { LoadingVisibility = Visibility.Collapsed; }
        }

        private void GetTopCustomer()
        {
            if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId))
            {
                var selectedCustomer = DbCustomerDataSource.FirstOrDefault(x => AppRef.SelectedCustomerId.Equals(x.CustomerId.ToString()));
                DbCustomerDataSource.Remove(selectedCustomer);
                DbCustomerDataSource.Insert(0, selectedCustomer);
            }
        }

        #endregion

        #region Public Methods

        public async Task AddNewlyAddedCustomer()
        {

            var states = await AppRef.QueryService.GetStateDict();
            var classifications = await AppRef.QueryService.GetClassificationDict();
            states.TryGetValue(CustomerPage.NewlyAddedCustomer.PhysicalAddressStateID, out string tempState);
            classifications.TryGetValue(int.Parse(CustomerPage.NewlyAddedCustomer.AccountClassification), out Classification tempClassification);
            CustomerPage.NewlyAddedCustomer.StateData = tempState;
            CustomerPage.NewlyAddedCustomer.ClassificationData = tempClassification;
            DbCustomerDataSource.Insert(0, CustomerPage.NewlyAddedCustomer.CopyToUIModel());
            Items.RefreshRows();
        }

        private void TravelPopUpCloseCommandHandler(string obj)
        {
            TravelPopUpVisibility = false;
        }

        private void EllipsisClicked(CustomerPageUIModel customerPageUIModel)
        {
            CustomerName = customerPageUIModel?.CustomerName;
            CustomerId = customerPageUIModel?.CustomerId.ToString();
            TravelPopUpVisibility = !TravelPopUpVisibility;
            isEllipsisCommandFired = true;
        }

        #endregion
    }
}
