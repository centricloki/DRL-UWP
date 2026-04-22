using System.Collections.Generic;
using System;
using DevExpress.Data;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DRLMobile.Core.Models.UIModels;

namespace DRLMobile.Helpers.CustomerPageGridHelper
{
    public static class CustomerPageDataSourceHelper
    {
        public static CustomerFilter MakeCustomerFilter(CriteriaOperator filter)
        {
            return filter.Match(
                binary: (propertyName, value, type) =>
                {
                    if (propertyName == "CustomerName" && type == BinaryOperatorType.Equal)
                    {
                        return new CustomerFilter(customerName: (string)value);
                    }

                    if (propertyName == "CustomerNumber" && type == BinaryOperatorType.Equal)
                    {
                        return new CustomerFilter(customerNumber: (string)value);
                    }

                    if (propertyName == "StoreType" && type == BinaryOperatorType.Equal)
                    {
                        return new CustomerFilter(storeType: (string)value);
                    }

                    if (propertyName == "Rank" && type == BinaryOperatorType.Equal)
                    {
                        return new CustomerFilter(rank: (string)value);
                    }

                    if (propertyName == "Address" && type == BinaryOperatorType.Equal)
                    {
                        return new CustomerFilter(address: (string)value);
                    }

                    if (propertyName == "City" && type == BinaryOperatorType.Equal)
                    {
                        return new CustomerFilter(city: (string)value);
                    }

                    if (propertyName == "State" && type == BinaryOperatorType.Equal)
                    {
                        return new CustomerFilter(state: (string)value);
                    }

                    if (propertyName == "LastCallDate" && type == BinaryOperatorType.Equal)
                    {
                        return new CustomerFilter(lastCallDate: (string)value);
                    }
                    throw new InvalidOperationException();
                },
                and: filters =>
                {
                    var result = new CustomerFilter
                    (
                        customerName: filters.Select(x => x.CustomerName).SingleOrDefault(x => x != null),
                        customerNumber: filters.Select(x => x.CustomerNumber).SingleOrDefault(x => x != null),
                        storeType: filters.Select(x => x.StoreType).SingleOrDefault(x => x != null),
                        rank: filters.Select(x => x.Rank).SingleOrDefault(x => x != null),
                        address: filters.Select(x => x.Address).SingleOrDefault(x => x != null),
                        city: filters.Select(x => x.City).SingleOrDefault(x => x != null),
                        state: filters.Select(x => x.State).SingleOrDefault(x => x != null),
                        lastCallDate: filters.Select(x => x.LastCallDate).SingleOrDefault(x => x != null)
                        );
                    return result;
                }
                , @null: default(CustomerFilter)
            );
        }

        public static CustomerSortOrder GetCustomerSortOrder(FetchRowsEventArgsBase e)
        {
            if (e.SortOrder.Length > 0)
            {
                var sort = e.SortOrder.Single();
                if (sort.PropertyName == "CustomerName")
                {
                    return sort.Direction == ListSortDirection.Ascending
                        ? CustomerSortOrder.CustomerNameAscending
                        : CustomerSortOrder.CustomerNameDescending;
                }
                if (sort.PropertyName == "CustomerNumber")
                {
                    return sort.Direction == ListSortDirection.Ascending
                        ? CustomerSortOrder.CustomerNumberAscending
                        : CustomerSortOrder.CustomerNumberDescending;
                }
                if (sort.PropertyName == "StoreType")
                {
                    return sort.Direction == ListSortDirection.Ascending
                        ? CustomerSortOrder.StoreTypeAscending
                        : CustomerSortOrder.StoreTypeDescending;
                }
                if (sort.PropertyName == "Rank")
                {
                    return sort.Direction == ListSortDirection.Ascending
                        ? CustomerSortOrder.RankAscending
                        : CustomerSortOrder.RankDescending;
                }
                if (sort.PropertyName == "Address")
                {
                    return sort.Direction == ListSortDirection.Ascending
                        ? CustomerSortOrder.AddressAscending
                        : CustomerSortOrder.AddressDescending;
                }
                if (sort.PropertyName == "City")
                {
                    return sort.Direction == ListSortDirection.Ascending
                        ? CustomerSortOrder.CityAscending
                        : CustomerSortOrder.CityDescending;
                }
                if (sort.PropertyName == "State")
                {
                    return sort.Direction == ListSortDirection.Ascending
                        ? CustomerSortOrder.StateAscending
                        : CustomerSortOrder.StateDescending;
                }
                if (sort.PropertyName == "LastCallDate")
                {
                    return sort.Direction == ListSortDirection.Ascending
                        ? CustomerSortOrder.LastCallDateAscending
                        : CustomerSortOrder.LastCallDateDescending;
                }
            }
            return CustomerSortOrder.Default;
        }

