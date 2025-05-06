using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public class StartReducerEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.ConicalPartLength)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty LengthOfConicalPart { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.Diameter)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty MaxDiameter { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.MinDiameter)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty MinDiameter { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.WallThickness)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ThicknessAtMaxDiameterPoint { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.MillTolerance)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty MillToleranceAtDMax { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }

        [JsonProperty(StartPropertyName.AngleBetweenEccentricityVectorAndZmAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<AngleProperty, double>))]
        public AngleProperty AngleBetweenEccentricityVectorAndZmAxis { get; set; } = AngleProperty.Zero;

        [JsonProperty(StartPropertyName.PipeName)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.MillToleranceAtDMin)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty MillToleranceAtDMin { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.ReducerMillTolerance)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty MillTolerance { get; set; } = LengthProperty.Zero;
    }
}