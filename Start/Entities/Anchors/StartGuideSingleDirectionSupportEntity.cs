using System.Collections.Generic;
using Newtonsoft.Json;
using Start.Entities.Abstract;

namespace Start.Entities.Anchors
{
    public class StartGuideSingleDirectionSupportEntity : StartAbstractAnchorEntity
    {
        [JsonProperty("1282")]
        public double Weight { get; set; }
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Weight", Weight.ToString("F5"));

            return dictionary;
        }
    }
}