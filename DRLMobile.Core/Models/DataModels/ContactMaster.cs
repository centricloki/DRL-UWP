using SQLite;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class ContactMaster : BaseModel
    {
        [PrimaryKey]
        [JsonProperty("contactid")]
        public int? ContactID { get; set; }

        [JsonProperty("devicecontactid")]
        public string DeviceContactID { get; set; }

        [JsonProperty("devicecustomerid")]
        public string DeviceCustomerID { get; set; }
        public int IsExported { get; set; }

        [JsonProperty("createdate")]
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        [JsonProperty("updatedate")]
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        [JsonProperty("contactname")]
        public string ContactName { get; set; }
        
        [JsonProperty("contactrole")]
        public string ContactRole { get; set; }

        [JsonProperty("contactemail")]
        public string ContactEmail { get; set; }

        [JsonProperty("contactphone")]
        public string ContactPhone { get; set; }

        [JsonProperty("contactfax")]
        public string ContactFax { get; set; }

        [JsonProperty("rankid")]
        public int RankID { get; set; }

        [JsonProperty("positionid")]
        public int PositionID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactCellPhone { get; set; }
        public string ContactNote { get; set; }
        public int IsDeleted { get; set; }

        private bool _IsDeletedFromServer;
        [Ignore]
        [JsonProperty("deleted")]
        public bool IsDeletedFromServer
        {
            get { return _IsDeletedFromServer; }
            set
            {
                _IsDeletedFromServer = value;

                IsDeleted = value ? 1 : 0;
            }
        }

        private bool _isexportedFromServer;
        [Ignore]
        [JsonProperty("isexported")]
        public bool IsExportedFromServer
        {
            get { return _isexportedFromServer; }
            set
            {
                _isexportedFromServer = value;

                IsExported = value ? 1 : 0;
            }
        }

        private int _CreatedByFromServer;
        [Ignore]
        [JsonProperty("createdby")]
        public int CreatedByFromServer
        {
            get { return _CreatedByFromServer; }
            set
            {
                _CreatedByFromServer = value;

                CreatedBy = Convert.ToString(value);
            }
        }

        private int _UpdatedByFromServer;
        [Ignore]
        [JsonProperty("updatedby")]
        public int UpdatedByFromServer
        {
            get { return _UpdatedByFromServer; }
            set
            {
                _UpdatedByFromServer = value;

                UpdatedBy = Convert.ToString(value);
            }
        }


        private string _selectedRank;
        [Ignore]
        public string SelectedRank
        {
            get { return _selectedRank; }
            set { SetProperty(ref _selectedRank, value); }
        }


        private string _selectedPosition;
        [Ignore]
        public string SelectedPosition
        {
            get { return _selectedPosition; }
            set { SetProperty(ref _selectedPosition, value); }
        }

        private Dictionary<int, string> _ranks;
        [Ignore]
        public Dictionary<int, string> Ranks
        {
            get { return _ranks; }
            set { SetProperty(ref _ranks, value); }
        }

        private Dictionary<int, string> _positions;
        [Ignore]
        public Dictionary<int, string> Positions
        {
            get { return _positions; }
            set { SetProperty(ref _positions, value); }
        }


        private bool _isInEditMode;
        [Ignore]
        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            set { SetProperty(ref _isInEditMode, value); }
        }


        private string _displayContactName;
        [Ignore]
        public string DisplayContactName
        {
            get { return _displayContactName; }
            set { SetProperty(ref _displayContactName, value); }
        }


        private string _displayContactPhone;
        [Ignore]
        public string DisplayContactPhone
        {
            get { return _displayContactPhone; }
            set { SetProperty(ref _displayContactPhone, value); }
        }


        private string _displayContactEmail;
        [Ignore]
        public string DisplayContactEmail
        {
            get { return _displayContactEmail; }
            set { SetProperty(ref _displayContactEmail, value); }
        }



        private string _displayContactFax;
        [Ignore]
        public string DisplayContactFax
        {
            get { return _displayContactFax; }
            set { SetProperty(ref _displayContactFax, value); }
        }


        #region input validation flags


        private bool _isEmailValid;
        [Ignore]
        public bool IsEmailValid
        {
            get { return _isEmailValid; }
            set { SetProperty(ref _isEmailValid, value); }
        }


        #endregion


        public void PopulateUI()
        {
            DisplayContactName = this.ContactName;
            DisplayContactEmail = ContactEmail;
            DisplayContactPhone = ContactPhone;
            DisplayContactFax = ContactFax;
        }
    }
}