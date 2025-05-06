using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Segments
{
    public class StartRigidElementEntity : StartAbstractSegmentEntity
    {
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.Weight)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassUnitProperty, double>))]
        public MassUnitProperty PipeUnitWeight { get; set; } = MassUnitProperty.Zero;

        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ProjectionAlongOXAxis { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ProjectionAlongOYAxis { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ProjectionAlongOZAxis { get; set; } = LengthProperty.Zero;
    }
}