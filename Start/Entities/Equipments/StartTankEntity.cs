using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Equipments
{
    public class StartTankEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.TankDistanceToNozzleAxis)]
        public double DistanceToNozzleAxis { get; set; }
        
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; }
        
        [JsonProperty(StartPropertyName.WallThickness)]
        public double WallThickness { get; set; }
        
        [JsonProperty(StartPropertyName.MillTolerance)]
        public double MillTolerance { get; set; }
        
        [JsonProperty(StartPropertyName.CorrosionAllowance)]
        public double CorrosionAllowance { get; set; }
        
        [JsonProperty(StartPropertyName.Temperature)]
        public double Temperature { get; set; }
        
        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        public StartManufacturingTechnologyEnum ManufacturingTechnology { get; set; }
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }
        
        [JsonProperty(StartPropertyName.TankRadius)]
        public double Radius { get; set; }
    }
}