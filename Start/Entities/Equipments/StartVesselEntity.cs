using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Equipments
{
    public class StartVesselEntity : StartAbstractEntity
    {
        [JsonProperty("7")] 
        public string MaterialName { get; set; }
        
        [JsonProperty("13")] 
        public double MillTolerance { get; set; }
        
        [JsonProperty("14")] 
        public double CorrosionAllowance { get; set; }
        
        [JsonProperty("30")]
        public double Temperature { get; set; }
        
        [JsonProperty("37")] 
        public StartManufacturingTechnologyEnum ManufacturingTechnology { get; set; }
        
        [JsonProperty("128")]
        public double ProjectionAlongOXAxis { get; set; }
    
        [JsonProperty("129")]
        public double ProjectionAlongOYAxis { get; set; }
    
        [JsonProperty("130")]
        public double ProjectionAlongOZAxis { get; set; }
        
        [JsonProperty("449")]
        public string Name { get; set; }
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new()
            {
                { "Name", Name },
                { "Material Name", MaterialName },
                { "Mill Tolerance", MillTolerance.ToString("F5") },
                { "Manufacturing Technology", ManufacturingTechnology.ToString() },
                { "Corrosion Allowance", CorrosionAllowance.ToString("F5") },
                { "Temperature", Temperature.ToString("F5") },
                { "Projection Along OX Axis", ProjectionAlongOXAxis.ToString("F5") },
                { "Projection Along OY Axis", ProjectionAlongOYAxis.ToString("F5") },
                { "Projection Along OZ Axis", ProjectionAlongOZAxis.ToString("F5") }
            };

            return dictionary;
        }
    }
}