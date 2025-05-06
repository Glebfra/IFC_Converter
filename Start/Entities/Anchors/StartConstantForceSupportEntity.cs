using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Anchors
{
    public class StartConstantForceSupportEntity : StartAbstractAnchorEntity
    {
        [JsonProperty(StartPropertyName.FrictionMoment)]
        public double FrictionMoment { get; set; }
        
        [JsonProperty(StartPropertyName.SupportsNumber)]
        public int SupportsNumber { get; set; }
        
        [JsonProperty(StartPropertyName.AnchorSupportWeight)]
        public double Weight { get; set; }
    }
}