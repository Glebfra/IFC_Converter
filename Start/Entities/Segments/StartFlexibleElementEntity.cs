using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Segments
{
    public class StartFlexibleElementEntity : StartAbstractSegmentEntity
    {
        [JsonProperty(StartPropertyName.FlexibleElementLength)]
        public double Length { get; set; }
        
        [JsonProperty(StartPropertyName.MaterialName)] 
        public string MaterialName { get; set; }
        
        [JsonProperty(StartPropertyName.Weight)] 
        public double PipeUnitWeight { get; set; }
        
        [JsonProperty(StartPropertyName.AdditionalWeightLoad)]
        public double AdditionalWeightLoad { get; set; }
        
        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheXAxis)] 
        public double AdditionalWeightLoadAlongTheXAxis { get; set; }
    
        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheYAxis)] 
        public double AdditionalWeightLoadAlongTheYAxis { get; set; }
    
        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheZAxis)] 
        public double AdditionalWeightLoadAlongTheZAxis { get; set; }
        
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        public double ProjectionAlongOXAxis { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        public double ProjectionAlongOYAxis { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        public double ProjectionAlongOZAxis { get; set; }

        [JsonProperty(StartPropertyName.UniformWeight)]
        public double UniformWeight { get; set; }
        
        [JsonProperty(StartPropertyName.ShearStiffness)]
        public double ShearStiffness { get; set; }
    }
}