using DRLMobile.Core.Interface;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace DRLMobile.Uwp.Helpers
{
    public class MapsStaticDataSourceHelper : IMapsStaticDataSourceHelper
    {
        public static List<Classification> ClassificationsList { get; set; } = new List<Classification>();

        public ObservableCollection<PlotByTypeFilterUIModel> GetPlotByTypeFiltersDataSource()
        {
            ObservableCollection<PlotByTypeFilterUIModel> _plotByFilter = new ObservableCollection<PlotByTypeFilterUIModel>();
            _plotByFilter.Add(new PlotByTypeFilterUIModel() { Title = "Trade Type", IsSelected = true, Tag = Core.Enums.MapFilter.TradeType });
            _plotByFilter.Add(new PlotByTypeFilterUIModel() { Title = "Account Rank", IsSelected = false, Tag = Core.Enums.MapFilter.Rank });
            _plotByFilter.Add(new PlotByTypeFilterUIModel() { Title = "Call Date", IsSelected = false, Tag = Core.Enums.MapFilter.CallDate });
            _plotByFilter.Add(new PlotByTypeFilterUIModel() { Title = "Cash Sales", IsSelected = false, Tag = Core.Enums.MapFilter.CashSales });
            _plotByFilter.Add(new PlotByTypeFilterUIModel() { Title = "Item No", IsSelected = false, Tag = Core.Enums.MapFilter.Item });
            return _plotByFilter;
        }

        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForCallDate()
        {
            ObservableCollection<MapsLegendFilterUIModel> _callDateFilter = new ObservableCollection<MapsLegendFilterUIModel>();
            _callDateFilter.Add(new MapsLegendFilterUIModel() { Title = "Less than 1 month", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Green), Tag = 1, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Green.png" });
            _callDateFilter.Add(new MapsLegendFilterUIModel() { Title = "1-3 months", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Yellow), Tag = 2, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Florecent.png" });
            _callDateFilter.Add(new MapsLegendFilterUIModel() { Title = "3-6 months", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Orange), Tag = 3, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Yellow.png" });
            _callDateFilter.Add(new MapsLegendFilterUIModel() { Title = "6 months – 1 year", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Red), Tag = 4, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Red.png" });
            _callDateFilter.Add(new MapsLegendFilterUIModel() { Title = "Over 1 year", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Black), Tag = 5, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Black.png" });
            return _callDateFilter;
        }

        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForCashSales()
        {
            ObservableCollection<MapsLegendFilterUIModel> _cashSalesFilter = new ObservableCollection<MapsLegendFilterUIModel>();
            _cashSalesFilter.Add(new MapsLegendFilterUIModel() { Title = "$0.01 - $100.00", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Orange), Tag = 1, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Yellow.png" });
            _cashSalesFilter.Add(new MapsLegendFilterUIModel() { Title = "$100.01 - $500.00", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Yellow), Tag = 2, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Florecent.png" });
            _cashSalesFilter.Add(new MapsLegendFilterUIModel() { Title = ">$500.01", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Green), Tag = 3, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Green.png" });
            _cashSalesFilter.Add(new MapsLegendFilterUIModel() { Title = "$0.00(No Sales Activity)", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Purple), Tag = 4, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Voilet.png" });
            return _cashSalesFilter;
        }

        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForItemNo()
        {
            ObservableCollection<MapsLegendFilterUIModel> _itemNoFilter = new ObservableCollection<MapsLegendFilterUIModel>();
            _itemNoFilter.Add(new MapsLegendFilterUIModel() { Title = "Sold", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Green), Tag = 1, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Green.png" });
            _itemNoFilter.Add(new MapsLegendFilterUIModel() { Title = "Not Sold", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Orange), Tag = 2, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Yellow.png" });
            return _itemNoFilter;
        }

        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForRank()
        {
            ObservableCollection<MapsLegendFilterUIModel> _rankTypeFilter = new ObservableCollection<MapsLegendFilterUIModel>();

            _rankTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Rank A", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Green), Rank = "A", MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Green.png" });
            _rankTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Rank B", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Blue), Rank = "B", MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Blue.png" });
            _rankTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Rank C", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Brown), Rank = "C", MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Brown.png" });
            _rankTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Other", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Red), Rank = "", MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Red.png" });
            return _rankTypeFilter;
        }

        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForTradeType()
        {
            ObservableCollection<MapsLegendFilterUIModel> _tradeTypeFilter = new ObservableCollection<MapsLegendFilterUIModel>();

            List<int> customerTypeDirect = new List<int>();

            if (ClassificationsList != null && ClassificationsList.Count > 0)
            {
                customerTypeDirect = ClassificationsList.Where(x => x.CustomerType == 1).Select(a => a.AccountClassificationId).ToList();

                customerTypeDirect.Add(20);
            }
            else
            {
                customerTypeDirect = new List<int> { 1, 2, 8, 20 };
            }
        
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "MSAi  List A", IsSelected = true, BackgroundColor = (SolidColorBrush)Application.Current.Resources["R255_G126_B121"], AccountClassificationIds = new List<int>() { 47 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-1.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "DM Location", IsSelected = true, BackgroundColor = new SolidColorBrush(Microsoft.Toolkit.Uwp.Helpers.ColorHelper.ToColor("#aaff00")), AccountClassificationIds = new List<int>() { 46 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-LimeGreen.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Wholesale", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Green), AccountClassificationIds = customerTypeDirect, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Green.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "C-Store Chain HQ", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Orange), AccountClassificationIds = new List<int>() { 22 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Yellow.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "C-Store Chain Location", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Purple), AccountClassificationIds = new List<int>() { 23 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Voilet.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "C-Store Independent", IsSelected = true, BackgroundColor = (SolidColorBrush)Application.Current.Resources["R204_G102_B0"], AccountClassificationIds = new List<int>() { 24 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-LightBrown.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Tobacco Outlet – Chain HQ", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Blue), AccountClassificationIds = new List<int>() { 25 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Blue.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Tobacco Outlet – Chain Location", IsSelected = true, BackgroundColor = (SolidColorBrush)Application.Current.Resources["R255_G64_B255"], AccountClassificationIds = new List<int>() { 26 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-5.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Tobacco Outlet - Independent", IsSelected = true, BackgroundColor = (SolidColorBrush)Application.Current.Resources["R146_G22_B37"], AccountClassificationIds = new List<int>() { 27 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-6.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Smoke Shop", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Red), AccountClassificationIds = new List<int>() { 28 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Red.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Dispensary Store", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Yellow), AccountClassificationIds = new List<int>() { 29 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Florecent.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "S-D-M Chain HQ", IsSelected = true, BackgroundColor = (SolidColorBrush)Application.Current.Resources["R0_G253_B255"], AccountClassificationIds = new List<int>() { 30 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Cyan.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "S-D-M Chain Location", IsSelected = true, BackgroundColor = (SolidColorBrush)Application.Current.Resources["R255_G126_B121"], AccountClassificationIds = new List<int>() { 31 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-1.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "S-D-M – Independent", IsSelected = true, BackgroundColor = (SolidColorBrush)Application.Current.Resources["R255_G102_B178"], AccountClassificationIds = new List<int>() { 32 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Pink.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Liquor Store – Chain HQ", IsSelected = true, BackgroundColor = (SolidColorBrush)Application.Current.Resources["R146_G146_B146"], AccountClassificationIds = new List<int>() { 33 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Gray.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Liquor Store – Chain Location", IsSelected = true, BackgroundColor = (SolidColorBrush)Application.Current.Resources["R130_G125_B21"], AccountClassificationIds = new List<int>() { 34 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-2.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Liquor Store – Independent", IsSelected = true, BackgroundColor = (SolidColorBrush)Application.Current.Resources["R122_G129_B255"], AccountClassificationIds = new List<int>() { 35 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-4.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Sub Jobber Wholesale", IsSelected = true, BackgroundColor = (SolidColorBrush)Application.Current.Resources["R6_G177_B177"], AccountClassificationIds = new List<int>() { 36 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-3.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Tribal Accounts ", IsSelected = true, BackgroundColor = (SolidColorBrush)Application.Current.Resources["R255_G212_B121"], AccountClassificationIds = new List<int>() { 37 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-LightYellow.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Out of business ", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Black), AccountClassificationIds = new List<int>() { 38 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Black.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Smoke Shop - Chain HQ ", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Brown), AccountClassificationIds = new List<int>() { 44 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Brown.png" });
            _tradeTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Smoke Shop - Chain Location ", IsSelected = true, BackgroundColor = (SolidColorBrush)Application.Current.Resources["R255_G212_B121"], AccountClassificationIds = new List<int>() { 45 }, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-LightYellow.png" });

            return _tradeTypeFilter;
        }
    }
}
