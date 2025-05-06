using Newtonsoft.Json;
using Start.API;
using Start.StartProperties;

namespace Start.Entities.Abstract
{
    public class StartAbstractSegmentEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.PipeName)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.Diameter)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty Diameter { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.WallThickness)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty WallThickness { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.Pressure)]
        [JsonConverter(typeof(StartPropertyJsonConverter<PressureProperty, double>))]
        public PressureProperty Pressure { get; set; } = PressureProperty.Zero;

        [JsonProperty(StartPropertyName.TestPressure)]
        [JsonConverter(typeof(StartPropertyJsonConverter<PressureProperty, double>))]
        public PressureProperty TestPressure { get; set; } = PressureProperty.Zero;

        [JsonProperty(StartPropertyName.Temperature)]
        [JsonConverter(typeof(StartPropertyJsonConverter<TemperatureProperty, double>))]
        public TemperatureProperty Temperature { get; set; } = TemperatureProperty.Zero;

        [JsonIgnore]
        public LengthProperty InnerDiameter => new LengthProperty(Diameter.StartProperty - WallThickness.StartProperty);
        
        [JsonIgnore]
        public PressureProperty[] PressureRange => new PressureProperty[] {Pressure, TestPressure};
        
        [JsonIgnore]
        public TemperatureProperty[] TemperatureRange => new TemperatureProperty[] {Temperature, Temperature};
    }
}