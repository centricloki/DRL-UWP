using DRLMobile.Core.Models.UIModels;

using Newtonsoft.Json;

using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class MasterTableType
    {
        [PrimaryKey]
        public byte Id { get; set; }
        public string Name { get; set; }
        public byte DisplayOrder { get; set; }
        public byte IsActive { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }
    public class UserActivityType : MasterTableType
    {
        public MasterTableTypeUIModel CopyToUIModel()
        {
            return new MasterTableTypeUIModel()
            {
                Id = this.Id,
                Name = this.Name
            };
        }


    }
    public class CustomerActivityType : MasterTableType
    {
        public MasterTableTypeUIModel CopyToUIModel()
        {
            return new MasterTableTypeUIModel()
            {
                Id = this.Id,
                Name = this.Name
            };
        }


    }
    public class CustomerDocumentType : MasterTableType
    {
        public MasterTableTypeUIModel CopyToUIModel()
        {
            return new MasterTableTypeUIModel()
            {
                Id = this.Id,
                Name = this.Name
            };
        }


    }
}