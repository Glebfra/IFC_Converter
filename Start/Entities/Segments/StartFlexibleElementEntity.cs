using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Segments
{
    public class StartFlexibleElementEntity : StartAbstractSegmentEntity
    {
        [JsonProperty(StartPropertyName.FlexibleElementLength)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty Length { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.Weight)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassUnitProperty, double>))]
        public MassUnitProperty PipeUnitWeight { get; set; } = MassUnitProperty.Zero;

        [JsonProperty(StartPropertyName.AdditionalWeightLoad)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassUnitProperty, double>))]
        public MassUnitProperty AdditionalWeightLoad { get; set; } = MassUnitProperty.Zero;

        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheXAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassUnitProperty, double>))]
        public MassUnitProperty AdditionalWeightLoadAlongTheXAxis { get; set; } = MassUnitProperty.Zero;

        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheYAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassUnitProperty, double>))]
        public MassUnitProperty AdditionalWeightLoadAlongTheYAxis { get; set; } = MassUnitProperty.Zero;

        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheZAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassUnitProperty, double>))]
        public MassUnitProperty AdditionalWeightLoadAlongTheZAxis { get; set; } = MassUnitProperty.Zero;

        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ProjectionAlongOXAxis { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ProjectionAlongOYAxis { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ProjectionAlongOZAxis { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.UniformWeight)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassProperty, double>))]
        public MassProperty UniformWeight { get; set; } = MassProperty.Zero;
        
        //TODO get measurements
        [JsonProperty(StartPropertyName.ShearStiffness)]
        public double ShearStiffness { get; set; }
    }
}