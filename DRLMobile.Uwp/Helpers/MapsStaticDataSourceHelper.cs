using DRLMobile.Core.Interface;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Core.Services;

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
        /// <summary>
        /// Gets or sets the list of active classifications loaded dynamically from the SQLite 
        /// <c>MapClassification</c> table. This datasource drives all trade type legend items, 
        /// colors, display orders, and pin configurations.
        /// </summary>
        public static List<MapClassificationViewModel> MapClassificationList { get; set; }


        // ──────────────────────────────────────────────────────────────────────────
        // Plot-by filter source (static — not classification-driven)
        // ──────────────────────────────────────────────────────────────────────────

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

        // ──────────────────────────────────────────────────────────────────────────
        // Call-date legends  (fixed: Green / Yellow-ish / Orange / Red / Black)
        // These represent time buckets, not classification types — unchanged.
        // ──────────────────────────────────────────────────────────────────────────

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

        // ──────────────────────────────────────────────────────────────────────────
        // Cash-sales legends (fixed: 4 amount tiers — unchanged)
        // ──────────────────────────────────────────────────────────────────────────

        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForCashSales()
        {
            ObservableCollection<MapsLegendFilterUIModel> _cashSalesFilter = new ObservableCollection<MapsLegendFilterUIModel>();
            _cashSalesFilter.Add(new MapsLegendFilterUIModel() { Title = "$0.01 - $100.00", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Orange), Tag = 1, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Yellow.png" });
            _cashSalesFilter.Add(new MapsLegendFilterUIModel() { Title = "$100.01 - $500.00", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Yellow), Tag = 2, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Florecent.png" });
            _cashSalesFilter.Add(new MapsLegendFilterUIModel() { Title = ">$500.01", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Green), Tag = 3, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Green.png" });
            _cashSalesFilter.Add(new MapsLegendFilterUIModel() { Title = "$0.00(No Sales Activity)", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Purple), Tag = 4, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Voilet.png" });
            return _cashSalesFilter;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Item-no legends (fixed: Sold / Not Sold — unchanged)
        // ──────────────────────────────────────────────────────────────────────────

        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForItemNo()
        {
            ObservableCollection<MapsLegendFilterUIModel> _itemNoFilter = new ObservableCollection<MapsLegendFilterUIModel>();
            _itemNoFilter.Add(new MapsLegendFilterUIModel() { Title = "Sold", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Green), Tag = 1, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Green.png" });
            _itemNoFilter.Add(new MapsLegendFilterUIModel() { Title = "Not Sold", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Orange), Tag = 2, MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Yellow.png" });
            return _itemNoFilter;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Rank legends (fixed: A / B / C / Other — unchanged)
        // ──────────────────────────────────────────────────────────────────────────

        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForRank()
        {
            ObservableCollection<MapsLegendFilterUIModel> _rankTypeFilter = new ObservableCollection<MapsLegendFilterUIModel>();
            _rankTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Rank A", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Green), Rank = "A", MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Green.png" });
            _rankTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Rank B", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Blue), Rank = "B", MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Blue.png" });
            _rankTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Rank C", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Brown), Rank = "C", MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Brown.png" });
            _rankTypeFilter.Add(new MapsLegendFilterUIModel() { Title = "Other", IsSelected = true, BackgroundColor = new SolidColorBrush(Colors.Red), Rank = "", MapIconImagePath = "ms-appx:///Assets/Maps/MapPin-Red.png" });
            return _rankTypeFilter;
        }

        /// <summary>
        /// Gets the Trade Type map legend datasource.
        /// 
        /// This is fully database-driven: it reads from <see cref="MapClassificationList"/> 
        /// (populated from the SQLite table). If the list is not yet loaded, it returns 
        /// an empty collection until the database query completes.
        /// </summary>
        /// <returns>A collection of legend items formatted for the map UI.</returns>
        public ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForTradeType()
        {
            if (MapClassificationList == null || MapClassificationList.Count == 0)
            {
                return new ObservableCollection<MapsLegendFilterUIModel>();
            }

            return BuildDbDrivenTradeTypeFilter();
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Private: DB-driven legend builder (Map view only).
        // Uses MapClassificationList populated from the MapClassification SQLite table.
        // Order  → DisplayOrder (ascending; −1 is excluded at the service layer).
        // Colour → ClassificationColorService.GetBrush (which resolves HexColorCode
        //          from the database, falling back to dynamic gradient if null).
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the map legend UI models dynamically from the database configuration.
        /// Classifications with CustomerType == 1 are grouped into a single "Wholesale" legend item.
        /// </summary>
        private ObservableCollection<MapsLegendFilterUIModel> BuildDbDrivenTradeTypeFilter()
        {
            var result = new ObservableCollection<MapsLegendFilterUIModel>();

            // Wholesale group: all CustomerType == 1 entries are merged into one legend row.
            List<int> wholesaleIds = null;
            var wholesaleItems = MapClassificationList
                .Where(x => x.CustomerType == 1)
                .ToList();
            int wholesaleDisplayOrder = -1;
            if (wholesaleItems != null && wholesaleItems.Count > 0)
            {
                wholesaleDisplayOrder = wholesaleItems.First().DisplayOrder;
                wholesaleIds = wholesaleItems
                   .Select(x => x.AccountClassificationId)
                   .ToList();
                if (wholesaleIds != null && wholesaleIds.Count > 0)
                {
                    foreach (var item in MapClassificationList.Where(x => x.CustomerType != 1 &&
                    x.DisplayOrder == wholesaleDisplayOrder))
                    {
                        wholesaleIds.Add(item.AccountClassificationId);
                    }
                }
            }

            bool wholesaleAdded = false;

            // Iterate in display order; Wholesale is inserted at the position of the first
            // Wholesale member encountered in the sorted list.
            foreach (var item in MapClassificationList)
            {
                if (item.CustomerType == 1)
                {
                    if (!wholesaleAdded)
                    {
                        // Represent the entire Wholesale group with a single entry.
                        int repId = item.AccountClassificationId;
                        result.Add(new MapsLegendFilterUIModel
                        {
                            Title = "Wholesale",
                            IsSelected = true,
                            BackgroundColor = ClassificationColorService.GetBrush(repId),
                            AccountClassificationIds = wholesaleIds,
                            MapIconImagePath = ClassificationColorService.GetMapPinPath(
                                                           repId, item.MapPinImageName)
                        });
                        wholesaleAdded = true;
                    }
                    // Skip subsequent wholesale members — they're already in the group.
                    continue;
                }
                if (item.DisplayOrder != wholesaleDisplayOrder)
                {
                    // Individual (non-Wholesale) entry.
                    result.Add(new MapsLegendFilterUIModel
                    {
                        Title = item.Name,
                        IsSelected = true,
                        BackgroundColor = ClassificationColorService.GetBrush(item.AccountClassificationId),
                        AccountClassificationIds = new List<int> { item.AccountClassificationId },
                        MapIconImagePath = ClassificationColorService.GetMapPinPath(
                                                      item.AccountClassificationId, item.MapPinImageName)
                    });
                }
            }

            return result;
        }
    }
}

