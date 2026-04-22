using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLMobile.Uwp.Helpers.CustomerPageGridHelper
{
    public enum CustomerSortOrder
    {
        Default,

        CustomerNameDescending,
        CustomerNameAscending,

        CustomerNumberDescending,
        CustomerNumberAscending,

        StoreTypeDescending,
        StoreTypeAscending,

        RankDescending,
        RankAscending,

        AddressDescending,
        AddressAscending,

        CityDescending,
        CityAscending,

        StateDescending,
        StateAscending,

        LastCallDateDescending,
        LastCallDateAscending,

        CallDateDecending,
        CallDateAscending,
    }
}
