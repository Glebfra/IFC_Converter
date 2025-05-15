using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public class StartAngularExpansionJointEntity : StartAbstractFittingEntity
    {
        //TODO get measurements
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        public double AllowableAxialExpansion { get; set; }

        [JsonProperty(StartPropertyName.AxialFlexibility)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FlexibilityProperty, double>))]
        public FlexibilityProperty AxialFlexibility { get; set; } = FlexibilityProperty.Zero;

        [JsonProperty(StartPropertyName.Length)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty Length { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; } = string.Empty;

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