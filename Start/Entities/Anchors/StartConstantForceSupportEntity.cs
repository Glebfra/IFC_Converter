using System.Collections.Generic;
using Newtonsoft.Json;
using Start.Entities.Abstract;

namespace Start.Entities.Anchors
{
    public class StartConstantForceSupportEntity : StartAbstractAnchorEntity
    {
        [JsonProperty("72")]
        public double FrictionMoment { get; set; }
        
        [JsonProperty("146")]
        public int SupportsNumber { get; set; }
        
        [JsonProperty("1282")]
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