using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartFlexibleElementEntity : StartAbstractEntity
    {
        [JsonProperty("2")]
        public double Length { get; set; }
        
        [JsonProperty("7")] 
        public string MaterialName { get; set; }
        
        [JsonProperty("34")] 
        public double PipeUnitWeight { get; set; }
        
        [JsonProperty("61")]
        public double AdditionalWeightLoad { get; set; }
        
        [JsonProperty("64")] 
        public double AdditionalWeightLoadAlongTheXAxis { get; set; }
    
        [JsonProperty("65")] 
        public double AdditionalWeightLoadAlongTheYAxis { get; set; }
    
        [JsonProperty("66")] 
        public double AdditionalWeightLoadAlongTheZAxis { get; set; }
        
        [JsonProperty("128")]
        public double ProjectionAlongOXAxis { get; set; }
    
        [JsonProperty("129")]
        public double ProjectionAlongOYAxis { get; set; }
    
        [JsonProperty("130")]
        public double ProjectionAlongOZAxis { get; set; }
        
        [JsonProperty("225")] 
        public string Name { get; set; }
        
        [JsonProperty("311")]
        public double UniformWeight { get; set; }
        
        [JsonProperty("1369")]
        public double ShearStiffness { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new()
            {
                { "Name", Name },
                { "Material Name", MaterialName },
                { "Pipe Unit Weight", PipeUnitWeight.ToString("F5") },
                { "Additional Weight Load", AdditionalWeightLoad.ToString("F5") },
                { "Additional Weight Load along the X Axis", AdditionalWeightLoadAlongTheXAxis.ToString("F5") },
                { "Additional Weight Load along the Y Axis", AdditionalWeightLoadAlongTheYAxis.ToString("F5") },
                { "Additional Weight Load along the Z Axis", AdditionalWeightLoadAlongTheZAxis.ToString("F5") },
                { "Projection Along OX Axis", ProjectionAlongOXAxis.ToString("F5") },
                { "Projection Along OY Axis", ProjectionAlongOYAxis.ToString("F5") },
                { "Projection Along OZ Axis", ProjectionAlongOZAxis.ToString("F5") },
                { "Uniform Weight", UniformWeight.ToString("F5") },
                { "Shear Stiffness", ShearStiffness.ToString("F5") },
            };

            return dictionary;
        }
    }
}