using DRLMobile.Core.Models;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.Helpers;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace DRLMobile.Uwp.ViewModel
{
    public class TravelVripPromotionContractPageViewModel : BaseModel
    {
        #region Properties
        private Visibility _isTravelGridVisible;
        public Visibility IsTravelGridVisible
        {
            get { return _isTravelGridVisible; }
            set { SetProperty(ref _isTravelGridVisible, value); }
        }
        private Visibility _isPromotionGridVisible;
        public Visibility IsPromotionGridVisible
        {
            get { return _isPromotionGridVisible; }
            set { SetProperty(ref _isPromotionGridVisible, value); }
        }
        private Visibility _isContractGridVisible;
        public Visibility IsContractGridVisible
        {
            get { return _isContractGridVisible; }
            set { SetProperty(ref _isContractGridVisible, value); }
        }
        private bool _isContractGridLoaded;
        public bool IsContractGridLoaded
        {
            get { return _isContractGridLoaded; }
            set { SetProperty(ref _isContractGridLoaded, value); }
        }
        private bool _isPromotionGridLoaded;
        public bool IsPromotionGridLoaded
        {
            get { return _isPromotionGridLoaded; }
            set { SetProperty(ref _isPromotionGridLoaded, value); }
        }
        private bool _isTravelVripGridLoaded;
        public bool IsTravelVripGridLoaded
        {
            get { return _isTravelVripGridLoaded; }
            set { SetProperty(ref _isTravelVripGridLoaded, value); }
        }
        private Visibility _isSearchBoxVisible;
        public Visibility IsSearchBoxVisible
        {
            get { return _isSearchBoxVisible; }
            set { SetProperty(ref _isSearchBoxVisible, value); }
        }
        private ObservableCollection<PromotionUiModel> _promotionGridDataSource;
        public ObservableCollection<PromotionUiModel> PromotionGridDataSource
        {
            get { return _promotionGridDataSource; }
            set { SetProperty(ref _promotionGridDataSource, value); }
        }
        private ObservableCollection<ContractUiModel> _contractGridDataSource;
        public ObservableCollection<ContractUiModel> ContractGridDataSource
        {
            get { return _contractGridDataSource; }
            set { SetProperty(ref _contractGridDataSource, value); }
        }
        private List<PromotionUiModel> _dbPromotionDataSource;
        public List<PromotionUiModel> DbPromotionDataSource
        {
            get { return _dbPromotionDataSource; }
            set { SetProperty(ref _dbPromotionDataSource, value); }
        }
        private List<ContractUiModel> _dbContractDataSource;
        public List<ContractUiModel> DbContractDataSource
        {
            get { return _dbContractDataSource; }
            set { SetProperty(ref _dbContractDataSource, value); }
        }
        private TravelUiModel _travelPageUiModel;
        public TravelUiModel TravelPageUiModel
        {
            get { return _travelPageUiModel; }
            set { SetProperty(ref _travelPageUiModel, value); }
        }
        private VripUiModel _vripPageUiModel;
        public VripUiModel VripPageUiModel
        {
            get { return _vripPageUiModel; }
            set { SetProperty(ref _vripPageUiModel, value); }
        }
        private Visibility _loadingVisiblity;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisiblity; }
            set { SetProperty(ref _loadingVisiblity, value); }
        }
        private ObservableCollection<object> _headerSearchItemSource;
        public ObservableCollection<object> HeaderSearchItemSource
        {
            get { return _headerSearchItemSource; }
            set { SetProperty(ref _headerSearchItemSource, value); }
        }
        private string _autoSuggestionText;
        public string AutoSuggestionText
        {
            get { return _autoSuggestionText; }
            set { SetProperty(ref _autoSuggestionText, value); }
        }
        #endregion
        #region Commands
        public ICommand LeftSegmentButtonCommand { get; private set; }
        public ICommand CenterSegmentButtonCommand { get; private set; }
        public ICommand RightSegmentButtonCommand { get; private set; }
        public ICommand LoadTravelDataCommand { get; private set; }
        public ICommand HeaderSearchTextChangeCommand { private set; get; }
        public ICommand HeaderSearchSuggestionChoosenCommand { private set; get; }
        #endregion
        #region Constructor
        public TravelVripPromotionContractPageViewModel()
        {

            LeftSegmentButtonCommand = new RelayCommand(LeftSegmentClicked);
            CenterSegmentButtonCommand = new AsyncRelayCommand(CenterSegmentClicked);
            RightSegmentButtonCommand = new AsyncRelayCommand(RightSegmentClicked);
            LoadTravelDataCommand = new AsyncRelayCommand(GetTravelVripData);
            IsSearchBoxVisible = Visibility.Collapsed;
            PromotionGridDataSource = new ObservableCollection<PromotionUiModel>();
            DbPromotionDataSource = new List<PromotionUiModel>();
            ContractGridDataSource = new ObservableCollection<ContractUiModel>();
            DbContractDataSource = new List<ContractUiModel>();
            IsTravelGridVisible = Visibility.Visible;
            IsPromotionGridVisible = Visibility.Collapsed;
            IsContractGridVisible = Visibility.Collapsed;
            IsSearchBoxVisible = Visibility.Collapsed;
            TravelPageUiModel = new TravelUiModel();
            VripPageUiModel = new VripUiModel();
            IsContractGridLoaded = false;
            IsPromotionGridLoaded = false;
            IsTravelVripGridLoaded = false;
            LoadingVisibility = Visibility.Collapsed;
            HeaderSearchItemSource = new ObservableCollection<object>();
            HeaderSearchTextChangeCommand = new RelayCommand<string>(HandleTextChangeHeaderCommand);
            HeaderSearchSuggestionChoosenCommand = new RelayCommand<object>(SuggestionChoosen);
        }
        #endregion
        #region Private Methods
        private async Task GetTravelVripData()
        {
            SetDefaultView();
            IsTravelVripGridLoaded = true;
            TravelPageUiModel = await ((App)Application.Current).QueryService.GetTravelDataForCustomer(CustomersListPageViewModel.CustomerId);
            VripPageUiModel = await ((App)Application.Current).QueryService.GetVripDataForCustomer(CustomersListPageViewModel.CustomerId);

        }

        private void SetDefaultView()
        {
            PromotionGridDataSource.Clear();
            ContractGridDataSource.Clear();
            DbContractDataSource.Clear();
            DbPromotionDataSource.Clear();
            IsContractGridLoaded = false;
            IsPromotionGridLoaded = false;
            HeaderSearchItemSource.Clear();
            AutoSuggestionText = string.Empty;
        }

        private void LeftSegmentClicked()
        {
            HeaderSearchItemSource = new ObservableCollection<object>();
            AutoSuggestionText = string.Empty;
            IsTravelVripGridLoaded = true;
            IsTravelGridVisible = Visibility.Visible;
            IsPromotionGridVisible = Visibility.Collapsed;
            IsContractGridVisible = Visibility.Collapsed;
            IsSearchBoxVisible = Visibility.Collapsed;

        }
        private async Task CenterSegmentClicked()
        {
            HeaderSearchItemSource = new ObservableCollection<object>();
            AutoSuggestionText = string.Empty;
            IsPromotionGridLoaded = true;
            IsTravelGridVisible = Visibility.Collapsed;
            IsPromotionGridVisible = Visibility.Visible;
            IsContractGridVisible = Visibility.Collapsed;
            IsSearchBoxVisible = Visibility.Visible;
            PromotionGridDataSource.Clear();

            DbPromotionDataSource.Clear();
            LoadingVisibility = Visibility.Visible;
            await FetchPromotionData();
            LoadingVisibility = Visibility.Collapsed;
        }

        private async Task RightSegmentClicked()
        {
            HeaderSearchItemSource = new ObservableCollection<object>();
            AutoSuggestionText = string.Empty;
            IsContractGridLoaded = true;
            IsTravelGridVisible = Visibility.Collapsed;
            IsPromotionGridVisible = Visibility.Collapsed;
            IsContractGridVisible = Visibility.Visible;
            IsSearchBoxVisible = Visibility.Visible;
            ContractGridDataSource.Clear();
            DbContractDataSource.Clear();
            LoadingVisibility = Visibility.Visible;
            await FetchContractData();
            LoadingVisibility = Visibility.Collapsed;
        }
        private async Task FetchPromotionData()
        {

            DbPromotionDataSource = await ((App)Application.Current).QueryService.GetPromotionDataForCustomer(CustomersListPageViewModel.CustomerId);
            if (DbPromotionDataSource?.Count > 0)
            {
                DbPromotionDataSource.ForEach(x => PromotionGridDataSource.Add(x));
            }

        }
        private async Task FetchContractData()
        {
            DbContractDataSource = await ((App)Application.Current).QueryService.GetContractsDataForCustomer(CustomersListPageViewModel.CustomerId);
            if (DbContractDataSource?.Count > 0)
            {
                DbContractDataSource.ForEach(x => ContractGridDataSource.Add(x));
            }

        }
        private void SuggestionChoosen(object selectedItem)
        {
            if (selectedItem is PromotionUiModel)
            {
                var promotionUiModel = selectedItem as PromotionUiModel;
                if (promotionUiModel.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
                {
                    return;
                }
                HeaderSearchItemSource.Clear();
                PromotionGridDataSource.Clear();
                var _filterItem = DbPromotionDataSource.FirstOrDefault(x => x.PromotionID.Equals(promotionUiModel.PromotionID));
                PromotionGridDataSource.Add(_filterItem);
            }
            else if (selectedItem is ContractUiModel)
            {
                var contractUiModel = selectedItem as ContractUiModel;
                if (contractUiModel.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
                {
                    return;
                }
                HeaderSearchItemSource.Clear();
                ContractGridDataSource.Clear();
                var _filterItem = DbContractDataSource.FirstOrDefault(x => x.ContractID.Equals(contractUiModel.ContractID));
                ContractGridDataSource.Add(_filterItem);
            }

        }

        private void HandleTextChangeHeaderCommand(string text)
        {
            if (IsPromotionGridVisible == Visibility.Visible)
            {
                HandlePromotionSearchBox(text);
            }
            else
            {
                HandleContractSearchBox(text);
            }
        }
        private void HandlePromotionSearchBox(string text)
        {
            HeaderSearchItemSource.Clear();

            if (string.IsNullOrWhiteSpace(text))
            {
                var ifDataGridHasAlreadyData = DbPromotionDataSource?.Count != PromotionGridDataSource?.Count;
                if (ifDataGridHasAlreadyData)
                {
                    PromotionGridDataSource = new ObservableCollection<PromotionUiModel>(DbPromotionDataSource);
                }
            }
            else
            {
                var tempList = DbPromotionDataSource?.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();
                if (tempList == null || tempList.Count == 0)
                {
                    HeaderSearchItemSource.Add(new PromotionUiModel() { PromotionPlanType = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                }
                else
                {
                    tempList.ForEach(x => HeaderSearchItemSource.Add(x));
                }
            }
        }
        private void HandleContractSearchBox(string text)
        {
            HeaderSearchItemSource.Clear();

            if (string.IsNullOrWhiteSpace(text))
            {
                var ifDataGridHasAlreadyData = DbContractDataSource?.Count != ContractGridDataSource?.Count;
                if (ifDataGridHasAlreadyData)
                {
                    ContractGridDataSource = new ObservableCollection<ContractUiModel>(DbContractDataSource);
                }
            }
            else
            {
                var tempList = DbContractDataSource?.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();
                if (tempList == null || tempList.Count == 0)
                {
                    HeaderSearchItemSource.Add(new ContractUiModel() { ContractPlanType = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                }
                else
                {
                    tempList.ForEach(x => HeaderSearchItemSource.Add(x));
                }
            }
        }
        #endregion
    }
}
