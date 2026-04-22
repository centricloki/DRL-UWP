namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class AddFavoriteModel
    {
        public int userid { get; set; }
        public int productid { get; set; }
        public bool isdeleted { get; set; }
    }
}