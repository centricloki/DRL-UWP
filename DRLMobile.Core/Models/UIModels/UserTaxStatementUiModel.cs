using DRLMobile.Core.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class UserTaxStatementUiModel : BaseModel
    {
        private UserTaxStatement _userTaxStatementMasterData;
        public UserTaxStatement UserTaxStatementMasterData
        {
            get { return _userTaxStatementMasterData; }
            set { SetProperty(ref _userTaxStatementMasterData, value); }
        }

        public int UserTaxStatementID { get; set; }

        private string _deviceUserTaxStatementID;
        public string DeviceUserTaxStatementID
        {
            get { return _deviceUserTaxStatementID; }
            set { SetProperty(ref _deviceUserTaxStatementID, value); }
        }
        
        public int UserID { get; set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        
        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        public int IsExported { get; set; }
        public int IsDeleted { get; set; }
        private string _createdDate;
        public string CreatedDate
        {
            get { return _createdDate; }
            set { SetProperty(ref _createdDate, value); }
        }
        private string _createdBy;
        public string CreatedBy
        {
            get { return _createdBy; }
            set { SetProperty(ref _createdBy, value); }
        }
        private string _updatedDate;
        public string UpdatedDate
        {
            get { return _updatedDate; }
            set { SetProperty(ref _updatedDate, value); }
        }
        private string _updatedBy;
        public string UpdatedBy
        {
            get { return _updatedBy; }
            set { SetProperty(ref _updatedBy, value); }
        }
        public void UserTaxStatementUiToDataModel()
        {
            UserTaxStatementMasterData = new UserTaxStatement();
            UserTaxStatementMasterData.UserTaxStatementID = UserTaxStatementID;
            UserTaxStatementMasterData.Title = Title;
            UserTaxStatementMasterData.Description = Description;
            UserTaxStatementMasterData.DeviceUserTaxStatementID = DeviceUserTaxStatementID;
            UserTaxStatementMasterData.UserID = UserID;
            UserTaxStatementMasterData.IsExported = IsExported;
            UserTaxStatementMasterData.CreatedDate = CreatedDate;
            UserTaxStatementMasterData.CreatedBy = CreatedBy;
            UserTaxStatementMasterData.UpdatedDate = UpdatedDate;
            UserTaxStatementMasterData.UpdatedBy = UpdatedBy;
            UserTaxStatementMasterData.IsDeleted = IsDeleted;
        }
    }
}
