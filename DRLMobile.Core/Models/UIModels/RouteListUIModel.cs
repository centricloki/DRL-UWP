using DRLMobile.ExceptionHandler;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace DRLMobile.Core.Models.UIModels
{
    public class RouteListUIModel : BaseModel
    {
        private readonly object thisLock = new object();

        public string TerritoryName { get; set; }

        public string CreatorName { get; set; }

        public string UserName { get; set; }

        public int RouteId { get; set; }

        public string RouteName { get; set; }

        public string StreetName { get; set; }

        private string _startDate;
        public string StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); PopulateStartDate(); }
        }

        private string _endDate;
        public string EndDate 
        {
            get { return _endDate; }
            set { SetProperty(ref _endDate, value); PopulateEndDate(); } 
        }

        public string UpdatedDate { get; set; }

        public int UserId { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public string Zipcode { get; set; }

        public string AddressName { get; set; }

        public string RouteType { get; set; }

        public int CityId { get; set; }

        public string RouteBrief { get; set; }

        public string HouseNo { get; set; }

        public string City { get; set; }
        
        public string StateProvinceRegion { get; set; }
        
        public string Country { get; set; }
        
        public int IsExported { get; set; }

        public string CreatedDate { get; set; }
        
        public string CreatedBy { get; set; }
        
        public string ImportedFrom { get; set; }
        
        public string UpdatedBy { get; set; }
        
        public int CustomerId { get; set; }

        public string DeviceRouteId { get; set; }

        public int IsDeleted { get; set; }

        public int idAssignToTSM { get; set; }

        public string SearchDisplayPath
        {
            get { return RouteName + " " + RouteBrief; }
        }

        private Visibility _editIconVisiblity;
        public Visibility EditIconVisibility
        {
            get { return _editIconVisiblity; }
            set { SetProperty(ref _editIconVisiblity, value); }
        }

        public DateTime RouteStartDate { get; set; }

        public DateTime RouteEndDate { get; set; }

        private void PopulateStartDate()
        {
            try
            {
                Parallel.Invoke(() =>
                {
                    lock (thisLock)
                    {
                        try
                        {
                            DateTime.TryParse(StartDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date);
                            RouteStartDate = date;
                        }
                        catch (Exception)
                        {
                            DateTime.TryParse(StartDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date);
                            RouteStartDate = date;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(RouteListUIModel), nameof(PopulateStartDate), ex.StackTrace);
            }
        }

        private void PopulateEndDate()
        {
            try
            {
                Parallel.Invoke(() =>
                {
                    lock (thisLock)
                    {
                        try
                        {
                            DateTime.TryParse(EndDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date);
                            RouteEndDate = date;
                        }
                        catch (Exception)
                        {
                            DateTime.TryParse(EndDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date);
                            RouteEndDate = date;
                        }
                    }
                }
                   );
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(RouteListUIModel), nameof(PopulateStartDate), ex.StackTrace);
            }
        }

    }
}