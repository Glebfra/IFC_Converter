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
        
        [JsonProperty(StartPropertyName.Temperature)]
        public double Temperature { get; set; }
        
        [JsonProperty(StartPropertyName.ManufacturingTechnology)] 
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }
        
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        public double ProjectionAlongOXAxis { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        public double ProjectionAlongOYAxis { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        public double ProjectionAlongOZAxis { get; set; }
        
        [JsonProperty(StartPropertyName.PermissibleExcessFactor)]
        public double PermissibleExcessFactor { get; set; }
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new()
            {
                { "Name", Name },
                { "Material Name", MaterialName },
                { "Temperature", Temperature.ToString("F5") },
                { "Manufacturing Technology", ManufacturingTechnologyEnum.ToString() },
                { "Projection Along OX Axis", ProjectionAlongOXAxis.ToString("F5") },
                { "Projection Along OY Axis", ProjectionAlongOYAxis.ToString("F5") },
                { "Projection Along OZ Axis", ProjectionAlongOZAxis.ToString("F5") },
                { "Permissible Excess Factor", PermissibleExcessFactor.ToString("F5") },
            };

            return dictionary;
        }
    }
}