using System.Diagnostics;
using Newtonsoft.Json;
using Start.Interfaces;

namespace Start.API
{
    /// <summary>
    ///     Represents an item in the data array, containing metadata and associated entity information.
    /// </summary>
    [DebuggerDisplay("Type = {Type.ToString()}, ID = {DataArrayIndex}, EntityName = {Entity?.Name}")]
    internal class StartDataArrayItem
    {
        /// <summary>
        ///     Gets or sets the entity associated with this item.
        /// </summary>
        [JsonIgnore] public IStartEntity Entity = null!;

        /// <summary>
        ///     Gets or sets the IDs of the nodes associated with this item.
        /// </summary>
        [JsonProperty("nodeIds")]
        public int[] NodeIds { get; set; } = default!;

        /// <summary>
        ///     Gets or sets the type of the element.
        /// </summary>
        [JsonProperty("typeId")]
        public StartElementTypeEnum Type { get; set; }

        /// <summary>
        ///     Gets or sets the index of this item in the data array.
        /// </summary>
        [JsonProperty("dataArrayIndex")]
        public int DataArrayIndex { get; set; }

        /// <summary>
        ///     Gets or sets the unique index of this item in the data array.
        /// </summary>
        [JsonProperty("dataArrayUIndex")]
        public int DataArrayUIndex { get; set; }

        /// <summary>
        ///     Gets or sets the data associated with this item.
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; } = default!;
    }
}