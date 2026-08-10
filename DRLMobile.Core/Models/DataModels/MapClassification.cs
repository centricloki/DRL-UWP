using Newtonsoft.Json;

using SQLite;

using System;

namespace DRLMobile.Core.Models.DataModels
{
    /// <summary>
    /// ORM model for the SQLite <c>MapClassification</c> table.
    /// Each row controls whether a classification appears on the map / legend,
    /// in what order it is listed, and what colour its pin should use.
    /// </summary>
    [Table("MapClassification")]
    public class MapClassification
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>FK to <see cref="Classification.AccountClassificationId"/>.</summary>
        [Indexed]
        public int AccountClassificationId { get; set; }

        /// <summary>1 = visible in map + legend, 0 = hidden.</summary>
        public int IsActive { get; set; }

        /// <summary>
        /// Ascending sort order in the legend.
        /// Value of −1 indicates a system/unordered entry; shown last.
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Optional 6-digit hex colour code (without #), e.g. "FF5733".
        /// When null or empty the dynamic gradient generator in
        /// ClassificationColorService is used.
        /// </summary>
        public string HexColorCode { get; set; }

        /// <summary>
        /// Optional static map-pin image filename (e.g. "MapPin-Red.png").
        /// When null or empty, a pin is generated dynamically at runtime by MapPinGenerator.
        /// </summary>
        public string MapPinImageName { get; set; }

        public string UpdatedDate { get; set; }
    }
}
