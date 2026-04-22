using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;

namespace DRLMobile.Core.Models.UIModels
{
    public class StyleUIModel : BaseModel
    {
        public int StyleId { get; set; }
        public string StyleName { get; set; }
        private string styleImage;
        public string StyleImage
        {
            get { return styleImage; }
            set
            {
                if (styleImage != value)
                {
                    styleImage = value;
                    OnPropertyChanged("StyleImage");
                }
            }
        }
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); OnPropertyChanged("StyleImageSource"); }
        }
        public int CatId { get; set; }
        public int Status { get; set; }

        public ImageSource StyleImageSource => new BitmapImage(new Uri($"ms-appx:///Assets/SRCProduct/style_{(IsSelected ? "" : "un")}selected.png"));

    }
}
