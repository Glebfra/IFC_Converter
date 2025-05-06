using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Equipments
{
    public class StartVesselEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.MaterialName)] 
        public string MaterialName { get; set; }
        
        [JsonProperty(StartPropertyName.MillTolerance)] 
        public double MillTolerance { get; set; }
        
        [JsonProperty(StartPropertyName.CorrosionAllowance)] 
        public double CorrosionAllowance { get; set; }
        
        [JsonProperty(StartPropertyName.Temperature)]
        public double Temperature { get; set; }
        
        [JsonProperty(StartPropertyName.ManufacturingTechnology)] 
        public StartManufacturingTechnologyEnum ManufacturingTechnology { get; set; }
        
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        public double ProjectionAlongOXAxis { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        public double ProjectionAlongOYAxis { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        public double ProjectionAlongOZAxis { get; set; }
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }

        [JsonProperty(StartPropertyName.DeviceInternalDiameter)] 
        public double DeviceInternalDiameter { get; set; }
        
        [JsonProperty(StartPropertyName.DeviceWallThickness)] 
        public double DeviceWallThickness { get; set; }
    }
}