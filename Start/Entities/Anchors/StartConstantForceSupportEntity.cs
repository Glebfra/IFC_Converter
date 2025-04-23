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
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                { "Friction Moment", FrictionMoment.ToString("F5") },
                { "Supports Number", SupportsNumber.ToString() },
                { "Weight", Weight.ToString("F5") }
            };

            return dictionary;
        }
    }
}