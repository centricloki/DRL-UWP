using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLMobile.Helpers.CustomerPageGridHelper
{
    public static class CustomerFetchService
    {
        public static Lazy<List<CustomerPageUIModel>> CustomerListMain { get; set; }


        public async static Task<CustomerPageUIModel[]> GetCustomerAsync(int page, int pageSize, CustomerSortOrder sortOrder,
            CustomerFilter filter)
        {

            return await Task.Run(() =>
            {
                try
                {

                    var sortedCustomer = CustomerPageDataSourceHelper.SortCustomer(sortOrder, CustomerListMain?.Value);
                    if (filter != null)
                        sortedCustomer = CustomerPageDataSourceHelper.FilterCustomer(filter, sortedCustomer);
                    sortedCustomer = sortedCustomer.Skip(page * pageSize).Take(pageSize);
                    return sortedCustomer.ToArray();
                }
                catch (Exception ex)
                {
                    ErrorHandler.LogException(nameof(CustomerFetchService), nameof(GetCustomerAsync), ex.Message);

                    return null;

                }
            });
        }
    }
}
