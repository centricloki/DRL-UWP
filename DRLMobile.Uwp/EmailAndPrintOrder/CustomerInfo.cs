using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLMobile.EmailAndPrintOrder
{
   public class CustomerInfo
    {
        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public string EmailID { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string PurchaseNumber { get; set; }
        public string PONumber { get; set; }
        public string OrderDate { get; set; }
        public string ShipDate { get; set; }
        public string SalesRepresentative { get; set; }
        public string RTAccountRep { get; set; }
        public string CustomerComment { get; set; }
        public string CustomStatement { get; set; }
        public string PermitNumber { get; set; }
        public string SalesRepUsername { get; set; }
        public string SalesRepEmail { get; set; }
        public string SalesRepPhone { get; set; }
        public string SalesRepManager { get; set; }
        public string RTSalesRepManagerEmail { get; set; }
        public string RTSalesRepEmail { get; set; }
        public string PrintName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string RegionID { get; set; }
        public string UserName { get; set; }
        public string AccountRepEmail { get; set; }
        //Added by Senthil Ramadoss on 11/06/2020
        public string AuthCode { get; set; }
        public string ReceiptNo { get; set; }
        public string CreatedAt { get; set; }
        public string CCBrand { get; set; }
        public string CCLFDigit { get; set; }
    }
}
