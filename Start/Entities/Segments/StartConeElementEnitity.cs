using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using Start.API;
using Start.Attributes;
using Start.Converters;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities.Segments
{
    /// <summary>
    ///     Represents a cone element entity in the Start framework.
    ///     Implements the <see cref="IStartMaterializedEntity" /> and <see cref="IStartConeEntity" /> interfaces.
    /// </summary>
    [StartElement(StartElementTypeEnum.CONE_ELEMENT)]
    public sealed class StartConeElementEntity : StartAbstractSegmentEntity,
        IStartMaterializedEntity, IStartConeEntity
    {
        /// <summary>
        ///     Gets or sets the second diameter of the cone element.
        /// </summary>
        [JsonProperty(StartPropertyName.ConeElementSecondDiameter)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> SecondDiameter { get; set; } = new LengthValueProperty<double>();

        /// <summary>
        ///     Gets the position of the cone element, which corresponds to its start position.
        /// </summary>
        public Vector<double> Position => StartPosition;

        /// <summary>
        ///     Gets the points defining the cone element, including the start and end positions.
        ///     This property is ignored during JSON serialization and by the Start framework.
        /// </summary>
        [JsonIgnore]
        [StartIgnore]
        public IEnumerable<Vector<double>> Points => new[] { StartPosition, EndPosition };

        /// <summary>
        ///     Gets the diameters of the cone element, including the primary diameter and the second diameter.
        ///     This property is ignored during JSON serialization and by the Start framework.
        /// </summary>
        [JsonIgnore]
        [StartIgnore]
        public IEnumerable<double> Diameters => new[] { Diameter.SIProperty, SecondDiameter.SIProperty };

        /// <summary>
        ///     Gets or sets the name of the material associated with the cone element.
        /// </summary>
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
    }
}