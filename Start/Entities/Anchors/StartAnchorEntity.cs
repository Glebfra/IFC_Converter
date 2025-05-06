using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Anchors
{
    public class StartAnchorEntity : StartAbstractAnchorEntity
    {
        [JsonProperty(StartPropertyName.Mx)] 
        public double Mx { get; set; }
        
        [JsonProperty(StartPropertyName.My)] 
        public double My { get; set; }
        
        [JsonProperty(StartPropertyName.Mz)] 
        public double Mz { get; set; }
    }
}