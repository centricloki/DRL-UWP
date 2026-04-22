using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLMobile.EmailAndPrintOrder
{
   public class OrderDetails
    {
        public int ItemNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Units { get; set; }
        public decimal NetPrice { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public decimal GrandTotal { get; set; }
        public string IsTobaccoProduct { get; set; }
        public string IsPromotional { get; set; }
        public string DistributorName { get; set; }
        public string DistributorState { get; set; }
        public string DistributorCity { get; set; }
        public string DistributorZip { get; set; }
        public string DistributorID { get; set; }
        public string TobaccoLicense { get; set; }
        public string RetailerLicense { get; set; }
        public string SalesTaxCertificate { get; set; }
        public string ReturnReason { get; set; }
        public string RegionID { get; set; }
    }
}
