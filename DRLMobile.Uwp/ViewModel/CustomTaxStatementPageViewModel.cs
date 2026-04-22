using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.ViewModel
{
    public class CustomTaxStatementPageViewModel : ObservableObject
    {
        #region Properties
        private readonly App AppRef = (App)Application.Current;
        private readonly ResourceLoader resourceLoader;

        private ObservableCollection<UserTaxStatementUiModel> _taxListGridDataSource;
        public ObservableCollection<UserTaxStatementUiModel> TaxListGridDataSource
        {
            get { return _taxListGridDataSource; }
            set { SetProperty(ref _taxListGridDataSource, value); }
        }
        private List<UserTaxStatementUiModel> _dbTaxListDataSource;
        public List<UserTaxStatementUiModel> DbTaxListDataSource
        {
            get { return _dbTaxListDataSource; }
            set { SetProperty(ref _dbTaxListDataSource, value); }
        }
        private UserTaxStatementPageUiModel _userTaxStatementPageModel;
        public UserTaxStatementPageUiModel UserTaxStatementPageModel
        {
            get { return _userTaxStatementPageModel; }
            set { SetProperty(ref _userTaxStatementPageModel, value); }
        }
        private Visibility _loadingVisiblity;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisiblity; }
            set { SetProperty(ref _loadingVisiblity, value); }
        }
        private Visibility _clearSaveVisiblity;
        public Visibility ClearSaveVisibility
        {
            get { return _clearSaveVisiblity; }
            set { SetProperty(ref _clearSaveVisiblity, value); }
        }
        private Visibility _cancelUpdateVisiblity;
        public Visibility CancelUpdateVisibility
        {
            get { return _cancelUpdateVisiblity; }
            set { SetProperty(ref _cancelUpdateVisiblity, value); }
        }
        private UserTaxStatementUiModel _updatedModel;
        public UserTaxStatementUiModel UpdatedModel
        {
            get { return _updatedModel; }
            set { SetProperty(ref _updatedModel, value); }
        }
        #endregion

        #region Commands
        public ICommand PageLoadedCommand { private set; get; }
        public ICommand EditIconClickedCommand { get; private set; }
        public ICommand DeleteClickedCommand { private set; get; }
        public ICommand ClearButtonCommand { private set; get; }
        public ICommand SaveButtonCommand { private set; get; }
        public ICommand CancelButtonCommand { private set; get; }
        public ICommand UpdateButtonCommand { private set; get; }
        #endregion

        #region Constructor
        public CustomTaxStatementPageViewModel()
        {
            InitializeCommands();
            resourceLoader = ResourceLoader.GetForCurrentView();
            DbTaxListDataSource = new List<UserTaxStatementUiModel>();
            TaxListGridDataSource = new ObservableCollection<UserTaxStatementUiModel>();
            UserTaxStatementPageModel = new UserTaxStatementPageUiModel();
            LoadingVisibility = Visibility.Visible;
            ClearSaveVisibility = Visibility.Visible;
            CancelUpdateVisibility = Visibility.Collapsed;
            UpdatedModel = new UserTaxStatementUiModel();
        }

        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            PageLoadedCommand = new AsyncRelayCommand(LoadInitialPageData);
            SaveButtonCommand = new AsyncRelayCommand(SaveClicked);
            ClearButtonCommand = new RelayCommand(ClearClicked);
            EditIconClickedCommand = new RelayCommand<UserTaxStatementUiModel>(EditIconClicked);
            DeleteClickedCommand = new AsyncRelayCommand<UserTaxStatementUiModel>(DeleteClicked);
            CancelButtonCommand = new RelayCommand(CancelClicked);
            UpdateButtonCommand = new AsyncRelayCommand(UpdateClicked);
        }
        private async Task LoadInitialPageData()
        {
            try
            {
                DbTaxListDataSource = await AppRef.QueryService.GetUserTaxStatementList();
                DbTaxListDataSource.ForEach(x => TaxListGridDataSource.Add(x));
                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "LoadInitialPageData", ex.StackTrace);
                LoadingVisibility = Visibility.Collapsed;
            }
        }
        private async Task UpdateClicked()
        {
            if (string.IsNullOrEmpty(UserTaxStatementPageModel?.Title?.Trim()) || string.IsNullOrEmpty(UserTaxStatementPageModel?.Description?.Trim()))
            {
                await ShowPleaseFillRequiredFieldsDialog();
            }
            else
            {
                UpdatedModel.Title = UserTaxStatementPageModel?.Title;
                UpdatedModel.Description = UserTaxStatementPageModel?.Description;
                await UpdateItemInDb(UpdatedModel);
                UserTaxStatementPageModel = new UserTaxStatementPageUiModel();
                ClearSaveVisibility = Visibility.Visible;
                CancelUpdateVisibility = Visibility.Collapsed;
                await ShowTaxStatementUpdatedDialog();
            }
        }

        private void CancelClicked()
        {
            ClearData();
            ClearSaveVisibility = Visibility.Visible;
            CancelUpdateVisibility = Visibility.Collapsed;
        }
        private void ClearClicked()
        {
            ClearData();
        }
        public void ClearData()
        {
            UserTaxStatementPageModel = new UserTaxStatementPageUiModel();
        }
        private async Task SaveClicked()
        {
            if (string.IsNullOrEmpty(UserTaxStatementPageModel?.Title?.Trim()) || string.IsNullOrEmpty(UserTaxStatementPageModel?.Description?.Trim()))
            {
                await ShowPleaseFillRequiredFieldsDialog();
            }
            else
            {
                UserTaxStatementUiModel userTaxStatementUiModel = new UserTaxStatementUiModel();
                userTaxStatementUiModel.Title = UserTaxStatementPageModel?.Title;
                userTaxStatementUiModel.Description = UserTaxStatementPageModel?.Description;
                await InsertUserTaxStatementData(userTaxStatementUiModel);
            }
        }

        private async Task InsertUserTaxStatementData(UserTaxStatementUiModel userTaxStatementUiModel)
        {
            userTaxStatementUiModel.UserTaxStatementID = HelperMethods.GenerateRandomNumberForGivenRange(10000, 99999);
            userTaxStatementUiModel.CreatedBy = AppRef.LoginUserNameProperty;
            userTaxStatementUiModel.CreatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            userTaxStatementUiModel.UpdatedBy = AppRef.LoginUserNameProperty;
            userTaxStatementUiModel.UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            userTaxStatementUiModel.UserID = Convert.ToInt32(AppRef.LoginUserIdProperty);
            Guid id = Guid.NewGuid();
            userTaxStatementUiModel.DeviceUserTaxStatementID = id.ToString();
            userTaxStatementUiModel.UserTaxStatementUiToDataModel();

            var _list = new List<UserTaxStatement>() { userTaxStatementUiModel.UserTaxStatementMasterData };

            if (_list?.Count > 0)
            {
                await AppRef.QueryService.InsertUserTaxStatement(_list[0]);
                TaxListGridDataSource.Add(userTaxStatementUiModel);
                UserTaxStatementPageModel = new UserTaxStatementPageUiModel();
                await ShowTaxStatementCreatedDialog();
            }
        }

        private async Task ShowPleaseFillRequiredFieldsDialog()
        {
            ContentDialog pleaseAddPopDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("PleaseFillRequiredFields"),
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await pleaseAddPopDialog.ShowAsync();
        }

        private async Task ShowTaxStatementUpdatedDialog()
        {
            ContentDialog taxStatementUpdatedDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("TaxStatementUpdated"),
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await taxStatementUpdatedDialog.ShowAsync();
        }
        private async Task ShowTaxStatementCreatedDialog()
        {
            ContentDialog taxStatementCreatedDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("TaxStatementCreated"),
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await taxStatementCreatedDialog.ShowAsync();
        }
        private async Task DeleteClicked(UserTaxStatementUiModel userTaxStatementUiModel)
        {
            await ShowDeleteWarning(userTaxStatementUiModel);
        }
        private async Task ShowDeleteWarning(UserTaxStatementUiModel userTaxStatementUiModel)
        {

            ContentDialog deleteTaxItemDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("DeleteTaxStatement"),
                PrimaryButtonText = resourceLoader.GetString("YesText"),
                SecondaryButtonText = resourceLoader.GetString("Cancel")
            };

            ContentDialogResult result = await deleteTaxItemDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await DeleteTaxItemAsync(userTaxStatementUiModel);
            }
            else
            {
                deleteTaxItemDialog.Hide();
            }
        }
        private async Task DeleteTaxItemAsync(UserTaxStatementUiModel userTaxStatementUiModel)
        {
            await DeleteItemFromDb(userTaxStatementUiModel);
            TaxListGridDataSource.Remove(userTaxStatementUiModel);
            UserTaxStatementPageModel = new UserTaxStatementPageUiModel();
        }
        private async Task DeleteItemFromDb(UserTaxStatementUiModel userTaxStatementUiModel)
        {
            userTaxStatementUiModel.UpdatedBy = AppRef.LoginUserNameProperty;
            userTaxStatementUiModel.UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            userTaxStatementUiModel.UserTaxStatementUiToDataModel();
            var _list = new List<UserTaxStatement>() { userTaxStatementUiModel.UserTaxStatementMasterData };
            await AppRef.QueryService.DeleteUserTaxStatement(_list[0]);

        }
        private void EditIconClicked(UserTaxStatementUiModel userTaxStatementUiModel)
        {
            UserTaxStatementPageModel.Title = userTaxStatementUiModel?.Title;
            UserTaxStatementPageModel.Description = userTaxStatementUiModel?.Description;
            UpdatedModel = userTaxStatementUiModel;
            ClearSaveVisibility = Visibility.Collapsed;
            CancelUpdateVisibility = Visibility.Visible;
        }
        private async Task UpdateItemInDb(UserTaxStatementUiModel userTaxStatementUiModel)
        {
            userTaxStatementUiModel.UpdatedBy = AppRef.LoginUserNameProperty;
            userTaxStatementUiModel.UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            userTaxStatementUiModel.UserTaxStatementUiToDataModel();
            var _list = new List<UserTaxStatement>() { userTaxStatementUiModel.UserTaxStatementMasterData };
            await AppRef.QueryService.UpdateUserTaxStatement(_list[0]);
        }

        #endregion
    }
}
