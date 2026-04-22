using DRLMobile.Core.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class OrderDetailUIModel : BaseModel
    {

        private OrderDetail _orderDetailData;
        public OrderDetail OrderDetailData
        {
            get { return _orderDetailData; }
            set { SetProperty(ref _orderDetailData, value); }
        }

        private ObservableCollection<string> _rtnDifCreditRequestComboboxList;
        public ObservableCollection<string> RtnDifCreditRequestComboboxList
        {
            get { return _rtnDifCreditRequestComboboxList; }
            set { SetProperty(ref _rtnDifCreditRequestComboboxList, value); }
        }

        private string _itemNumber;
        public string ItemNumber
        {
            get { return _itemNumber; }
            set { SetProperty(ref _itemNumber, value); }
        }

        private string _itemDescription;
        public string ItemDescription
        {
            get { return _itemDescription; }
            set { SetProperty(ref _itemDescription, value); }
        }

        private int _categoryId;
        public int CategoryId
        {
            get { return _categoryId; }
            set { SetProperty(ref _categoryId, value); }
        }

        private string _categoryName;
        public string CategoryName
        {
            get { return _categoryName; }
            set { SetProperty(ref _categoryName, value); }
        }

        private int _brandId;
        public int BrandId
        {
            get { return _brandId; }
            set { SetProperty(ref _brandId, value); }
        }

        private string _brandName;
        public string BrandName
        {
            get { return _brandName; }
            set { SetProperty(ref _brandName, value); }
        }
        private int _styleId;
        public int StyleId
        {
            get { return _styleId; }
            set { SetProperty(ref _styleId, value); }
        }
        private string _styleName;
        public string StyleName
        {
            get { return _styleName; }
            set { SetProperty(ref _styleName, value); }
        }

        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set { SetProperty(ref _productName, value); }
        }
        private int _isTobbaco;
        public int isTobbaco
        {
            get { return _isTobbaco; }
            set { SetProperty(ref _isTobbaco, value); }
        }
        private int _productID;
        public int ProductID
        {
            get { return _productID; }
            set { SetProperty(ref _productID, value); }
        }

        private string _productDescription;
        public string ProductDescription
        {
            get { return _productDescription; }
            set { SetProperty(ref _productDescription, value); }
        }

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }

        private string _quantityDisplay;
        public string QuantityDisplay
        {
            get { return _quantityDisplay; }
            set { SetProperty(ref _quantityDisplay, value); }
        }

        private string _price;
        public string Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }

        private string _priceDisplay;
        public string PriceDisplay
        {
            get { return _priceDisplay; }
            set { SetProperty(ref _priceDisplay, value); }
        }

        private string _createdDate;
        public string CreatedDate
        {
            get { return _createdDate; }
            set { SetProperty(ref _createdDate, value); }
        }
        private string _updatedDate;
        public string UpdatedDate
        {
            get { return _updatedDate; }
            set { SetProperty(ref _updatedDate, value); }
        }
        private string _unit;
        public string Unit
        {
            get { return _unit; }
            set { SetProperty(ref _unit, value); }
        }
        private string _total;
        public string Total
        {
            get { return _total; }
            set { SetProperty(ref _total, value); }
        }
        private string _totalDisplay;
        public string TotalDisplay
        {
            get { return _totalDisplay; }
            set { SetProperty(ref _totalDisplay, value); }
        }
        private string _subTotalUpperDisplay;
        public string SubTotalUpperDisplay
        {
            get { return _subTotalUpperDisplay; }
            set { SetProperty(ref _subTotalUpperDisplay, value); }
        }
        private string _subTotalLowerDisplay;
        public string SubTotalLowerDisplay
        {
            get { return _subTotalLowerDisplay; }
            set { SetProperty(ref _subTotalLowerDisplay, value); }
        }
        private string _grandTotalDisplay;
        public string GrandTotalDisplay
        {
            get { return _grandTotalDisplay; }
            set { SetProperty(ref _grandTotalDisplay, value); }
        }
        private string _deviceOrderID;
        public string DeviceOrderID
        {
            get { return _deviceOrderID; }
            set { SetProperty(ref _deviceOrderID, value); }
        }
        private int? _orderId;
        public int? OrderId
        {
            get { return _orderId; }
            set { SetProperty(ref _orderId, value); }
        }
        private int? _orderDetailId;
        public int? OrderDetailId
        {
            get { return _orderDetailId; }
            set { SetProperty(ref _orderDetailId, value); }
        }
        private string _creditRequest;
        public string CreditRequest
        {
            get { return _creditRequest; }
            set { SetProperty(ref _creditRequest, value); }
        }
        private bool _isAllUpperGridSelected;
        public bool IsAllUpperGridSelected
        {
            get { return _isAllUpperGridSelected; }
            set { SetProperty(ref _isAllUpperGridSelected, value); }
        }

        private bool _isAllLowerGridSelected;
        public bool IsAllLowerGridSelected
        {
            get { return _isAllLowerGridSelected; }
            set { SetProperty(ref _isAllLowerGridSelected, value); }
        }
        private bool _isQuantityEdited;
        public bool IsQuantityEdited
        {
            get { return _isQuantityEdited; }
            set { SetProperty(ref _isQuantityEdited, value); }
        }
        private bool _isPriceEdited;
        public bool IsPriceEdited
        {
            get { return _isPriceEdited; }
            set { SetProperty(ref _isPriceEdited, value); }
        }
        
        public void OrderDetailUiToDataModel()
        {
            OrderDetailData = new OrderDetail();
            OrderDetailData.ProductName = ItemNumber;
            OrderDetailData.ProductDescription = ItemDescription;
            OrderDetailData.CategoryId = CategoryId;
            OrderDetailData.BrandId = BrandId;
            OrderDetailData.StyleId = StyleId;
            OrderDetailData.BrandId = BrandId;
            OrderDetailData.Unit = Unit;
            OrderDetailData.ProductId = ProductID;
            OrderDetailData.isTobbaco = isTobbaco;
            OrderDetailData.DeviceOrderID = DeviceOrderID;
            OrderDetailData.OrderId = OrderId;
            OrderDetailData.OrderDetailId = OrderDetailId;
            OrderDetailData.Price = Price;
            OrderDetailData.Quantity = Quantity;
            OrderDetailData.BrandName = BrandName;
            OrderDetailData.StyleName = StyleName;
            OrderDetailData.CategoryName = CategoryName;
            OrderDetailData.Total = Total;
            OrderDetailData.CreditRequest = CreditRequest;
            OrderDetailData.CreatedDate = CreatedDate;
        }
    }
}