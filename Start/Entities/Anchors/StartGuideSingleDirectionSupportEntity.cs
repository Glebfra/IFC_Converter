using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Anchors
{
    public class StartGuideSingleDirectionSupportEntity : StartAbstractAnchorEntity
    {
        [JsonProperty(StartPropertyName.AnchorSupportWeight)]
        public double Weight { get; set; }
    }
}