        public static IEnumerable<CustomerPageUIModel> FilterCustomer(CustomerFilter filter, IEnumerable<CustomerPageUIModel> customers)
        {
            if (filter == null)
                return customers;

            var tempList = new List<CustomerPageUIModel>();
            var filterCustomerName = filter?.CustomerName == null ? string.Empty : filter?.CustomerName?.ToLower();
            var filterCustomerNumber = filter?.CustomerNumber == null ? string.Empty : filter?.CustomerNumber?.ToLower();
            var filterStoreType = filter?.StoreType == null ? string.Empty : filter?.StoreType?.ToLower();
            var filterRank = filter?.Rank == null ? string.Empty : filter?.Rank?.ToLower();
            var filterAddress = filter?.Address == null ? string.Empty : filter?.Address?.ToLower();
            var filterCity = filter?.City == null ? string.Empty : filter?.City?.ToLower();
            var filterState = filter?.State == null ? string.Empty : filter?.State?.ToLower();
            var filterLastCallDate = filter?.LastCallDate == null ? string.Empty : filter?.LastCallDate?.ToLower();

            foreach (var item in customers)
            {
                var boolRes = !string.IsNullOrEmpty(filterCustomerName) && !string.IsNullOrEmpty(item.CustomerName) && item.CustomerName.ToLower().Equals(filterCustomerName) ||
                !string.IsNullOrWhiteSpace(filterCustomerNumber) && !string.IsNullOrWhiteSpace(item.CustomerNumber) && item.CustomerNumber.ToLower().Equals(filterCustomerNumber) ||
                !string.IsNullOrEmpty(filterStoreType) && !string.IsNullOrEmpty(item.StoreType) && item.StoreType.ToLower().Equals(filterStoreType) ||
                !string.IsNullOrEmpty(filterRank) && !string.IsNullOrEmpty(item.Rank) && item.Rank.ToLower().Equals(filterRank) ||
                !string.IsNullOrEmpty(filterAddress) && !string.IsNullOrEmpty(item.Address) && item.Address.ToLower().Equals(filterAddress) ||
                !string.IsNullOrEmpty(filterCity) && !string.IsNullOrEmpty(item.City) && item.City.ToLower().Equals(filterCity) ||
                !string.IsNullOrEmpty(filterState) && !string.IsNullOrEmpty(item.State) && item.State.ToLower().Equals(filterState) ||
                !string.IsNullOrEmpty(filterLastCallDate) && !string.IsNullOrEmpty(item.LastCallDate) && item.LastCallDate.ToLower().Equals(filterLastCallDate);
                if (boolRes)
                    tempList.Add(item);
            }
            return tempList;
        }


        public static IEnumerable<CustomerPageUIModel> SortCustomer(CustomerSortOrder sortOrder, IEnumerable<CustomerPageUIModel> customers)
        {
            switch (sortOrder)
            {
                case CustomerSortOrder.Default:
                    return customers;
                case CustomerSortOrder.CustomerNameDescending:
                    return customers.OrderByDescending(x => x.CustomerName);
                case CustomerSortOrder.CustomerNameAscending:
                    return customers.OrderBy(x => x.CustomerName);

                case CustomerSortOrder.CustomerNumberDescending:
                    return customers.OrderByDescending(x => x.CustomerNumber);
                case CustomerSortOrder.CustomerNumberAscending:
                    return customers.OrderBy(x => x.CustomerNumber);

                case CustomerSortOrder.StoreTypeDescending:
                    return customers.OrderByDescending(x => x.StoreType);
                case CustomerSortOrder.StoreTypeAscending:
                    return customers.OrderBy(x => x.StoreType);

                case CustomerSortOrder.RankDescending:
                    return customers.OrderByDescending(x => x.Rank);
                case CustomerSortOrder.RankAscending:
                    return customers.OrderBy(x => x.Rank);

                case CustomerSortOrder.AddressDescending:
                    return customers.OrderByDescending(x => x.Address);
                case CustomerSortOrder.AddressAscending:
                    return customers.OrderBy(x => x.Address);

                case CustomerSortOrder.CityDescending:
                    return customers.OrderByDescending(x => x.City);
                case CustomerSortOrder.CityAscending:
                    return customers.OrderBy(x => x.City);

                case CustomerSortOrder.StateDescending:
                    return customers.OrderByDescending(x => x.State);
                case CustomerSortOrder.StateAscending:
                    return customers.OrderBy(x => x.State);

                case CustomerSortOrder.LastCallDateDescending:
                    return customers.OrderByDescending(x => x.LastCallDate);
                case CustomerSortOrder.LastCallDateAscending:
                    return customers.OrderBy(x => x.LastCallDate);

                default:
                    return customers;
            }
        }

    }

    class DefaultSortComparer : IComparer<CustomerPageUIModel>
    {
        int IComparer<CustomerPageUIModel>.Compare(CustomerPageUIModel x, CustomerPageUIModel y)
        {
            if (x.CustomerName != y.CustomerName)
                return Comparer<string>.Default.Compare(x.CustomerName, y.CustomerName);
            return Comparer<string>.Default.Compare(x.CustomerNumber, y.CustomerNumber);
        }
    }
}
