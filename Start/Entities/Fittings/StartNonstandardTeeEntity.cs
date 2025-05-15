using Newtonsoft.Json;
using Start.API;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public class StartNonstandardTeeEntity : StartTeeEntity
    {
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
        
        [JsonProperty(StartPropertyName.StressIntensificationFactor_ii)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty MainLineIntensificationFactor_ii { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.StressIntensificationFactor_it)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty MainLineIntensificationFactor_it { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.StressIntensificationFactor_ia)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty MainLineIntensificationFactor_ia { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.StressIntensificationFactor_io)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty MainLineIntensificationFactor_io { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.MainLineFlexibilityFactor_ki)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty MainLineFlexibilityFactor_ki { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.MainLineFlexibilityFactor_ko)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty MainLineFlexibilityFactor_ko { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.MainLineFlexibilityFactor_kt)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty MainLineFlexibilityFactor_kt { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.MainLineFlexibilityFactor_ka)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty MainLineFlexibilityFactor_ka { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.BranchIntensificationFactor_ii)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty BranchIntensificationFactor_ii { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.BranchIntensificationFactor_io)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty BranchIntensificationFactor_io { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.BranchIntensificationFactor_it)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty BranchIntensificationFactor_it { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.BranchIntensificationFactor_ia)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty BranchIntensificationFactor_ia { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.BranchFlexibilityFactor_ki)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty BranchFlexibilityFactor_ki { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.BranchFlexibilityFactor_ko)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty BranchFlexibilityFactor_ko { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.BranchFlexibilityFactor_kt)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty BranchFlexibilityFactor_kt { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.BranchFlexibilityFactor_ka)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty BranchFlexibilityFactor_ka { get; set; } = FactorProperty.Zero;
    }
}