using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media;

namespace DRLMobile.Core.Models.UIModels
{
    public class MapsLegendFilterUIModel : BaseModel
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private SolidColorBrush _backgroundColor;
        public SolidColorBrush BackgroundColor
        {
            get { return _backgroundColor; }
            set { SetProperty(ref _backgroundColor, value); }
        }

        public List<int> AccountClassificationIds { get; set; }

        public int Tag { get; set; }
        public string Rank { get; set; }
        public string MapIconImagePath { get; set; }
    }
}
