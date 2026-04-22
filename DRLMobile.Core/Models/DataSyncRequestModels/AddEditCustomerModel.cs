namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class AddEditCustomerModel
    {
        public string TaxStatement { get; set; }
        public int accountclassification { get; set; }
        public string accountresponsibility { get; set; }
        public int accounttype { get; set; }
        public string broker { get; set; }
        public string buyer { get; set; }
        public string contactemail { get; set; }
        public string contactname { get; set; }
        public string contactphone { get; set; }
        public string contactrole { get; set; }
        public string createdate { get; set; }
        public string customername { get; set; }
        public string devicecustomerid { get; set; }
        public string devicedistributorcustomerid { get; set; }
        public string emailid { get; set; }
        public string fax { get; set; }
        public string generalcomments { get; set; }
        public int iscreatepermanent { get; set; }
        public string mailingaddress { get; set; }
        public string mailingaddresscityid { get; set; }
        public int mailingaddressstateid { get; set; }
        public string mailingaddresszipcode { get; set; }
        public string managername { get; set; }
        public string phone { get; set; }
        public string physicaladdress { get; set; }
        public string physicaladdresscityid { get; set; }
        public int physicaladdressstateid { get; set; }
        public string physicaladdresszipcode { get; set; }
        public string ranking { get; set; }
        public string retailerlicense { get; set; }
        public string retailersalestaxcertificate { get; set; }
        public string shippingaddress { get; set; }
        public string shippingaddresscityid { get; set; }
        public int shippingaddressstateid { get; set; }
        public string shippingaddresszipcode { get; set; }
        public string statetobaccolicense { get; set; }
        public int storecount { get; set; }
        public int territoryid { get; set; }
        
        public int isexported { get; set; }
        public int isdeleted { get; set; }
        public string OrderDeliveryWeekDays { get; set; }
    }
}
