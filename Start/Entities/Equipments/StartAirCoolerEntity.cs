using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;

namespace Start.Entities.Equipments
{
    public class StartAirCoolerEntity : StartPumpEntity
    {
        [JsonProperty(StartPropertyName.AirCoolerLength)]
        public double Length { get; set; }

        [JsonProperty(StartPropertyName.PermissibleLoads)]
        public StartPermissibleLoadsEnum PermissibleLoads { get; set; }
        
        [JsonProperty(StartPropertyName.VesselFrad)]
        public double VesselFrad { get; set; }
        
        [JsonProperty(StartPropertyName.VesselFvert)]
        public double VesselFvert { get; set; }
        
        [JsonProperty(StartPropertyName.VesselFshaft)]
        public double VesselFshaft { get; set; }
        
        [JsonProperty(StartPropertyName.VesselMrad)]
        public double VesselMrad { get; set; }
        
        [JsonProperty(StartPropertyName.VesselMvert)]
        public double VesselMvert { get; set; }
        
        [JsonProperty(StartPropertyName.VesselMshaft)]
        public double VesselMshaft { get; set; }
        
        [JsonProperty(StartPropertyName.ManifoldFrad)]
        public double ManifoldFrad { get; set; }
        
        [JsonProperty(StartPropertyName.ManifoldFvert)]
        public double ManifoldFvert { get; set; }
        
        [JsonProperty(StartPropertyName.ManifoldFshaft)]
        public double ManifoldFshaft { get; set; }
        
        [JsonProperty(StartPropertyName.ManifoldMrad)]
        public double ManifoldMrad { get; set; }
        
        [JsonProperty(StartPropertyName.ManifoldMvert)]
        public double ManifoldMvert { get; set; }
        
        [JsonProperty(StartPropertyName.ManifoldMshaft)]
        public double ManifoldMshaft { get; set; }
    }
}