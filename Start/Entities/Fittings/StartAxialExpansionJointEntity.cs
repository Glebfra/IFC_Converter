using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    //TODO implement with StartLateralExpansionJointEntity
    public class StartAxialExpansionJointEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty AllowableAxialExpansion { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.AxialFlexibility)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FlexibilityProperty, double>))]
        public FlexibilityProperty AxialFlexibility { get; set; } = FlexibilityProperty.Zero;

        [JsonProperty(StartPropertyName.EffectiveArea)]
        [JsonConverter(typeof(StartPropertyJsonConverter<AreaProperty, double>))]
        public AreaProperty EffectiveArea { get; set; } = AreaProperty.Zero;

        [JsonProperty(StartPropertyName.Length)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty Length { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.EffectiveDiameter)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty EffectiveDiameter { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.StiffnessTempFactor)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StiffnessTempFactor { get; set; } = FactorProperty.Zero;

        [JsonProperty(StartPropertyName.AllowableCorrFactor)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty AllowableCorrFactor { get; set; } = FactorProperty.Zero;
        
        //TODO get measurements
        [JsonProperty(StartPropertyName.AxialStiffness)]
        public double AxialStiffness { get; set; }
    }
}