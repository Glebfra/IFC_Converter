using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public class StartUniversalExpansionJointEntity : StartAbstractFittingEntity
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
        
        //TODO get measurements
        [JsonProperty(StartPropertyName.ShearCompliance)]
        public double ShearCompliance { get; set; }
        
        //TODO get measurements
        [JsonProperty(StartPropertyName.PermissibleLateralMovement)]
        public double PermissibleLateralMovement { get; set; }

        [JsonProperty(StartPropertyName.StiffnessTempFactor)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StiffnessTempFactor { get; set; } = FactorProperty.Zero;

        [JsonProperty(StartPropertyName.AllowableCorrFactor)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty AllowableCorrFactor { get; set; } = FactorProperty.Zero;

        [JsonProperty(StartPropertyName.StiffnessAngleFactor)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StiffnessAngleFactor { get; set; } = FactorProperty.Zero;

        [JsonProperty(StartPropertyName.AngleAllowableCorrFactor)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty AngleAllowableCorrFactor { get; set; } = FactorProperty.Zero;

        [JsonProperty(StartPropertyName.StiffnessShearFactor)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StiffnessShearFactor { get; set; } = FactorProperty.Zero;

        [JsonProperty(StartPropertyName.ShearAllowableCorrFactor)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty ShearAllowableCorrFactor { get; set; } = FactorProperty.Zero;
    }
}