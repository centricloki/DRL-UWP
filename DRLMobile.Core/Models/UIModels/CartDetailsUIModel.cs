using DRLMobile.Core.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml;

namespace DRLMobile.Core.Models.UIModels
{
    public class CartDetailsUIModel : BaseModel
    {
        //private CustomerMaster _customerData;
        //public CustomerMaster CustomerData
        //{
        //    get { return _customerData; }
        //    set { SetProperty(ref _customerData, value); }
        //}

        private OrderDetailUIModel _orderDetailModel;
        public OrderDetailUIModel OrderDetailModel
        {
            get { return _orderDetailModel; }
            set { SetProperty(ref _orderDetailModel, value); }
        }

        private List<OrderDetailUIModel> _orderDetailListModel;
        public List<OrderDetailUIModel> OrderDetailsList
        {
            get { return _orderDetailListModel; }
            set { SetProperty(ref _orderDetailListModel, value); }
        }

        private Visibility _rtnGridVisiblity = Visibility.Collapsed;
        public Visibility RtnGridVisibility
        {
            get { return _rtnGridVisiblity; }
            set { SetProperty(ref _rtnGridVisiblity, value); }
        }

        private Visibility _difGridVisiblity = Visibility.Collapsed;
        public Visibility DifGridVisibility
        {
            get { return _difGridVisiblity; }
            set { SetProperty(ref _difGridVisiblity, value); }
        }

        private Visibility _tobaccoGridVisiblity = Visibility.Collapsed;
        public Visibility TobaccoGridVisibility
        {
            get { return _tobaccoGridVisiblity; }
            set { SetProperty(ref _tobaccoGridVisiblity, value); }
        }

        private Visibility _nonTobaccoGridVisiblity = Visibility.Collapsed;
        public Visibility NonTobaccoGridVisibility
        {
            get { return _nonTobaccoGridVisiblity; }
            set { SetProperty(ref _nonTobaccoGridVisiblity, value); }
        }

        private Visibility _emptyCartVisiblity = Visibility.Collapsed;
        public Visibility EmptyCartVisibility
        {
            get { return _emptyCartVisiblity; }
            set { SetProperty(ref _emptyCartVisiblity, value); }
        }

        private Visibility _noSelectedCustomerVisiblity = Visibility.Collapsed;
        public Visibility NoSelectedCustomerVisibility
        {
            get { return _noSelectedCustomerVisiblity; }
            set { SetProperty(ref _noSelectedCustomerVisiblity, value); }
        }

        private Visibility _grandTotalVisiblity = Visibility.Collapsed;
        public Visibility GrandTotalVisibility
        {
            get { return _grandTotalVisiblity; }
            set { SetProperty(ref _grandTotalVisiblity, value); }
        }

        private Visibility _bottomButtonVisiblity = Visibility.Collapsed;
        public Visibility BottomButtonVisiblity
        {
            get { return _bottomButtonVisiblity; }
            set { SetProperty(ref _bottomButtonVisiblity, value); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        public string CustomerNameNumber { get; set; }
        public int CustomerAccountType { get; set; }

        private string _loadingText;
        public string LoadingText
        {
            get { return _loadingText; }
            set { SetProperty(ref _loadingText, value); }
        }
 
        private decimal _uperGridSubTotal;
        public decimal UperGridSubTotal
        {
            get { return _uperGridSubTotal; }
            set { SetProperty(ref _uperGridSubTotal, value); }
        }

        private decimal _lowerGridSubTotal;
        public decimal LowerGridSubTotal
        {
            get { return _lowerGridSubTotal; }
            set { SetProperty(ref _lowerGridSubTotal, value); }
        }

        private ObservableCollection<OrderDetailUIModel> _rtnCreditRequestCbDataSource;
        public ObservableCollection<OrderDetailUIModel> RtnCreditRequestCbDataSource
        {
            get { return _rtnCreditRequestCbDataSource; }
            set { SetProperty(ref _rtnCreditRequestCbDataSource, value); }
        }

        private ObservableCollection<OrderDetailUIModel> _difCreditRequestCbDataSource;
        public ObservableCollection<OrderDetailUIModel> DifCreditRequestCbDataSource
        {
            get { return _difCreditRequestCbDataSource; }
            set { SetProperty(ref _difCreditRequestCbDataSource, value); }
        }

        //public void SetCustomerName()
        //{
        //    if (CustomerData != null)
        //    {
        //        CustomerNameNumber = CustomerData.CustomerName + " " + CustomerData.CustomerNumber;
        //    }
        //}

        public void ShowNoCustomerSelected()
        {
            NoSelectedCustomerVisibility = Visibility.Visible;
            TobaccoGridVisibility = Visibility.Collapsed;
            NonTobaccoGridVisibility = Visibility.Collapsed;
            RtnGridVisibility = Visibility.Collapsed;
            DifGridVisibility = Visibility.Collapsed;
            EmptyCartVisibility = Visibility.Collapsed;  
            GrandTotalVisibility = Visibility.Collapsed;
            BottomButtonVisiblity = Visibility.Collapsed;
        }

        public void ShowCartIsEmpty()
        {
            EmptyCartVisibility = Visibility.Visible;
            NoSelectedCustomerVisibility = Visibility.Collapsed;
            TobaccoGridVisibility = Visibility.Collapsed;
            NonTobaccoGridVisibility = Visibility.Collapsed;
            RtnGridVisibility = Visibility.Collapsed;
            DifGridVisibility = Visibility.Collapsed;
            GrandTotalVisibility = Visibility.Collapsed;
            BottomButtonVisiblity = Visibility.Collapsed;
        }

        public void PopulateCartDetailsUiModel()
        {
            OrderDetailModel = new OrderDetailUIModel();
           // SetCustomerName();
        }
    }
}
