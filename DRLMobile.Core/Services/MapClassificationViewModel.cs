using DRLMobile.Core.Interface;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.ExceptionHandler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRLMobile.Core.Services
{
    // ──────────────────────────────────────────────────────────────────────────────
    // MapClassificationViewModel
    // Flattened view-model produced by MapClassificationService.
    // Combines MapClassification (colour/order/active) with Classification (name).
    // This lives in Services because it is only ever used by the Map feature.
    // ──────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Merged view of <see cref="MapClassification"/> and <see cref="Classification"/>,
    /// ready for the map legend and pin-generation pipeline.
    /// </summary>
    public class MapClassificationViewModel
    {
        /// <summary>Maps to <see cref="Classification.AccountClassificationId"/>.</summary>
        public int AccountClassificationId { get; set; }

        /// <summary>Display name from the <see cref="Classification"/> table.</summary>
        public string Name { get; set; }

        /// <summary>
        /// 6-digit hex colour code from <see cref="MapClassification.HexColorCode"/>.
        /// Null or empty means the dynamic gradient generator is used
        /// (produces the same colour for every user, based on AccountClassificationId).
        /// </summary>
        public string HexColorCode { get; set; }

        /// <summary>Ascending sort position in the legend.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>CustomerType value from <see cref="Classification"/>. 1 = Wholesale group.</summary>
        public int CustomerType { get; set; }

        /// <summary>
        /// Static map-pin image filename from <see cref="MapClassification.MapPinImageName"/>.
        /// When null/empty the pin is generated dynamically by MapPinGenerator and cached locally.
        /// </summary>
        public string MapPinImageName { get; set; }
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // MapClassificationService
    // ──────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Map-only service that reads the <c>MapClassification</c> SQLite table and
    /// merges its rows with the <c>Classification</c> table to produce a sorted,
    /// filtered list ready for the legend UI and pin-generation pipeline.
    ///
    /// This service is intentionally decoupled from the existing
    /// <c>QueryService.GetClassificationDict()</c> flow so that other ViewModels
    /// (CustomerList, CustomerPage, etc.) are completely unaffected.
    /// </summary>
    public static class MapClassificationService
    {
        /// <summary>
        /// Loads active <see cref="MapClassification"/> rows, joins them with
        /// <see cref="Classification"/> names, and returns the list sorted by
        /// <see cref="MapClassificationViewModel.DisplayOrder"/> ascending.
        ///
        /// Rows where <c>IsActive == 0</c> are excluded entirely (they will not
        /// appear in the legend or as map pins).
        ///
        /// When <c>HexColorCode</c> is null or empty for a row, the caller
        /// (<see cref="DRLMobile.Uwp.Helpers.ClassificationColorService"/>) will
        /// fall back to the deterministic gradient generator — producing the same
        /// colour for every user regardless of install/reinstall.
        /// </summary>
        public static async Task<List<MapClassificationViewModel>> GetActiveMapClassificationsAsync(
            IDatabaseService dbService)
        {
            if (dbService == null)
                throw new ArgumentNullException(nameof(dbService));

            try
            {
                // Load both tables in parallel for efficiency.
                var mapRowsTask = dbService.GetMapClassificationsAsync();
                var classDictTask = dbService.GetClassificationDictionaryAsync();

                await Task.WhenAll(mapRowsTask, classDictTask).ConfigureAwait(false);

                var mapRows = mapRowsTask.Result ?? new List<MapClassification>();
                var classDict = classDictTask.Result ?? new Dictionary<int, Classification>();

                return mapRows
                    .Where(r => r.IsActive == 1 && r.DisplayOrder > 0)
                    .OrderBy(r => r.DisplayOrder)
                    .ThenBy(r => r.AccountClassificationId)
                    .Select(r =>
                    {
                        // Join Classification only for Name and CustomerType.
                        // HexColorCode and MapPinImageName come exclusively from
                        // MapClassification so Classification.cs is never modified.
                        classDict.TryGetValue(r.AccountClassificationId, out var cls);
                        return new MapClassificationViewModel
                        {
                            AccountClassificationId = r.AccountClassificationId,
                            DisplayOrder = r.DisplayOrder,
                            // ── Source: MapClassification table ──────────────────
                            HexColorCode = string.IsNullOrWhiteSpace(r.HexColorCode)
                                                 ? null
                                                 : r.HexColorCode.TrimStart('#'),
                            MapPinImageName = r.MapPinImageName,   // from MapClassification
                            // ── Source: Classification table (name / type only) ──
                            Name = cls?.AccountClassificationName
                                           ?? $"Classification {r.AccountClassificationId}",
                            CustomerType = cls?.CustomerType ?? 0,
                        };
                    })
                    .ToList();

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(
                    nameof(MapClassificationService),
                    nameof(GetActiveMapClassificationsAsync),
                    ex.Message);
                return new List<MapClassificationViewModel>();
            }
        }
    }
}
