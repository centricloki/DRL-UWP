namespace DRLMobile.Core.Models.DataModels
{
    public class ProductRoleLink
    {
        public int ProductRoleLinkID { get; set; }
        public int CatBrandProductID { get; set; }
        public string Type { get; set; }
        public int RoleID { get; set; }
        public int Status { get; set; }
        public string UpdateDate { get; set; }

    }
}