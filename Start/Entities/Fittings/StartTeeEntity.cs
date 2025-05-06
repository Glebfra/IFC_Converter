using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public class StartTeeEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.WallThickness)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty HeaderThickness { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.MillTolerance)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty MillTolerance { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.HeaderLength)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty HeaderLength { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.BranchWallThickness)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty BranchWallThickness { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.MillToleranceForBranch)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty MillToleranceForBranch { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.BranchHeight)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty BranchHeight { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.PadThickness)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty PadThickness { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.PadWidth)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty PadWidth { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.CrotchHeight)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty CrotchHeight { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.CrotchThickness)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty CrotchThickness { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.LongitudinalWeldJointFactor)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StrengthFactorOfLongitudinalWeldSeamOnPressure { get; set; } = FactorProperty.Zero;

        [JsonProperty(StartPropertyName.CrotchRadius)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty CrotchRadius { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.PipeName)]
        public string Name { get; set; } = string.Empty;
    }
}