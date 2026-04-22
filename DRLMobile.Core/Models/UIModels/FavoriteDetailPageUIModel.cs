using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class FavoriteDetailPageUIModel : BaseModel
    {
        private FavoriteUiModel _selectedFavorite;
        private readonly string PlaceholderImage = "ms-appx:///Assets/SRCProduct/no_product.png";
        public FavoriteUiModel SelectedFavorite
        {
            get { return _selectedFavorite; }
            set { SetProperty(ref _selectedFavorite, value); }
        }

        public string Factsheet { get; set; }
        public string RetailImage { get; set; }
        public string ProductImage { get; set; }
        public string IpImage { get; set; }
        public bool IsInCart { get; set; }

        public FavoriteDetailPageUIModel()
        {
            SelectedFavorite = new FavoriteUiModel();
            ProductImagePath = PlaceholderImage;
        }


        private string _productImagePath ;
        public string ProductImagePath
        {
            get { return _productImagePath; }
            set { SetProperty(ref _productImagePath, value); }
        }



        private string _cartImagePath;
        public string CartImagePath
        {
            get { return _cartImagePath; }
            set { SetProperty(ref _cartImagePath, value); }
        }

        //NoProduct

    }
}
