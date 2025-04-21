using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Equipments
{
    public class StartTankEntity : StartAbstractEntity
    {
        [JsonProperty("2")]
        public double DistanceToNozzleAxis { get; set; }
        
        [JsonProperty("7")]
        public string MaterialName { get; set; }
        
        [JsonProperty("9")]
        public double WallThickness { get; set; }
        
        [JsonProperty("13")]
        public double MillTolerance { get; set; }
        
        [JsonProperty("14")]
        public double CorrosionAllowance { get; set; }
        
        [JsonProperty("30")]
        public double Temperature { get; set; }
        
        [JsonProperty("37")]
        public StartManufacturingTechnologyEnum ManufacturingTechnology { get; set; }
        
        [JsonProperty("449")]
        public string Name { get; set; }
        
        [JsonProperty("899")]
        public double Radius { get; set; }
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new()
            {
                { "Name", Name },
                { "Material Name", MaterialName },
                { "Wall Thickness", WallThickness.ToString("F5") },
                { "Mill Tolerance", MillTolerance.ToString("F5") },
                { "Manufacturing Technology", ManufacturingTechnology.ToString() },
                { "Corrosion Allowance", CorrosionAllowance.ToString("F5") },
                { "Temperature", Temperature.ToString("F5") },
            };

            return dictionary;
        }
    }
}