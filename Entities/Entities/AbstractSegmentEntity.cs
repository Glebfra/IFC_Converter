using Entities.Attributes;
using Entities.Entities.Properties;
using Newtonsoft.Json;

namespace Entities.Entities
{
    public abstract class AbstractSegmentEntity
    {
        [JsonProperty(PropertyName.PipeName)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(PropertyName.Diameter)]
        [JsonConverter(typeof(PropertyValueJsonConverter<LengthProperty, double>))]
        public LengthProperty Diameter { get; set; } = LengthProperty.Zero;

        [JsonProperty(PropertyName.WallThickness)]
        [JsonConverter(typeof(PropertyValueJsonConverter<LengthProperty, double>))]
        public LengthProperty WallThickness { get; set; } = LengthProperty.Zero;

        [JsonProperty(PropertyName.Pressure)]
        [JsonConverter(typeof(PropertyValueJsonConverter<PressureProperty, double>))]
        public PressureProperty Pressure { get; set; } = PressureProperty.Zero;

        [JsonProperty(PropertyName.TestPressure)]
        [JsonConverter(typeof(PropertyValueJsonConverter<PressureProperty, double>))]
        public PressureProperty TestPressure { get; set; } = PressureProperty.Zero;

        [JsonProperty(PropertyName.Temperature)]
        [JsonConverter(typeof(PropertyValueJsonConverter<TemperatureProperty, double>))]
        public TemperatureProperty Temperature { get; set; } = TemperatureProperty.Zero;

        [JsonIgnore]
        [PropertyIgnore]
        public LengthProperty InnerDiameter => new LengthProperty(Diameter.StartProperty - WallThickness.StartProperty);
        
        [JsonIgnore]
        [PropertyIgnore]
        public PressureProperty[] PressureRange => new PressureProperty[] {Pressure, TestPressure};
        
        [JsonIgnore]
        [PropertyIgnore]
        public TemperatureProperty[] TemperatureRange => new TemperatureProperty[] {Temperature, Temperature};
    }
}