namespace DRLMobile.Core.Models.UIModels
{
    public class LoggedInUserDetailsUIModel : BaseModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public int Pin { get; set; }
        public int RoleId { get; set; }
        public int RegionId { get; set; }
        public int ZoneId { get; set; }
        public string EmailId { get; set; }
        public string TerritoryId { get; set; }
        public int BDId { get; set; }
        public int AVPId { get; set; }
    }
}
