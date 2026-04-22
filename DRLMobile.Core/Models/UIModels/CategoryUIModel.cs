using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;

namespace DRLMobile.Core.Models.UIModels
{
    public class CategoryUIModel : BaseModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
        private string categoryImage;
        public string CategoryImage
        {
            get { return categoryImage; }
            set
            {
                if (categoryImage != value)
                {
                    categoryImage = value;
                    OnPropertyChanged("CategoryImage");
                }
            }
        }
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                SetProperty(ref isSelected, value);
                OnPropertyChanged("CategoryImageSource");
            }
        }
        private int _ERPCategoryId;
        public int ERPCategoryId
        {
            get { return _ERPCategoryId; }
            set { SetProperty(ref _ERPCategoryId, value); }
        }

        public ImageSource CategoryImageSource
        {
            get
            {
                return new BitmapImage(new Uri($"ms-appx:///Assets/SRCProduct/category_{(IsSelected ? "" : "un")}selected.png"));
            }
        }
    }
}
