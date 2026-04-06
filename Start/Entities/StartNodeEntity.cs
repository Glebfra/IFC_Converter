using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using Start.API;
using Start.Attributes;
using Start.Converters;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities
{
    /// <summary>
    ///     Represents a node entity in the Start framework.
    /// </summary>
    [StartElement(StartElementTypeEnum.NODE)]
    public sealed class StartNodeEntity : StartAbstractEntity, IStartNodeEntity
    {
        /// <summary>
        ///     Gets or sets the name of the node.
        /// </summary>
        [JsonProperty(StartPropertyName.NodeName)]
        public override string Name { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the description of the node.
        /// </summary>
        [JsonProperty(StartPropertyName.Description)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the X-coordinate of the node.
        /// </summary>
        [JsonProperty(StartPropertyName.XCoord)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> XCoord { get; set; } = new LengthValueProperty<double>();

        /// <summary>
        ///     Gets or sets the Y-coordinate of the node.
        /// </summary>
        [JsonProperty(StartPropertyName.YCoord)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> YCoord { get; set; } = new LengthValueProperty<double>();

        /// <summary>
        ///     Gets or sets the Z-coordinate of the node.
        /// </summary>
        [JsonProperty(StartPropertyName.ZCoord)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> ZCoord { get; set; } = new LengthValueProperty<double>();

        /// <summary>
        ///     Gets the position of the node as a 3D vector.
        /// </summary>
        [JsonIgnore]
        public Vector<double> Position =>
            Vector<double>.Build.Dense(new[] { XCoord.SIProperty, YCoord.SIProperty, ZCoord.SIProperty });
    }
}