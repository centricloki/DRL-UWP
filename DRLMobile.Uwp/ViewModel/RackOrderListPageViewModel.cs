using DRLMobile.Core.Models;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.ViewModel
{
    public class RackOrderListPageViewModel : BaseModel
    {
        #region Properties

        private readonly App AppRef = (App)Application.Current;

        private string _customerNameNumber = string.Empty;
        public string CustomerNameNumber
        {
            get { return _customerNameNumber; }
            set { SetProperty(ref _customerNameNumber, value); }
        }

        private string _customerAddresss = string.Empty;
        public string CustomerAddresss
        {
            get { return _customerAddresss; }
            set { SetProperty(ref _customerAddresss, value); }
        }

        private string _customerCityState = string.Empty;
        public string CustomerCityState
        {
            get { return _customerCityState; }
            set { SetProperty(ref _customerCityState, value); }
        }
        private Visibility _customerTitlePanelVisibility = Visibility.Collapsed;
        public Visibility CustomerTitlePanelVisibility
        {
            get { return _customerTitlePanelVisibility; }
            set { SetProperty(ref _customerTitlePanelVisibility, value); }
        }
        private RackOrderUiModel _rackOrderUiModel;
        public RackOrderUiModel RackOrderUiModel
        {
            get { return _rackOrderUiModel; }
            set { SetProperty(ref _rackOrderUiModel, value); }
        }
        private ObservableCollection<RackOrderUiModel> _rackOrderListGridDataSource;
        public ObservableCollection<RackOrderUiModel> RackOrderListGridDataSource
        {
            get { return _rackOrderListGridDataSource; }
            set { SetProperty(ref _rackOrderListGridDataSource, value); }
        }
        private List<RackOrderUiModel> _dbRackOrderListDataSource;
        public List<RackOrderUiModel> DbRackOrderListDataSource
        {
            get { return _dbRackOrderListDataSource; }
            set { SetProperty(ref _dbRackOrderListDataSource, value); }
        }

        private bool _isPreviewDocumentVisibile;
        public bool IsPreviewDocumentVisibile
        {
            get { return _isPreviewDocumentVisibile; }
            set { SetProperty(ref _isPreviewDocumentVisibile, value); }
        }

        private string _previewUrl;
        public string PreviewUrl
        {
            get { return _previewUrl; }
            set { SetProperty(ref _previewUrl, value); }
        }

        private PhotosPrintHelper _printHelper;
        public PhotosPrintHelper PrintHelper
        {
            get { return _printHelper; }
            set { SetProperty(ref _printHelper, value); }
        }

        #endregion

        #region Commands

        public ICommand PageLoadedCommand { private set; get; }
        public ICommand NavigateToRackCartScreenCommand { private set; get; }
        public ICommand ClosePreviewCommand { get; private set; }
        public ICommand PrintRackPOSImageCommand { get; private set; }

        #endregion

        #region Constructor

        public RackOrderListPageViewModel()
        {
            RackOrderListGridDataSource = new ObservableCollection<RackOrderUiModel>();
            DbRackOrderListDataSource = new List<RackOrderUiModel>();
            IsPreviewDocumentVisibile = false;

            PageLoadedCommand = new AsyncRelayCommand(LoadInitialPageData);
            NavigateToRackCartScreenCommand = new RelayCommand<RackOrderUiModel>(NavigateToRackCartScreen);
            ClosePreviewCommand = new RelayCommand(ClosePreviewCommandHandler);
            PrintRackPOSImageCommand = new AsyncRelayCommand(PrintPOPImageCommandHandler);
        }

        #endregion

        #region Private Methods
        private async Task LoadInitialPageData()
        {
            ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = false;
            }

            try
            {
                IsPreviewDocumentVisibile = false;
                await FetchOrderListData();
                if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId.Trim()))
                {

                    int customerId = Convert.ToInt32(AppRef.SelectedCustomerId);

                    var selectedCustomer = await ((App)Application.Current).QueryService.GetSavedCustomerInformation(customerId);

                    if (selectedCustomer != null)
                    {
                        CustomerNameNumber = selectedCustomer.CustomerName + " " + (!string.IsNullOrWhiteSpace(selectedCustomer.CustomerNumber) ? selectedCustomer.CustomerNumber : "");
                        CustomerAddresss = selectedCustomer.PhysicalAddress;
                        CustomerCityState = selectedCustomer.SubAddressText;
                        CustomerTitlePanelVisibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "LoadInitialPageData", ex.StackTrace);
            }

            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = true;
            }

        }
        //private async Task FetchOrderListData()
        //{
        //    DbRackOrderListDataSource = await ((App)Application.Current).QueryService.GetRackOrderListData();
        //    if (DbRackOrderListDataSource?.Count > 0)
        //    {

        //        foreach (var item in DbRackOrderListDataSource)
        //        {
        //            var details = await GetAdditionalDocuments(item.ProductID);
        //            var productImage = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, Core.Helpers.HelperMethods.GetNameFromURL(details.ProductImage));
        //            if (!string.IsNullOrWhiteSpace(productImage))
        //            {
        //                item.ProductImagePath = productImage;
        //            }
        //            RackOrderListGridDataSource.Add(item);
        //        }
        //    }
        //}

        private async Task FetchOrderListData()
        {
            DbRackOrderListDataSource = await ((App)Application.Current).QueryService.GetRackOrderListData();

            if (DbRackOrderListDataSource?.Count > 0)
            {
                string productImage = string.Empty;



                foreach (var item in DbRackOrderListDataSource)
                {
                    var details = await GetAdditionalDocuments(item.ProductID);



                    if (details != null)
                    {
                        productImage = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, Core.Helpers.HelperMethods.GetNameFromURL(details.ProductImage));
                    }



                    if (!string.IsNullOrWhiteSpace(productImage))
                    {
                        item.ProductImagePath = productImage;
                    }



                    RackOrderListGridDataSource.Add(item);
                }
            }
        }
        private async Task<ProductDetailUiModel> GetAdditionalDocuments(int productID)
        {
            var productDetailUiModel = await ((App)Application.Current).QueryService.GetProductAdditionalDocumentData(productID);
            return productDetailUiModel;
        }

        private void NavigateToRackCartScreen(RackOrderUiModel RackOrderUiModel)
        {
            NavigationService.NavigateShellFrame(typeof(RackOrderCartPage), RackOrderUiModel);
        }

        private void ClosePreviewCommandHandler()
        {
            if (PrintHelper != null)
            {
                PrintHelper.UnregisterForPrinting();
                PrintHelper = null;
            }

            IsPreviewDocumentVisibile = false;
        }

        private async Task PrintPOPImageCommandHandler()
        {
            // Initialize print content, by passing the file as IStorage File  
            if (!string.IsNullOrEmpty(PreviewUrl))
            {
                await PrintHelper.ShowPrintUIAsync();
            }
        }

        #endregion
    }
}
