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
    public class RouteListPageViewModel : BaseModel
    {
        #region Properties
        private readonly ResourceLoader resourceLoader;

        private readonly App AppReference = (App)Application.Current;

        private ObservableCollection<RouteListUIModel> _routeListUiItemSource;
        public ObservableCollection<RouteListUIModel> RouteListItemSource
        {
            get { return _routeListUiItemSource; }
            set { SetProperty(ref _routeListUiItemSource, value); }
        }

        private ObservableCollection<RouteListUIModel> _headerSearchItemSource;
        public ObservableCollection<RouteListUIModel> HeaderSearchItemSource
        {
            get { return _headerSearchItemSource; }
            set { SetProperty(ref _headerSearchItemSource, value); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private Visibility _loadingVisibility;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set { SetProperty(ref _loadingVisibility, value); }
        }

        public List<RouteListUIModel> RouteListDBSource { get; set; }

        #endregion

        #region Commands
        public ICommand HeaderSearchTextChangeCommand { private set; get; }
        public ICommand HeaderSearchSuggestionChoosenCommand { private set; get; }
        public ICommand EditRouteCommand { get; private set; }
        public ICommand ViewRouteCommand { get; private set; }
        public ICommand ClearDateCommnd { get; private set; }
        public ICommand AddRouteCommnd { get; private set; }
        #endregion

        #region Constructor
        public RouteListPageViewModel()
        {
            resourceLoader = ResourceLoader.GetForCurrentView();

            RouteListDBSource = new List<RouteListUIModel>();
            RouteListItemSource = new ObservableCollection<RouteListUIModel>();
            HeaderSearchItemSource = new ObservableCollection<RouteListUIModel>();

            ClearDateCommnd = new RelayCommand(ClearDatesCommandHanlder);
            AddRouteCommnd = new RelayCommand(AddNewRouteCommandHandler);
            EditRouteCommand = new RelayCommand<RouteListUIModel>(EditRouteCommandHandler);
            ViewRouteCommand = new RelayCommand<RouteListUIModel>(ViewRouteCommandHandler);
            HeaderSearchTextChangeCommand = new AsyncRelayCommand<string>(HeaderSearchTextChangeCommandHandler);
            HeaderSearchSuggestionChoosenCommand = new RelayCommand<RouteListUIModel>(SuggestionChoosen);

            LoadingVisibility = Visibility.Collapsed;
        }


        #endregion

        public async Task InitializeDataOnPageLoad()
        {
            LoadingVisibilityHandler(isLoading: true);

            await AppReference.QueryService.UpdateRoutesForCurrentUser(Convert.ToInt32(AppReference.LoginUserIdProperty));

            RouteListDBSource = await AppReference.QueryService.GetScheduledRouteListData();
            RouteListItemSource.Clear();
            if (RouteListDBSource != null && RouteListDBSource.Count > 0)
            {
                foreach (var item in RouteListDBSource)
                {
                    item.EditIconVisibility = item.UserId == Convert.ToInt32(AppReference.LoginUserIdProperty) ? Visibility.Visible : (item.idAssignToTSM == 0 ? Visibility.Collapsed : (item.idAssignToTSM == Convert.ToInt32(AppReference.LoginUserIdProperty) ? Visibility.Visible : Visibility.Collapsed));

                    RouteListItemSource.Add(item);
                }
            }

            LoadingVisibilityHandler(isLoading: false);
        }


        #region Private Methods
        private async void ClearDatesCommandHanlder()
        {
            LoadingVisibilityHandler(isLoading: true);

            await Task.Delay(100);

            RouteListItemSource.Clear();

            RouteListDBSource.ForEach(x =>
            {
                x.EditIconVisibility = x.UserId == Convert.ToInt32(AppReference.LoginUserIdProperty) ? Visibility.Visible : Visibility.Collapsed;

                RouteListItemSource.Add(x);
            });

            LoadingVisibilityHandler(isLoading: false);
        }

        private void AddNewRouteCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(AddEditRoutePage));
        }

        private void ViewRouteCommandHandler(RouteListUIModel selectedRoute)
        {
            NavigationService.NavigateShellFrame(typeof(ViewRouteListPage), selectedRoute);
        }

        private void EditRouteCommandHandler(RouteListUIModel selectedRoute)
        {
            NavigationService.NavigateShellFrame(typeof(AddEditRoutePage), selectedRoute);
        }

        private async Task HeaderSearchTextChangeCommandHandler(string SearchText)
        {
            HeaderSearchItemSource.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                var ifDataGridHasAlreadyData = RouteListDBSource?.Count == RouteListItemSource?.Count;

                if (ifDataGridHasAlreadyData)
                {
                    LoadHeaderSearchWithInitialData();
                }
                else
                {
                    await LoadDataGridAndHeaderSearchWithInitialData();
                }
            }
            else
            {
                var tempList = RouteListDBSource?.Where(x => x.SearchDisplayPath.ToLower().Contains(SearchText.ToLower())).ToList();

                if (tempList == null || tempList.Count == 0)
                {
                    HeaderSearchItemSource.Add(new RouteListUIModel() { RouteName = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                }
                else
                {
                    tempList.ForEach(x =>
                    {
                        x.EditIconVisibility = x.UserId == Convert.ToInt32(AppReference.LoginUserIdProperty) ? Visibility.Visible : Visibility.Collapsed;
                        HeaderSearchItemSource.Add(x);
                    });
                }
            }
        }

        private async Task LoadDataGridAndHeaderSearchWithInitialData()
        {
            LoadingVisibilityHandler(isLoading: true);

            await Task.Delay(50);

            RouteListItemSource.Clear();

            RouteListDBSource.ForEach(x =>
            {
                x.EditIconVisibility = x.UserId == Convert.ToInt32(AppReference.LoginUserIdProperty) ? Visibility.Visible : Visibility.Collapsed;
                RouteListItemSource.Add(x);
            });

            HeaderSearchItemSource.Clear();

            LoadingVisibilityHandler(isLoading: false);
        }

        private void LoadHeaderSearchWithInitialData()
        {
            RouteListDBSource.ForEach(x =>
            {
                x.EditIconVisibility = x.UserId == Convert.ToInt32(AppReference.LoginUserIdProperty) ? Visibility.Visible : Visibility.Collapsed;
                HeaderSearchItemSource.Add(x);
            });
        }

        private void SuggestionChoosen(RouteListUIModel selectedItem)
        {
            if (selectedItem.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
            {
                return;
            }

            RouteListItemSource.Clear();

            var _filterItem = RouteListDBSource.FirstOrDefault(x => x.RouteName.Equals(selectedItem.RouteName));

            RouteListItemSource.Add(_filterItem);
        }

        private void LoadingVisibilityHandler(bool isLoading)
        {
            LoadingVisibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion
    }
}