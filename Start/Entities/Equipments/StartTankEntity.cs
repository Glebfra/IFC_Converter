using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Equipments
{
    public class StartTankEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.TankDistanceToNozzleAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty DistanceToNozzleAxis { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
        
        [JsonProperty(StartPropertyName.WallThickness)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty WallThickness { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.MillTolerance)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty MillTolerance { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.CorrosionAllowance)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty CorrosionAllowance { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.Temperature)]
        [JsonConverter(typeof(StartPropertyJsonConverter<TemperatureProperty, double>))]
        public TemperatureProperty Temperature { get; set; } = TemperatureProperty.Zero;
        
        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        [JsonConverter(typeof(StartEnumPropertyJsonConverter<StartManufacturingTechnologyEnum>))]
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }

        [JsonProperty(StartPropertyName.Name)] 
        public string Name { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.TankRadius)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty Radius { get; set; } = LengthProperty.Zero;
    }
}