using DRLMobile.Core.Models.UIModels;
using System.Collections.ObjectModel;

namespace DRLMobile.Core.Interface
{
    public interface IMapsStaticDataSourceHelper
    {
        ObservableCollection<PlotByTypeFilterUIModel> GetPlotByTypeFiltersDataSource();
        ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForTradeType();
        ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForRank();
        ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForCallDate();
        ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForItemNo();
        ObservableCollection<MapsLegendFilterUIModel> MapLegendsFiltersDataSourceForCashSales();

    }
}
