using Newtonsoft.Json;
using Start.API;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public class StartNonStandardBendEntity : StartBendEntity
    {
        [JsonProperty(StartPropertyName.CorrosionAllowance)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty CorrosionAllowance { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.FlexibilityFactor_k)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty FlexibilityFactor_k { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.StressIntensificationFactor_ii)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StressIntensificationFactor_ii { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.StressIntensificationFactor_io)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StressIntensificationFactor_io { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.StressIntensificationFactor_it)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StressIntensificationFactor_it { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.StressIntensificationFactor_ia)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StressIntensificationFactor_ia { get; set; } = FactorProperty.Zero;
    }
}