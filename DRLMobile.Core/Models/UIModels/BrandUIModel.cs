using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using DRLMobile.Core.Helpers;
using System.IO;

namespace DRLMobile.Core.Models.UIModels
{
    public class BrandUIModel : BaseModel
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        private string brandImage;
        public string BrandImage
        {
            get { return brandImage; }
            set
            {
                if (brandImage != value)
                {
                    brandImage = value;
                    OnPropertyChanged("BrandImage");
                }
            }
        }
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); OnPropertyChanged("SelectedImageSource"); }
        }
        public int CatId { get; set; }
        public int Status { get; set; }
        public string ImageFileName { get; set; }
        public string LocalFilePath { get; set; }
        public int IsDownload { get; set; }
        public string Description { get; set; }
        public int IsDeleted { get; set; }
        public int IsPopOrder { get; set; }
        public int SortOrder { get; set; }

        private string _brandImageFromServer;
        public string BrandImageFromLocal
        {
            get { return _brandImageFromServer; }
            set { SetProperty(ref _brandImageFromServer, value); OnPropertyChanged("BrandImageSource"); }
        }

        private string _selectedImage;
        public string SelectedImage
        {
            get { return _selectedImage; }
            set { SetProperty(ref _selectedImage, value); }
        }
        public ImageSource SelectedImageSource
        => new BitmapImage(new Uri($"ms-appx:///Assets/SRCProduct/category_{(IsSelected ? "" : "un")}selected.png"));

        //public ImageSource BrandImageSource
        //=> new BitmapImage(new Uri(BrandImageFromLocal));

        public ImageSource BrandImageSource
        {
            get
            {
                BitmapImage result = new BitmapImage(new Uri($"ms-appx:///Assets/SRCProduct/no_product.png"));
                if (File.Exists(BrandImageFromLocal))
                {
                    result = new BitmapImage(new Uri(BrandImageFromLocal));
                }
                return result;
            }
        }

    }
}
