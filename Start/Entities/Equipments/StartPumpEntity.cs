using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Equipments
{
    public class StartPumpEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }
        
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; }
        
        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }
        
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        public double PumpCenterCoordX { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        public double PumpCenterCoordY { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        public double PumpCenterCoordZ { get; set; }
        
        [JsonProperty(StartPropertyName.PermissibleExcessFactor)]
        public double PermissibleExcessFactor { get; set; }
        
        [JsonProperty(StartPropertyName.FShaftXAxisAngle)]
        public double FShaftXAxisAngle { get; set; }
        
        [JsonProperty(StartPropertyName.FShaftYAxisAngle)]
        public double FShaftYAxisAngle { get; set; }
        
        [JsonProperty(StartPropertyName.Temperature)]
        public double Temperature { get; set; }
    }
}