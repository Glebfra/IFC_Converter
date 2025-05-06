using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Equipments
{
    public class StartVesselEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
        
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
        public StartManufacturingTechnologyEnum ManufacturingTechnology { get; set; }
        
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ProjectionAlongOXAxis { get; set; } = LengthProperty.Zero;
    
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ProjectionAlongOYAxis { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ProjectionAlongOZAxis { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.DeviceInternalDiameter)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty DeviceInternalDiameter { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.DeviceWallThickness)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty DeviceWallThickness { get; set; } = LengthProperty.Zero;
    }
}