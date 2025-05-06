using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities
{
    public class StartNodeEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.AdditionalWeightLoad)] 
        public double AdditionalLoadFromWeight { get; set; }
        
        [JsonProperty(StartPropertyName.PipeName)] 
        public string Name { get; set; }
        
        [JsonProperty(StartPropertyName.Description)] 
        public string Description { get; set; }

        [JsonProperty(StartPropertyName.XCoord)]
        public double XCoord;
        
        [JsonProperty(StartPropertyName.YCoord)]
        public double YCoord;
        
        [JsonProperty(StartPropertyName.ZCoord)]
        public double ZCoord;
    }
}