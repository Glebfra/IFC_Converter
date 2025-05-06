using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Equipments
{
    public class StartPumpEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.Name)] 
        public string Name { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
        
        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }
        
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty PumpCenterCoordX { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty PumpCenterCoordY { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty PumpCenterCoordZ { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.PermissibleExcessFactor)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty PermissibleExcessFactor { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.FShaftXAxisAngle)]
        [JsonConverter(typeof(StartPropertyJsonConverter<AngleProperty, double>))]
        public AngleProperty FShaftXAxisAngle { get; set; } = AngleProperty.Zero;
        
        [JsonProperty(StartPropertyName.FShaftYAxisAngle)]
        [JsonConverter(typeof(StartPropertyJsonConverter<AngleProperty, double>))]
        public AngleProperty FShaftYAxisAngle { get; set; } = AngleProperty.Zero;
        
        [JsonProperty(StartPropertyName.Temperature)]
        [JsonConverter(typeof(StartPropertyJsonConverter<TemperatureProperty, double>))]
        public TemperatureProperty Temperature { get; set; } = TemperatureProperty.Zero;
    }
}