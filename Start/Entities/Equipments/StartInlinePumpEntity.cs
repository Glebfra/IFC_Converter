using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Fittings;

namespace Start.Entities.Equipments
{
    public class StartInlinePumpEntity : StartArmatureEntity
    {
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; }
        
        [JsonProperty(StartPropertyName.Temperature)]
        public double Temperature { get; set; }
        
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        public double PumpCenterCoordX { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        public double PumpCenterCoordY { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        public double PumpCenterCoordZ { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Material Name", MaterialName);
            dictionary.Add("Temperature", Temperature.ToString("F5"));
            dictionary.Add("Pump Center CoordX", PumpCenterCoordX.ToString("F5"));
            dictionary.Add("Pump Center CoordY", PumpCenterCoordY.ToString("F5"));
            dictionary.Add("Pump Center CoordZ", PumpCenterCoordZ.ToString("F5"));

            return dictionary;
        }
    }
}