using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class ProductMasterUIModel:BaseModel
    {
        private int _productID;
        public int ProductID
        {
            get { return _productID; }
            set {SetProperty(ref _productID , value); }
        }


        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }



        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set { SetProperty(ref _productName, value); }
        }

    }
}
