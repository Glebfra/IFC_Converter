using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public class StartConnectorEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.ConnectionType)]
        public StartConnectionTypeEnum ConnectionType { get; set; }
        
        [JsonProperty(StartPropertyName.StrengthFactorOfTheTraverseWeld)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StrengthFactorOfTheTraverseWeld { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.StrengthFactorOfTheTransverseTension)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StrengthFactorOfTheTransverseTension { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.StressIntensificationFactor_ii)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StressIntensificationFactor_ii { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.StressIntensificationFactor_io)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StressIntensificationFactor_io { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.StressIntensificationFactor_ia)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StressIntensificationFactor_ia { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.StressIntensificationFactor_it)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StressIntensificationFactor_it { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.SeamDeltaParameter)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty SeamDeltaParameter { get; set; } = LengthProperty.Zero;
    }
}