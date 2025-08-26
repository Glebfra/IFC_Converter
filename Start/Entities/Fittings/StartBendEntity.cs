using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public class StartBendEntity : StartAbstractFittingEntity
    {
        [JsonIgnore]
        [StartIgnore]
        public override StartElementType Type { get; set; } = StartElementType.ELBOW;
        
        [JsonProperty(StartPropertyName.WallThickness)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty WallThickness { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.MillTolerance)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty MillTolerance { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }

        [JsonProperty(StartPropertyName.Radius)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty Radius { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.OvalizationCoefficient)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty OvalizationCoefficient { get; set; } = FactorProperty.Zero;

        [JsonProperty(StartPropertyName.NumberOfMilters)]
        [JsonConverter(typeof(StartPropertyJsonConverter<NumberProperty, int>))]
        public NumberProperty NumberOfMilters { get; set; } = NumberProperty.Zero;

        [JsonProperty(StartPropertyName.MillToleranceOutside)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty MillToleranceOutside { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.PipeName)]
        public string Name { get; set; } = string.Empty;
    }
}