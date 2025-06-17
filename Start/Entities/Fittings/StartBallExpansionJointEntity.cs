using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public class StartBallExpansionJointEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty AllowableAxialExpansion { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.Length)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty Length { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.FrictionMoment1)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty FrictionMoment1 { get; set; } = MomentProperty.Zero;

        [JsonProperty(StartPropertyName.Pressure1)]
        [JsonConverter(typeof(StartPropertyJsonConverter<PressureProperty, double>))]
        public PressureProperty Pressure1 { get; set; } = PressureProperty.Zero;

        [JsonProperty(StartPropertyName.FrictionMoment2)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty FrictionMoment2 { get; set; } = MomentProperty.Zero;

        [JsonProperty(StartPropertyName.Pressure2)]
        [JsonConverter(typeof(StartPropertyJsonConverter<PressureProperty, double>))]
        public PressureProperty Pressure2 { get; set; } = PressureProperty.Zero;

        [JsonProperty(StartPropertyName.FrictionMoment3)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty FrictionMoment3 { get; set; } = MomentProperty.Zero;

        [JsonProperty(StartPropertyName.Pressure3)]
        [JsonConverter(typeof(StartPropertyJsonConverter<PressureProperty, double>))]
        public PressureProperty Pressure3 { get; set; } = PressureProperty.Zero;
    }
}