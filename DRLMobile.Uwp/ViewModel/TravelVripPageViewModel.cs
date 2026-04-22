using DRLMobile.Core.Models;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace DRLMobile.Uwp.ViewModel
{
    public class TravelVripPageViewModel : BaseModel
    {
        #region Properties
        private readonly ResourceLoader resourceLoader;

        private ObservableCollection<TravelUiModel> _travelGridDataSource;
        public ObservableCollection<TravelUiModel> TravelGridDataSource
        {
            get { return _travelGridDataSource; }
            set { SetProperty(ref _travelGridDataSource, value); }
        }

        private ObservableCollection<VripUiModel> _vripGridDataSource;
        public ObservableCollection<VripUiModel> VripGridDataSource
        {
            get { return _vripGridDataSource; }
            set { SetProperty(ref _vripGridDataSource, value); }
        }

        private Visibility _isTravelGridVisible;
        public Visibility IsTravelGridVisible
        {
            get { return _isTravelGridVisible; }
            set { SetProperty(ref _isTravelGridVisible, value); }
        }

        private Visibility _isVripGridVisible;
        public Visibility IsVripGridVisible
        {
            get { return _isVripGridVisible; }
            set { SetProperty(ref _isVripGridVisible, value); }
        }

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set { SetProperty(ref _pageTitle, value); }
        }
        private List<string> _travelProgramYears;
        public List<string> TravelProgramYears
        {
            get { return _travelProgramYears; }
            set { SetProperty(ref _travelProgramYears, value); }
        }
        private List<string> _vripProgramYears;
        public List<string> VripProgramYears
        {
            get { return _vripProgramYears; }
            set { SetProperty(ref _vripProgramYears, value); }
        }

        private ObservableCollection<string> _headerComboBoxDataSource;
        public ObservableCollection<string> HeaderComboBoxDataSource
        {
            get { return _headerComboBoxDataSource; }
            set { SetProperty(ref _headerComboBoxDataSource, value); }
        }

        private string _comboboxPlaceHolderText;
        public string ComboboxPlaceHolderText
        {
            get { return _comboboxPlaceHolderText; }
            set { SetProperty(ref _comboboxPlaceHolderText, value); }
        }

        private List<TravelUiModel> _dbTravelDataSource;
        public List<TravelUiModel> DbTravelDataSource
        {
            get { return _dbTravelDataSource; }
            set { SetProperty(ref _dbTravelDataSource, value); }
        }

        private List<VripUiModel> _dbVripDataSource;
        public List<VripUiModel> DbVripDataSource
        {
            get { return _dbVripDataSource; }
            set { SetProperty(ref _dbVripDataSource, value); }
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

        private Visibility _loadingVisiblity;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisiblity; }
            set { SetProperty(ref _loadingVisiblity, value); }
        }
        #endregion

        #region Commands
        public ICommand OnNavigatedTo { private set; get; }
        public ICommand LeftSegmentButtonCommand { get; private set; }
        public ICommand RightSegmentButtonCommand { get; private set; }
        public ICommand TravelComboBoxClickedCommand { private set; get; }
        public ICommand HeaderSearchTextChangeCommand { private set; get; }
        public ICommand HeaderSearchSuggestionChoosenCommand { private set; get; }
        public ICommand TravelItemSelectedCommand { private set; get; }
        public ICommand VripItemSelectedCommand { private set; get; }

        #endregion

        #region Constructor
        public TravelVripPageViewModel()
        {
            LoadingVisibility = Visibility.Visible;

            resourceLoader = ResourceLoader.GetForCurrentView();
            PageTitle = resourceLoader.GetString("TRAVEL");
            
            AutoSuggestionText = string.Empty;
           
            TravelGridDataSource = new ObservableCollection<TravelUiModel>();
            DbTravelDataSource = new List<TravelUiModel>();
            VripGridDataSource = new ObservableCollection<VripUiModel>();
            DbVripDataSource = new List<VripUiModel>();
            TravelProgramYears = new List<string>();
            VripProgramYears = new List<string>();
            HeaderComboBoxDataSource = new ObservableCollection<string>();
            HeaderSearchItemSource = new ObservableCollection<object>();

            IsTravelGridVisible = Visibility.Visible;
            IsVripGridVisible = Visibility.Collapsed;

            OnNavigatedTo = new AsyncRelayCommand(LoadInitialPageData);

            LeftSegmentButtonCommand = new AsyncRelayCommand(LeftSegmentClicked);
            RightSegmentButtonCommand = new AsyncRelayCommand(RightSegmentClicked);
            TravelComboBoxClickedCommand = new RelayCommand<int>(HandleTravelComboboxChanged);
            HeaderSearchTextChangeCommand = new RelayCommand<string>(HandleTextChangeHeaderCommand);
            HeaderSearchSuggestionChoosenCommand = new RelayCommand<object>(SuggestionChoosen);
            TravelItemSelectedCommand = new RelayCommand<TravelUiModel>(TravelItemSelected);
            VripItemSelectedCommand = new RelayCommand<VripUiModel>(VripItemSelected);
        }

        #endregion

        #region Private Methods
        private async Task LoadInitialPageData()
        {
            try
            {
                await GetProgramYearsForComboBox();

                await FetchTravelDataForCurrentYear();
                
                LoadingVisibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private async Task GetProgramYearsForComboBox()
        {
            TravelProgramYears = await ((App)Application.Current).QueryService.GetTravelProgramYearFromVripTravelData();

            VripProgramYears = await ((App)Application.Current).QueryService.GetVripProgramYearFromVripTravelData();
            
            if (TravelProgramYears != null)
            {
                var travelProgramYear = Convert.ToInt32(TravelProgramYears[0]) - 1;

                TravelProgramYears?.Add(travelProgramYear.ToString());
                
                HeaderComboBoxDataSource = new ObservableCollection<string>(TravelProgramYears);
                
                ComboboxPlaceHolderText = TravelProgramYears[0];
            }

            if (VripProgramYears != null)
            {
                var vripProgramYear = Convert.ToInt32(VripProgramYears[0]) - 1;
                VripProgramYears?.Add(vripProgramYear.ToString());
            }
        }

        private async void HandleTravelComboboxChanged(int selectedIndex)
        {
            AutoSuggestionText = string.Empty;

            if (selectedIndex == 1)
            {
                if (IsTravelGridVisible == Visibility.Visible)
                {
                    await FetchTravelDataForPreviousYear();
                }
                else
                {
                    await FetchVripDataForPreviousYear();
                }
            }
            else if (selectedIndex == 0)
            {
                if (IsTravelGridVisible == Visibility.Visible)
                {
                    await FetchTravelDataForCurrentYear();
                }
                else
                {
                    await FetchVripDataForCurrentYear();
                }
            }
        }

        private async Task FetchTravelDataForPreviousYear()
        {
            LoadingVisibility = Visibility.Visible;

            if (TravelProgramYears != null)
            {
                DbTravelDataSource = await ((App)Application.Current).QueryService.GetTravelDataForUser(TravelProgramYears[1]);
                TravelGridDataSource.Clear();
                if (DbTravelDataSource?.Count > 0)
                {
                    DbTravelDataSource.ForEach(x => TravelGridDataSource.Add(x));
                }
            }
            LoadingVisibility = Visibility.Collapsed;

        }

        private async Task FetchTravelDataForCurrentYear()
        {
            LoadingVisibility = Visibility.Visible;

            if (TravelProgramYears != null)
            {
                TravelGridDataSource.Clear();

                DbTravelDataSource = await ((App)Application.Current).QueryService.GetTravelDataForUser(TravelProgramYears[0]);
                
                if (DbTravelDataSource?.Count > 0)
                {
                    DbTravelDataSource.ForEach(x => TravelGridDataSource.Add(x));
                }
            }

            LoadingVisibility = Visibility.Collapsed;

        }

        private async Task FetchVripDataForPreviousYear()
        {
            LoadingVisibility = Visibility.Visible;

            if (VripProgramYears != null)
            {
                VripGridDataSource.Clear();
                DbVripDataSource = await ((App)Application.Current).QueryService.GetVripDataForUser(VripProgramYears[1]);
                if (DbVripDataSource?.Count > 0)
                {
                    DbVripDataSource.ForEach(x => VripGridDataSource.Add(x));
                }
            }
            LoadingVisibility = Visibility.Collapsed;

        }
        private async Task FetchVripDataForCurrentYear()
        {
            LoadingVisibility = Visibility.Visible;

            if (VripProgramYears != null)
            {
                VripGridDataSource.Clear();
                DbVripDataSource = await ((App)Application.Current).QueryService.GetVripDataForUser(VripProgramYears[0]);
                if (DbVripDataSource?.Count > 0)
                {
                    DbVripDataSource.ForEach(x => VripGridDataSource.Add(x));
                }
            }
            LoadingVisibility = Visibility.Collapsed;

        }
        private async Task LeftSegmentClicked()
        {
            IsTravelGridVisible = Visibility.Visible;
            IsVripGridVisible = Visibility.Collapsed;
            AutoSuggestionText = string.Empty;

            PageTitle = resourceLoader.GetString("TRAVEL");
            if (TravelProgramYears != null)
            {
                HeaderComboBoxDataSource = new ObservableCollection<string>(TravelProgramYears);
                ComboboxPlaceHolderText = TravelProgramYears[0];
            }

            await FetchTravelDataForCurrentYear();

            HeaderSearchItemSource = new ObservableCollection<object>();
        }

        private async Task RightSegmentClicked()
        {
            IsTravelGridVisible = Visibility.Collapsed;
            IsVripGridVisible = Visibility.Visible;
            AutoSuggestionText = string.Empty;

            PageTitle = resourceLoader.GetString("VRIP");
            if (VripProgramYears != null)
            {
                HeaderComboBoxDataSource = new ObservableCollection<string>(VripProgramYears);
                ComboboxPlaceHolderText = VripProgramYears[0];
            }
            await FetchVripDataForCurrentYear();
            HeaderSearchItemSource = new ObservableCollection<object>();
        }
        private void SuggestionChoosen(object selectedItem)
        {
            if (selectedItem is TravelUiModel)
            {
                var travelUiModel = selectedItem as TravelUiModel;
                if (travelUiModel.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
                {
                    return;
                }
                TravelGridDataSource.Clear();
                var _filterItem = DbTravelDataSource.FirstOrDefault(x => x.CustomerNumber.Equals(travelUiModel.CustomerNumber));
                TravelGridDataSource.Add(_filterItem);
            }
            else if (selectedItem is VripUiModel)
            {
                var vripUiModel = selectedItem as VripUiModel;
                if (vripUiModel.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
                {
                    return;
                }
                VripGridDataSource.Clear();
                var _filterItem = DbVripDataSource.FirstOrDefault(x => x.CustomerNumber.Equals(vripUiModel.CustomerNumber));
                VripGridDataSource.Add(_filterItem);
            }

        }

        private void HandleTextChangeHeaderCommand(string text)
        {
            if (IsTravelGridVisible == Visibility.Visible)
            {
                HandleTravelSearchBox(text);
            }
            else
            {
                HandleVripSearchBox(text);
            }
        }

        private void HandleTravelSearchBox(string text)
        {
            HeaderSearchItemSource.Clear();

            if (string.IsNullOrWhiteSpace(text))
            {
                var ifDataGridHasAlreadyData = DbTravelDataSource?.Count != TravelGridDataSource?.Count;
                if (ifDataGridHasAlreadyData)
                {
                    TravelGridDataSource = new ObservableCollection<TravelUiModel>(DbTravelDataSource);
                }
            }
            else
            {
                var tempList = DbTravelDataSource?.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();
                if (tempList == null || tempList.Count == 0)
                {
                    HeaderSearchItemSource.Add(new TravelUiModel() { CustomerName = ResourceExtensions.GetLocalized("NoResultsErrorMessage"), NeededPoint = string.Empty, NeededPointToShow = string.Empty, Awards = string.Empty });
                }
                else
                {
                    tempList.ForEach(x => HeaderSearchItemSource.Add(x));
                }
            }
        }
        private void HandleVripSearchBox(string text)
        {
            HeaderSearchItemSource.Clear();

            if (string.IsNullOrWhiteSpace(text))
            {
                var ifDataGridHasAlreadyData = DbVripDataSource?.Count != VripGridDataSource?.Count;
                if (ifDataGridHasAlreadyData)
                {
                    VripGridDataSource = new ObservableCollection<VripUiModel>(DbVripDataSource);
                }
            }
            else
            {
                var tempList = DbVripDataSource?.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();
                if (tempList == null || tempList.Count == 0)
                {
                    HeaderSearchItemSource.Add(new VripUiModel() { CustomerName = ResourceExtensions.GetLocalized("NoResultsErrorMessage"), TargetToShow = string.Empty, CSNeededToShow = string.Empty, CsytdToShow = string.Empty, Cslyr = string.Empty });
                }
                else
                {
                    tempList.ForEach(x => HeaderSearchItemSource.Add(x));
                }
            }
        }
        private void TravelItemSelected(TravelUiModel travelUiModel)
        {
            NavigationService.NavigateShellFrame(typeof(CustomerDetailsPage), Convert.ToInt32(travelUiModel.CustomerId));

        }

        private void VripItemSelected(VripUiModel vripUiModel)
        {
            NavigationService.NavigateShellFrame(typeof(CustomerDetailsPage), Convert.ToInt32(vripUiModel.CustomerId));
        }
        #endregion

    }
}
