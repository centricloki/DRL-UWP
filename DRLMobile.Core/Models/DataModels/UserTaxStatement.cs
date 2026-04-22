using DRLMobile.Core.Models.UIModels;
using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class UserTaxStatement
    {
        [PrimaryKey]
        public int UserTaxStatementID { get; set; }
        public string DeviceUserTaxStatementID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int IsExported { get; set; }
        public int IsDeleted { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public UserTaxStatementUiModel CopyToUIModel()
        {
            var uiModel = new UserTaxStatementUiModel()
            {
                UserTaxStatementID = this.UserTaxStatementID,
                DeviceUserTaxStatementID = this.DeviceUserTaxStatementID,
                UserID = this.UserID,
                Title = this.Title,
                Description = this.Description,
                IsExported = this.IsExported,
                IsDeleted = this.IsDeleted,
                CreatedDate= this.CreatedDate,
                CreatedBy = this.CreatedBy,
                UpdatedDate = this.UpdatedDate,
                UpdatedBy = this.UpdatedBy
            };
            return uiModel;

        }
    }
}