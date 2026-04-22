using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class RackOrderUiModel : BaseModel
    {
        private readonly string PlaceholderImage = "ms-appx:///Assets/RackOrder/rack_item_placeholder.png";
        public RackOrderUiModel()
        {
            ProductImagePath = PlaceholderImage;
        }
        private int _productId;
        public int ProductID
        {
            get { return _productId; }
            set { SetProperty(ref _productId, value); }
        }


        private string _documentFileName;
        public string DocumentFileName
        {
            get { return _documentFileName; }
            set { SetProperty(ref _documentFileName, value); }
        }
        private string _documentType;
        public string DocumentType
        {
            get { return _documentType; }
            set { SetProperty(ref _documentType, value); }
        }


        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        private int _brandId;
        public int BrandId
        {
            get { return _brandId; }
            set { SetProperty(ref _brandId, value); }
        }
        private int _catId;
        public int CatId
        {
            get { return _catId; }
            set { SetProperty(ref _catId, value); }
        }
        private int _status;

        public int Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
        private string _updateDate;
        public string UpdateDate
        {
            get { return _updateDate; }
            set { SetProperty(ref _updateDate, value); }
        }
        private string _imageFileName;
        public string ImageFileName
        {
            get { return _imageFileName; }
            set { SetProperty(ref _imageFileName, value); }
        }
        private string _brandName;
        public string BrandName
        {
            get { return _brandName; }
            set { SetProperty(ref _brandName, value); }
        }
        private int _languageId;

        public int LanguageId
        {
            get { return _languageId; }
            set { SetProperty(ref _languageId, value); }
        }
        private int _priority;

        public int Priority
        {
            get { return _priority; }
            set { SetProperty(ref _priority, value); }
        }
        private int _promoId;

        public int PromoId
        {
            get { return _promoId; }
            set { SetProperty(ref _promoId, value); }
        }
        private string _localFilePath;

        public string LocalFilePath
        {
            get { return _localFilePath; }
            set { SetProperty(ref _localFilePath, value); }
        }

        private int _isDownload;

        public int IsDownload
        {
            get { return _isDownload; }
            set { SetProperty(ref _isDownload, value); }
        }
        private int _isDeleted;

        public int IsDeleted
        {
            get { return _isDeleted; }
            set { SetProperty(ref _isDeleted, value); }
        }
        private int _isPopOrder;

        public int IsPopOrder
        {
            get { return _isPopOrder; }
            set { SetProperty(ref _isPopOrder, value); }
        }
        private int _sortOrder;

        public int SortOrder
        {
            get { return _sortOrder; }
            set { SetProperty(ref _sortOrder, value); }
        }

        private string _productImage;
        public string ProductImage
        {
            get { return _productImage; }
            set
            {
                SetProperty(ref _productImage, value);
            }
        }
        private string _productImagePath;
        public string ProductImagePath
        {
            get { return _productImagePath; }
            set { SetProperty(ref _productImagePath, value); }
        }
    }
}
