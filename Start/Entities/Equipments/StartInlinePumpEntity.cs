using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Fittings;
using Start.StartProperties;

namespace Start.Entities.Equipments
{
    public class StartInlinePumpEntity : StartArmatureEntity
    {
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
        
        [JsonProperty(StartPropertyName.Temperature)]
        [JsonConverter(typeof(StartPropertyJsonConverter<TemperatureProperty, double>))]
        public TemperatureProperty Temperature { get; set; } = TemperatureProperty.Zero;
        
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty PumpCenterCoordX { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty PumpCenterCoordY { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty PumpCenterCoordZ { get; set; } = LengthProperty.Zero;
    }
}