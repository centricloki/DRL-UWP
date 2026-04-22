using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.CustomControls;
using DRLMobile.ExceptionHandler;
using DRLMobile.Helpers;
using DRLMobile.Services;
using DRLMobile.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DRLMobile.ViewModels
{
    public class FavoritePageViewModel : ObservableObject
    {

        #region properties
        private readonly ResourceLoader resourceLoader;
        private int _badgeCount = 0;
        public int BadgeCount
        {
            get { return _badgeCount; }
            set { SetProperty(ref _badgeCount, value); }
        }

        private ObservableCollection<FavoriteUiModel> _favoriteGridDataSource;

        public ObservableCollection<FavoriteUiModel> FavoriteGridDataSource
        {
            get { return _favoriteGridDataSource; }
            set { SetProperty(ref _favoriteGridDataSource, value); }
        }
        private Visibility _isBadgeVisible;
        public Visibility IsBadgeVisible
        {
            get { return _isBadgeVisible; }
            set { SetProperty(ref _isBadgeVisible, value); }
        }

        private string _badgeText;
        public string BadgeText
        {
            get { return _badgeText; }
            set { SetProperty(ref _badgeText, value); }
        }
        private Visibility _noProductVisiblity;

        public Visibility NoProductVisibility
        {
            get { return _noProductVisiblity; }
            set { SetProperty(ref _noProductVisiblity, value); }
        }
        private Visibility _productListVisiblity;

        public Visibility ProductListVisibility
        {
            get { return _productListVisiblity; }
            set { SetProperty(ref _productListVisiblity, value); }
        }

        private Visibility _bottomButtonStackButtonVisiblity;

        public Visibility BottomButtonStackVisiblity
        {
            get { return _bottomButtonStackButtonVisiblity; }
            set { SetProperty(ref _bottomButtonStackButtonVisiblity, value); }
        }
        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set { SetProperty(ref _customerName, value); }
        }
        private string _customerNumber;
        public string CustomerNumber
        {
            get { return _customerNumber; }
            set { SetProperty(ref _customerNumber, value); }
        }
        private Visibility _customerTitlePanelVisibility;

        public Visibility CustomerTitlePanelVisibility
        {
            get { return _customerTitlePanelVisibility; }
            set { SetProperty(ref _customerTitlePanelVisibility, value); }
        }
        private ObservableCollection<FavoriteUiModel> _headerSearchItemSource;
        public ObservableCollection<FavoriteUiModel> HeaderSearchItemSource
        {
            get { return _headerSearchItemSource; }
            set { SetProperty(ref _headerSearchItemSource, value); }
        }
        private Visibility _loadingVisiblity;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisiblity; }
            set { SetProperty(ref _loadingVisiblity, value); }
        }
        private bool _isHeaderChecked;
        public bool IsHeaderChecked
        {
            get { return _isHeaderChecked; }
            set { SetProperty(ref _isHeaderChecked, value); }
        }
        private readonly App AppRef = ((App)Application.Current);
        private List<FavoriteUiModel> DbFavoriteDataSource;
        private List<OrderDetailUIModel> DbCartDataSource;
        private List<CategoryMaster> DbCategoryDataSource;

        #endregion

        #region commands
        public ICommand OnNavigatedTo { private set; get; }
        public ICommand EmailFactsheetButtonCommand { get; private set; }
        public ICommand AddToCartButtonCommand { get; private set; }
        public ICommand UnfavoriteImageCommand { private set; get; }
        public ICommand IsAllRowsSelectedCommand { private set; get; }
        public ICommand IsAllRowsUnselectedCommand { private set; get; }
        private ICommand _cartButtonCommand;
        public ICommand CartButtonCommand => _cartButtonCommand ?? (_cartButtonCommand = new RelayCommand(NavigateToCartPage));
        public ICommand CustomerTitlePanelClickCommand { get; private set; }
        public ICommand HeaderSearchTextChangeCommand { private set; get; }
        public ICommand HeaderSearchSuggestionChoosenCommand { private set; get; }

        public ICommand DetailsCartCommand { get; private set; }
        private List<int> CartProductIdList;

        #endregion

        #region constructor

        public FavoritePageViewModel()
        {
            LoadingVisibility = Visibility.Visible;
            CustomerName = string.Empty;
            CustomerNumber = string.Empty;
            IsHeaderChecked = false;
            FavoriteGridDataSource = new ObservableCollection<FavoriteUiModel>();
            HeaderSearchItemSource = new ObservableCollection<FavoriteUiModel>();
            DbFavoriteDataSource = new List<FavoriteUiModel>();
            DbCartDataSource = new List<OrderDetailUIModel>();
            DbCategoryDataSource = new List<CategoryMaster>();
            BadgeText = string.Empty;
            IsBadgeVisible = Visibility.Collapsed;
            OnNavigatedTo = new AsyncRelayCommand(LoadInitialPageData);
            UnfavoriteImageCommand = new AsyncRelayCommand<FavoriteUiModel>(UnfavoriteImageClickedAsync);
            IsAllRowsSelectedCommand = new RelayCommand(IsAllRowsSelected);
            IsAllRowsUnselectedCommand = new RelayCommand(IsAllRowsUnselected);
            ItemClickCommand = new AsyncRelayCommand<FavoriteUiModel>(ItemClickCommandHandler);
            ShowHideDetailCommand = new RelayCommand(ShowHideDetailCommandHandler);
            GridRowChecBoxCheckChangeCommand = new RelayCommand(GridRowChecBoxCheckChangeCommandHandler);
            DetailUIModel = new FavoriteDetailPageUIModel();
            PrevoiusNextCommand = new AsyncRelayCommand<string>(PrevoiusNextCommandHandler);
            ClosePreviewCommand = new RelayCommand(ClosePreviewCommandHandler);
            ShowPreviewCommand = new RelayCommand(ShowPreviewCommandHandler);
            IsPreviewDocumentControlVisible = false;
            DetailsActionCommand = new AsyncRelayCommand<string>(DetailsActionCommandHandler);
            MailProductDetailsCommand = new AsyncRelayCommand(MailProductDetailsCommandHandler);

            resourceLoader = ResourceLoader.GetForCurrentView();
            HeaderSearchTextChangeCommand = new AsyncRelayCommand<string>(HandleTextChangeHeaderCommand);
            EmailFactsheetButtonCommand = new AsyncRelayCommand(EmailFactsheetButtonCommandHandler);
            AddToCartButtonCommand = new AsyncRelayCommand(AddToCartButtonCommandHandler);
            DetailsCartCommand = new AsyncRelayCommand(DetailsCartCommandHandler);
            CustomerTitlePanelClickCommand = new AsyncRelayCommand(CustomerTitlePanelClicked);


            HeaderSearchSuggestionChoosenCommand = new RelayCommand<FavoriteUiModel>(SuggestionChoosen);
            NoProductVisibility = Visibility.Collapsed;
            ProductListVisibility = Visibility.Visible;
            BottomButtonStackVisiblity = Visibility.Visible;
            CustomerTitlePanelVisibility = Visibility.Collapsed;
            CartProductIdList = new List<int>();
        }




        #endregion

        #region private methods

        private async Task LoadInitialPageData()
        {
            try
            {
                BadgeCount = AppRef.CartItemCount;

                UpdateBadgeText();

                if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId))
                {
                    int customerId = Convert.ToInt32(AppRef.SelectedCustomerId);

                    var selectedCustomer = await ((App)Application.Current).QueryService.GetSavedCustomerInformation(customerId);

                    if (selectedCustomer != null)
                    {
                        CustomerName = selectedCustomer.CustomerName;
                        CustomerNumber = selectedCustomer.CustomerNumber;
                        CustomerTitlePanelVisibility = Visibility.Visible;
                    }
                }

                DbFavoriteDataSource = await ((App)Application.Current).QueryService.GetFavoriteGridData();

                DbCartDataSource = await ((App)Application.Current).QueryService.GetCartProductDetailsData(AppRef.CurrentOrderId);

                CartProductIdList = await ((App)Application.Current).QueryService.GetCartEligibleProducts(AppRef.CurrentOrderId);

                DbCategoryDataSource = await ((App)Application.Current).QueryService.GetCategoryData();

                if (DbFavoriteDataSource?.Count > 0)
                {
                    DbFavoriteDataSource.ForEach(x => FavoriteGridDataSource.Add(x));
                    ProductListVisibility = Visibility.Visible;
                    NoProductVisibility = Visibility.Collapsed;
                    BottomButtonStackVisiblity = Visibility.Visible;
                }
                else
                {
                    ShowNoProductView();
                }

                LoadingVisibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Show no product view
        /// </summary>
        private void ShowNoProductView()
        {
            ProductListVisibility = Visibility.Collapsed;
            NoProductVisibility = Visibility.Visible;
            BottomButtonStackVisiblity = Visibility.Collapsed;
        }

        private void NavigateToCartPage()
        {
            NavigationService.Navigate<CartPage>();
        }

        /// <summary>
        /// Delete cart item
        /// </summary>
        /// <param name="FavoriteUiModel"></param>
        private async Task UnfavoriteImageClickedAsync(FavoriteUiModel favoriteUiModel)
        {
            _isNotARowSelection = true;
            await ShowUnfavoriteWarning(favoriteUiModel);
        }

        /// <summary>
        /// Warning pop-up for deleting cart item
        /// </summary>
        /// <param name="FavoriteUiModel"></param>
        private async Task ShowUnfavoriteWarning(FavoriteUiModel favoriteUiModel)
        {

            ContentDialog unfavoriteItemDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("UnfavoriteWarning"),
                PrimaryButtonText = resourceLoader.GetString("OK"),
                SecondaryButtonText = resourceLoader.GetString("Cancel")
            };

            ContentDialogResult result = await unfavoriteItemDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await DeleteFavoriteItemFromDb(favoriteUiModel);

                FavoriteGridDataSource.Remove(favoriteUiModel);

                if (FavoriteGridDataSource?.Count == 0)
                {
                    ShowNoProductView();
                }
            }
            else
            {
                unfavoriteItemDialog.Hide();
            }
        }

        private async Task ShowRemoveCustomerWarning()
        {
            CustomContentDialog dialog = new CustomContentDialog();
            dialog.Title = resourceLoader.GetString("WouldYouLikeTo");
            dialog.FirstButtonText = resourceLoader.GetString("CustomerKeepCart");
            dialog.SecondButtonText = resourceLoader.GetString("CustomerEmptyCart");
            dialog.CancelButtonText = resourceLoader.GetString("Cancel");

            await dialog.ShowAsync();

            if (dialog.Result == Result.Yes)
            {
                ChangeCustomerKeepCart();
            }
            else if (dialog.Result == Result.No)
            {
                await ChangeCustomerEmptyCart();
            }
        }

        private void ChangeCustomerKeepCart()
        {
            AppRef.SelectedCustomerId = string.Empty;
            CustomerName = string.Empty;
            CustomerNumber = string.Empty;
            CustomerTitlePanelVisibility = Visibility.Collapsed;

        }
        private async Task ChangeCustomerEmptyCart()
        {
            AppRef.SelectedCustomerId = string.Empty;
            CustomerName = string.Empty;
            CustomerNumber = string.Empty;
            CustomerTitlePanelVisibility = Visibility.Collapsed;
            foreach (var item in DbCartDataSource)
            {
                await DeleteCartItemFromDb(item.ProductID, AppRef.CurrentOrderId);
            }
            BadgeCount = 0;
            AppRef.CartItemCount = 0;
            IsBadgeVisible = Visibility.Collapsed;
            BadgeText = string.Empty;
        }
        private async Task DeleteCartItemFromDb(int productId, int deviceOrderId)
        {
            await ((App)Application.Current).QueryService.DeleteOrderDetail(productId, deviceOrderId);
        }

        private async Task DeleteFavoriteItemFromDb(FavoriteUiModel favoriteUiModel)
        {
            favoriteUiModel.FavoriteUiToDataModel();

            var _list = new List<Favorite>() { favoriteUiModel.FavoriteData };

            _list[0].UserId = Convert.ToInt32(AppRef.LoginUserIdProperty);
            _list[0].UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            _list[0].isDeleted = 1;

            await ((App)Application.Current).QueryService.InsertFavorite(_list);
        }

        private void IsAllRowsUnselected()
        {
            _isNotARowSelection = true;
            foreach (var item in FavoriteGridDataSource)
            {
                item.IsAllRowsSelected = false;
            }
            IsHeaderChecked = false;
        }

        private void IsAllRowsSelected()
        {
            _isNotARowSelection = true;
            foreach (var item in FavoriteGridDataSource)
            {
                item.IsAllRowsSelected = true;
            }
            IsHeaderChecked = true;
        }

        private void SuggestionChoosen(FavoriteUiModel selectedItem)
        {
            if (selectedItem.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
            {
                return;
            }
            FavoriteGridDataSource.Clear();
            var _filterItem = DbFavoriteDataSource.FirstOrDefault(x => x.ItemNumber.Equals(selectedItem.ItemNumber));
            FavoriteGridDataSource.Add(_filterItem);
        }

        private async Task HandleTextChangeHeaderCommand(string text)
        {
            HeaderSearchItemSource.Clear();
            if (string.IsNullOrWhiteSpace(text))
            {
                var ifDataGridHasAlreadyData = DbFavoriteDataSource?.Count == FavoriteGridDataSource?.Count;

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
                var tempList = DbFavoriteDataSource?.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();

                if (tempList == null || tempList.Count == 0)
                {
                    HeaderSearchItemSource.Add(new FavoriteUiModel() { ItemNumber = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                }
                else
                {
                    tempList.ForEach(x => HeaderSearchItemSource.Add(x));
                }
            }
        }

        private void LoadHeaderSearchWithInitialData()
        {
            DbFavoriteDataSource.ForEach(x => { HeaderSearchItemSource.Add(x); });
        }

        private async Task LoadDataGridAndHeaderSearchWithInitialData()
        {
            LoadingVisibilityHandler(isLoading: true);
            await Task.Delay(200);
            FavoriteGridDataSource.Clear();
            DbFavoriteDataSource.ForEach(x =>
            {
                FavoriteGridDataSource.Add(x);
                HeaderSearchItemSource.Add(x);
            });
            LoadingVisibilityHandler(isLoading: false);
        }

        private void LoadingVisibilityHandler(bool isLoading)
        {
            LoadingVisibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        private async Task AddToCartButtonCommandHandler()
        {
            if (string.IsNullOrEmpty(AppRef.SelectedCustomerId))
            {
                await ShowNoCustomerSelected();
            }
            else
            {
                var selectedRows = FavoriteGridDataSource.Where(x => x.IsAllRowsSelected);
                if (selectedRows?.Count() > 0)
                {
                    List<FavoriteUiModel> filteredItemList = await FilterItemsEligibleForCart(selectedRows);
                    if (filteredItemList?.Count > 0)
                    {
                        await AddItemsToCart(filteredItemList);
                    }

                    await Task.Delay(200);
                    DbCartDataSource = await ((App)Application.Current).QueryService.GetCartProductDetailsData(AppRef.CurrentOrderId);
                }
                else
                {
                    await ShowNoProductSelected();
                }
            }

        }


        private async Task< List<FavoriteUiModel>> FilterItemsEligibleForCart(IEnumerable<FavoriteUiModel> selectedRows)
        {
            var selectedList = selectedRows?.ToList();
            List<FavoriteUiModel> filteredItemList = new List<FavoriteUiModel>();
            if (CartProductIdList != null && selectedList != null)
            {
                for (int i = 0; i < selectedList.Count; i++)
                {
                    int index = CartProductIdList.FindIndex(item => item == selectedList[i].ProductId);
                    if (index >= 0)
                    {
                        filteredItemList.Add(selectedList[i]);
                    }
                    else
                    {
                        filteredItemList.Clear();
                        string message = "You cannot add" + " " + selectedList[i].ItemDescription + " " + "product in the cart.";
                        await ShowProductCannotBeAddedToCart(message);
                        break;
                    }
                }
            }
            return filteredItemList;
        }

        private async Task ShowProductCannotBeAddedToCart(string message)
        {

            ContentDialog productCannotBeAddedDialog = new ContentDialog
            {
                Content = message,
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await productCannotBeAddedDialog.ShowAsync();
        }


        private async Task AddItemsToCart(List<FavoriteUiModel> selectedRows)
        {
            List<FavoriteUiModel> favoriteList = new List<FavoriteUiModel>();
            List<OrderDetail> cartList = new List<OrderDetail>();

            foreach (var item in selectedRows)
            {
                var matchedItem = DbCartDataSource.FirstOrDefault(x => x.ProductID == item.ProductId);
                if (matchedItem != null)
                {
                    string message = matchedItem.ItemDescription + " " + "product is already in the cart.";
                    await ShowProductAlreadyInCart (message);
                    favoriteList.Clear();
                    return;
                }
                else
                {
                    favoriteList.Add(item);
                }
            }
            if (favoriteList?.Count > 0)
            {
                await AddCartItemsToDb(favoriteList, cartList);
            }
        }
        private static async Task InsertEntryIntoOrderMaster()
        {
            try
            {
                var orderMasterRecord = new OrderMaster();

                orderMasterRecord.IsExported = 1;

                var newOrderAdded = await ((App)Application.Current).QueryService.InsertOrUpdateOrderMaster(orderMasterRecord);

                if (newOrderAdded != null)
                {
                    ((App)Application.Current).CurrentOrderId = newOrderAdded.OrderID;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(FavoritePageViewModel), nameof(InsertEntryIntoOrderMaster), ex.Message);
            }

        }
        private async Task AddCartItemsToDb(List<FavoriteUiModel> favoriteList, List<OrderDetail> cartList)
        {
            foreach (var favoriteUiModel in favoriteList)
            {
                if (AppRef.CurrentOrderId == 0)
                {
                    await InsertEntryIntoOrderMaster();
                }
                var categoryInfo = DbCategoryDataSource.FirstOrDefault(x => x.CategoryID == favoriteUiModel.CategoryId);

                favoriteUiModel.FavoriteToOrderDetailModel();
                favoriteUiModel.OrderDetailMasterData.OrderId = ((App)Application.Current).CurrentOrderId;
                favoriteUiModel.OrderDetailMasterData.DeviceOrderID = Convert.ToString(((App)Application.Current).CurrentOrderId);
                favoriteUiModel.OrderDetailMasterData.Total = "0";
                favoriteUiModel.OrderDetailMasterData.Quantity = 1;
                SetUomForCategoryId(categoryInfo, favoriteUiModel.OrderDetailMasterData);

                cartList.Add(favoriteUiModel.OrderDetailMasterData);
                await ((App)Application.Current).QueryService.InsertOrUpdateOrderDetail(favoriteUiModel.OrderDetailMasterData);
                BadgeCount++;
            }
            UpdateBadgeText();
            ((App)Application.Current).CartItemCount = BadgeCount;
            IsAllRowsUnselected();
            await ShowSuccessfullyAddedToCart();
        }
        private void SetUomForCategoryId(CategoryMaster categoryInfo, OrderDetail orderDetail)
        {
            switch (categoryInfo?.ERPCategoryId)
            {
                case 1010:
                case 1030:
                case 1031:
                case 1042:
                    orderDetail.Unit = "BX";
                    break;
                case 1020:
                case 1021:
                case 1040:
                case 1041:
                case 1043:
                case 1070:
                    orderDetail.Unit = "EA";
                    break;
                default:
                    orderDetail.Unit = "CA";
                    break;
            }
        }
        private void UpdateBadgeText()
        {
            if (BadgeCount <= 0)
            {
                IsBadgeVisible = Visibility.Collapsed;
            }
            else
            {
                IsBadgeVisible = Visibility.Visible;
                BadgeText = BadgeCount.ToString();
            }
        }

        private async Task ShowNoCustomerSelected()
        {
            ContentDialog noCustomerSelectedDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("NoCustomerSelectedText"),
                PrimaryButtonText = resourceLoader.GetString("SelectCustomer"),
                SecondaryButtonText = resourceLoader.GetString("Cancel")
            };

            ContentDialogResult result = await noCustomerSelectedDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                NavigationService.Navigate<CustomerPage>();
            }
            else
            {
                noCustomerSelectedDialog.Hide();
            }
        }

        private async Task ShowNoProductSelected()
        {
            ContentDialog noProductSelectedDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("SelectAnyProduct"),
                CloseButtonText = resourceLoader.GetString("OK"),
            };

            await noProductSelectedDialog.ShowAsync();
        }

        private async Task ShowProductAlreadyInCart(string message)
        {
            ContentDialog productAlreadyInCartDialog = new ContentDialog
            {
                Content = message,
                CloseButtonText = resourceLoader.GetString("OK"),
            };

            await productAlreadyInCartDialog.ShowAsync();
        }



        private async Task ShowSuccessfullyAddedToCart()
        {
            ContentDialog successfullyAddedToCartDialog = new ContentDialog
            {
                Title = "Success",
                Content = "Added to cart Successfully.",
                CloseButtonText = resourceLoader.GetString("OK"),
            };

            await successfullyAddedToCartDialog.ShowAsync();
        }
        private async Task EmailFactsheetButtonCommandHandler()
        {
            try
            {
                var selectedList = FavoriteGridDataSource.Where(x => x.IsAllRowsSelected);
                var total = selectedList.Count();

                if (total > 5)
                {
                    await AlertHelper.Instance.ShowConfirmationAlert(resourceLoader.GetString("ALERT"), resourceLoader.GetString("CART_FACT_SHEET_LESS_THAN_5_ITEM_ALERT"), resourceLoader.GetString("OK"));
                }
                else if (total > 0)
                {
                    if (AppRef.LocalFileService.IsSrcZipFolderExist(Core.Interface.SrcZipFileType.Product))
                    {
                        IsLoading = true;
                        var listOfFilePath = new List<string>();
                        var listOfProductNumber = new List<string>();

                        foreach (var item in selectedList)
                        {
                            var docs = await GetAdditionalDocuments(item.ProductId);
                            var path = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, docs.Factsheet);
                            if (!string.IsNullOrWhiteSpace(path))
                            {
                                listOfFilePath.Add(path);
                                listOfProductNumber.Add(item.ItemNumber);
                            }
                        }
                        await EmailService.Instance.SendMailFromOutlook(new EmailModel() { Subject = string.Join(',', listOfProductNumber), AttachmentListByPath = listOfFilePath });
                        await Task.Delay(10000);
                        IsLoading = false;
                    }
                    else
                    {
                        await ShowConfirmationAlert(ResourceExtensions.GetLocalized("ALERT"), Helpers.ResourceExtensions.GetLocalized("FileDoesNotExist"), Helpers.ResourceExtensions.GetLocalized("OK"));
                    }
                }
                else
                {
                    await AlertHelper.Instance.ShowConfirmationAlert(resourceLoader.GetString("ALERT"), resourceLoader.GetString("SelectAnyProduct"), resourceLoader.GetString("OK"), string.Empty);
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                ErrorLogger.WriteToErrorLog(nameof(FavoritePageViewModel), nameof(EmailFactsheetButtonCommandHandler), ex.StackTrace);
            }
        }

        private async Task CustomerTitlePanelClicked()
        {
            if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId))
            {
                await ShowRemoveCustomerWarning();
            }
        }
        #endregion

        #region Akshay
        public bool _isNotARowSelection { get; set; }
        public ICommand ItemClickCommand { get; private set; }
        public ICommand ShowHideDetailCommand { get; private set; }
        public ICommand GridRowChecBoxCheckChangeCommand { get; private set; }
        public ICommand PrevoiusNextCommand { get; private set; }
        public ICommand ClosePreviewCommand { get; private set; }
        public ICommand ShowPreviewCommand { get; private set; }
        public ICommand DetailsActionCommand { get; private set; }
        public ICommand MailProductDetailsCommand { get; private set; }


        private readonly SolidColorBrush Gray = new SolidColorBrush(Colors.LightGray);
        private readonly SolidColorBrush Black = new SolidColorBrush(Colors.Black);

        private bool _isProductDetailsVisible;
        public bool IsProductDetailsVisible
        {
            get { return _isProductDetailsVisible; }
            set { SetProperty(ref _isProductDetailsVisible, value); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }


        private SolidColorBrush _leftArrowColor;
        public SolidColorBrush LeftArrowColor
        {
            get { return _leftArrowColor; }
            set { SetProperty(ref _leftArrowColor, value); }
        }

        private SolidColorBrush _rightArrowColor;
        public SolidColorBrush RightArrowColor
        {
            get { return _rightArrowColor; }
            set { SetProperty(ref _rightArrowColor, value); }
        }


        private FavoriteDetailPageUIModel _detailUIModel;
        public FavoriteDetailPageUIModel DetailUIModel
        {
            get { return _detailUIModel; }
            set { SetProperty(ref _detailUIModel, value); }
        }


        private string _previewUrl;
        public string PreviewUrl
        {
            get { return _previewUrl; }
            set { SetProperty(ref _previewUrl, value); }
        }

        private bool _isPreviewDocumentControlVisible;
        public bool IsPreviewDocumentControlVisible
        {
            get { return _isPreviewDocumentControlVisible; }
            set { SetProperty(ref _isPreviewDocumentControlVisible, value); }
        }


        private string _previewFilePath;
        public string PreviewFilePath
        {
            get { return _previewFilePath; }
            set { SetProperty(ref _previewFilePath, value); }
        }





        private async Task ItemClickCommandHandler(FavoriteUiModel obj)
        {
            if (_isNotARowSelection)
            {
                _isNotARowSelection = false;
            }
            else
            {
                /// show the detail page
                var details = await GetAdditionalDocuments(obj.ProductId);
                DetailUIModel.Factsheet = details.Factsheet;
                DetailUIModel.ProductImage = details.ProductImage;
                DetailUIModel.RetailImage = details.RetailImage;
                DetailUIModel.IpImage = details.IpImage;
                DetailUIModel.SelectedFavorite = obj;

                var productImage = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, Core.Helpers.HelperMethods.GetNameFromURL(details.ProductImage));
                if (!string.IsNullOrWhiteSpace(productImage))
                {
                    DetailUIModel.ProductImagePath = productImage;
                }
                var isInCart = DbCartDataSource.Any(x => x.ProductID == obj.ProductId);
                DetailUIModel.IsInCart = isInCart;
                DetailUIModel.CartImagePath = GetCartImage(isInCart);

                var currentIndex = FavoriteGridDataSource.IndexOf(obj);
                HandleArrowColors(currentIndex);
                ShowHideDetailCommandHandler();
            }
        }

        private void HandleArrowColors(int currentIndex)
        {
            LeftArrowColor = currentIndex == 0 ? Gray : Black;
            RightArrowColor = currentIndex == (FavoriteGridDataSource.Count - 1) ? Gray : Black;
        }

        private void ShowHideDetailCommandHandler()
        {
            IsProductDetailsVisible = !IsProductDetailsVisible;
        }

        private void GridRowChecBoxCheckChangeCommandHandler()
        {
            _isNotARowSelection = true;
        }


        private async Task PrevoiusNextCommandHandler(string type)
        {
            var currentIndex = FavoriteGridDataSource.IndexOf(DetailUIModel.SelectedFavorite);
            FavoriteUiModel nextPreviousObject = null;
            if ("PRE".Equals(type))
            {
                nextPreviousObject = currentIndex == 0 ? null : FavoriteGridDataSource[currentIndex - 1];
                if (nextPreviousObject != null)
                {
                    DetailUIModel.SelectedFavorite = nextPreviousObject;
                    HandleArrowColors(currentIndex - 1);
                }
            }
            else if ("NEXT".Equals(type))
            {
                nextPreviousObject = currentIndex == FavoriteGridDataSource.Count - 1 ? null : FavoriteGridDataSource[currentIndex + 1];
                if (nextPreviousObject != null)
                {
                    DetailUIModel.SelectedFavorite = nextPreviousObject;
                    HandleArrowColors(currentIndex + 1);
                }
            }


            if (nextPreviousObject != null)
            {
                var isInCart = DbCartDataSource.Any(x => x.ProductID == nextPreviousObject.ProductId);
                DetailUIModel.IsInCart = isInCart;
                DetailUIModel.CartImagePath = GetCartImage(isInCart);
                var deatils = await GetAdditionalDocuments(nextPreviousObject.ProductId);
                DetailUIModel.Factsheet = deatils.Factsheet;
                DetailUIModel.ProductImage = deatils.ProductImage;
                DetailUIModel.RetailImage = deatils.RetailImage;

                var productImage = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, Core.Helpers.HelperMethods.GetNameFromURL(deatils.ProductImage));
                if (!string.IsNullOrWhiteSpace(productImage))
                {
                    DetailUIModel.ProductImagePath = productImage;
                }
            }
        }

        private string GetCartImage(bool isInCart)
        {
            if (isInCart)
            {
                return (string)Application.Current.Resources["CartNormal"];
            }
            else
            {
                return (string)Application.Current.Resources["CartHover"];
            }
        }

        private async Task<ProductDetailUiModel> GetAdditionalDocuments(int productID)
        {
            var productDetailUiModel = await ((App)Application.Current).QueryService.GetProductAdditionalDocumentData(productID);
            return productDetailUiModel;
        }


        private void ClosePreviewCommandHandler()
        {
            IsPreviewDocumentControlVisible = false;
        }


        private void ShowPreviewCommandHandler()
        {
            ///IsPreviewDocumentControlVisible = true;
            throw new NotImplementedException();
        }

        private async Task DetailsCartCommandHandler()
        {
            if (DetailUIModel.IsInCart)
            {
                await DeleteCartItemFromDb(DetailUIModel.SelectedFavorite.ProductId, AppRef.CurrentOrderId);
                DetailUIModel.IsInCart = false;
                DetailUIModel.CartImagePath = GetCartImage(false);
                BadgeCount = BadgeCount == 0 ? 0 : BadgeCount - 1;
                AppRef.CartItemCount = BadgeCount;
            }
            else
            {
                if (string.IsNullOrEmpty(AppRef.SelectedCustomerId))
                {
                    await ShowNoCustomerSelected();
                }
                else
                {
                    await AddItemsToCart(new List<FavoriteUiModel>() { DetailUIModel.SelectedFavorite });
                    DetailUIModel.IsInCart = false;
                    DetailUIModel.CartImagePath = GetCartImage(true);
                    AppRef.CartItemCount = BadgeCount;
                }

            }
            UpdateBadgeText();
            await Task.Delay(200);
            DbCartDataSource = await ((App)Application.Current).QueryService.GetCartProductDetailsData(AppRef.CurrentOrderId);
        }


        private async Task DetailsActionCommandHandler(string command)
        {
            string filePath = string.Empty;
            switch (command)
            {
                case "FACTSHEET":
                    filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, DetailUIModel.Factsheet);
                    break;
                case "RETAIL_IMAGE":
                    filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, DetailUIModel.RetailImage);
                    break;
                case "IP_IMAGE":
                    filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, DetailUIModel.IpImage);
                    break;
                case "PRODUCT":
                    filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, DetailUIModel.ProductImage);
                    break;
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                await ShowConfirmationAlert(ResourceExtensions.GetLocalized("ALERT"), Helpers.ResourceExtensions.GetLocalized("FileDoesNotExist"), Helpers.ResourceExtensions.GetLocalized("OK"));
            }
            else
            {
                PreviewFilePath = filePath;
                IsPreviewDocumentControlVisible = true;
            }
        }


        private async Task<bool> ShowConfirmationAlert(string title, string msg, string primaryButton, string secondaryButton = "")
        {
            ContentDialog userLogoutDialog = new ContentDialog
            {
                Title = title,
                Content = msg,
                PrimaryButtonText = primaryButton,
                SecondaryButtonText = secondaryButton
            };

            var result = await userLogoutDialog.ShowAsync();
            userLogoutDialog.Hide();
            if (result == ContentDialogResult.Primary)
            {
                return true;
            }
            else return false;

        }

        private async Task MailProductDetailsCommandHandler()
        {
            try
            {
                if (AppRef.LocalFileService.IsSrcZipFolderExist(Core.Interface.SrcZipFileType.Product))
                {
                    IsLoading = true;
                    var listOfFilePath = new List<string>();
                    listOfFilePath.Add(AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, DetailUIModel.Factsheet));
                    listOfFilePath.Add(AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, DetailUIModel.RetailImage));
                    listOfFilePath.Add(AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, DetailUIModel.IpImage));
                    listOfFilePath.Add(AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, DetailUIModel.ProductImage));
                    await EmailService.Instance.SendMailFromOutlook(new EmailModel() { Subject = DetailUIModel.SelectedFavorite.ItemNumber, AttachmentListByPath = listOfFilePath });
                    await Task.Delay(10000);
                    IsLoading = false;
                }
                else
                {
                    await ShowConfirmationAlert(ResourceExtensions.GetLocalized("ALERT"), Helpers.ResourceExtensions.GetLocalized("FileDoesNotExist"), Helpers.ResourceExtensions.GetLocalized("OK"));
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                ErrorLogger.WriteToErrorLog(nameof(FavoritePageViewModel), nameof(MailProductDetailsCommandHandler), ex.StackTrace);
            }
        }

        #endregion
    }
}
