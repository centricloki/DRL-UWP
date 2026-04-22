using DRLMobile.Core.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class PopOrderPageUiModel : BaseModel
    {
        private List<PopOrderCartUiModel> _popOrderCartUiModelList;
        public List<PopOrderCartUiModel> PopOrderCartUiModelList
        {
            get { return _popOrderCartUiModelList; }
            set { SetProperty(ref _popOrderCartUiModelList, value); }
        }
        private List<BrandUIModel> _categoryList;
        public List<BrandUIModel> CategoryList
        {
            get { return _categoryList; }
            set { SetProperty(ref _categoryList, value); }
        }
        private List<BrandUIModel> _materialList;
        public List<BrandUIModel> MaterialList
        {
            get { return _materialList; }
            set { SetProperty(ref _materialList, value); }
        }
        private List<BrandUIModel> _familyList;
        public List<BrandUIModel> FamilyList
        {
            get { return _familyList; }
            set { SetProperty(ref _familyList, value); }
        }
        private List<BrandUIModel> _brandList;
        public List<BrandUIModel> BrandList
        {
            get { return _brandList; }
            set { SetProperty(ref _brandList, value); }
        }
        private List<BrandUIModel> _groupList;
        public List<BrandUIModel> GroupList
        {
            get { return _groupList; }
            set { SetProperty(ref _groupList, value); }
        }
        private BrandUIModel _selectedCategory;
        public BrandUIModel SelectedCategory
        {
            get { return _selectedCategory; }
            set { SetProperty(ref _selectedCategory, value); }
        }
        private BrandUIModel _selectedMaterial;
        public BrandUIModel SelectedMaterial
        {
            get { return _selectedMaterial; }
            set { SetProperty(ref _selectedMaterial, value); }
        }
        private BrandUIModel _selectedFamily;
        public BrandUIModel SelectedFamily
        {
            get { return _selectedFamily; }
            set { SetProperty(ref _selectedFamily, value); }
        }
        private BrandUIModel _selectedBrand;
        public BrandUIModel SelectedBrand
        {
            get { return _selectedBrand; }
            set { SetProperty(ref _selectedBrand, value); }
        }
        private BrandUIModel _selectedGroup;
        public BrandUIModel SelectedGroup
        {
            get { return _selectedGroup; }
            set { SetProperty(ref _selectedGroup, value); }
        }
    }
}
