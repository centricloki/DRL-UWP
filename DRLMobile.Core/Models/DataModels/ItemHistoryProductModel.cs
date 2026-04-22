using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.DataModels
{
    public class ItemHistoryProductModel : BaseModel
    {
        private int _productId;
        public int ProductID
        {
            get { return _productId; }
            set {SetProperty(ref _productId , value); }
        }


        private string  _productName;
        public string ProductName
        {
            get { return _productName; }
            set { SetProperty(ref _productName, value); }
        }


        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }


        private string _uom;
        public string UOM
        {
            get { return _uom; }
            set { SetProperty(ref _uom, value); }
        }

    }
}
