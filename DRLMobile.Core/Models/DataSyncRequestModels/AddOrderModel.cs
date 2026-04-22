using System.Collections.Generic;

namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class AddOrderModel
    {
        public int IsVoided { get; set; }
        public string createdate { get; set; }
        public string customercomment { get; set; }
        public string customername { get; set; }
        public int customerparentid { get; set; }
        public string customershippingcityid { get; set; }
        public int customershippingstateid { get; set; }
        public string customershippingzipcode { get; set; }
        public string customstatement { get; set; }
        public string devicecustomerid { get; set; }
        public string deviceorderid { get; set; }
        public string emailrecipients { get; set; }
        public string grandtotal { get; set; }
        public string invoicenumber { get; set; }
        public int isoderconfirmed { get; set; }
        public int isorderemailsent { get; set; }
        public string orderaddress { get; set; }
        public string ordercityid { get; set; }
        public string orderdate { get; set; }
        public List<AddOrderDetails> orderdetails { get; set; }
        public string ordernumber { get; set; }
        public int orderstateid { get; set; }
        public string orderzipcode { get; set; }
        public string prebookshipdate { get; set; }
        public string printname { get; set; }
        public string purchaseordernumber { get; set; }
        public string republicsalesrepository { get; set; }
        public string retailerlicense { get; set; }
        public string retailersalestaxcertificate { get; set; }
        public int salestype { get; set; }
        public string sellername { get; set; }
        public string sellerreptobaccopermitno { get; set; }
        public string statetobaccolicense { get; set; }
        public int isexported { get; set; }
        public string retaildistributornumber { get; set; }
    }

    public class AddOrderDetails
    {
        public string creditrequest { get; set; }
        public decimal price { get; set; }
        public int productid { get; set; }
        public int quantity { get; set; }
        public string units { get; set; }
    }
}
