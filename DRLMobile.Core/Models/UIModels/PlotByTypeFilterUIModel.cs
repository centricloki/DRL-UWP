using DRLMobile.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class PlotByTypeFilterUIModel : BaseModel
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set {SetProperty(ref _title , value); }
        }


        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public MapFilter Tag { get; set; }

    }
}
