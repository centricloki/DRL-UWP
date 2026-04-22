using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLMobile.OrderEmailAndPrint
{
    public static class Helper
    {
        public  enum  SalesType : int
        { 
            Cash_Sales  = 1,
            Prebook =  2,
            Bill_Through =  3,
            Suggested_Order =    4,
            Rack_POS =  5,
            Distributor_Order  = 6,
            Distributor_Invoice = 7,
            Credit_Request =  8,
            Cash_Sales_Initiative = 9,
            POP = 10,
            Sample_Order = 11,
            Chain_Distribution = 12,
            Credit_Card_Sales =  13
                     
                    


        }
    }
}
