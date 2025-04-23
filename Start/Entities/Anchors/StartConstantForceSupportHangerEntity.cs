using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Anchors
{
    public class StartConstantForceSupportHangerEntity : StartAbstractAnchorEntity
    {
        [JsonProperty(StartPropertyName.AnchorSupportWeight)]
        public double Weight { get; set; }
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Weight", Weight.ToString("F5"));

            return dictionary;
        }
    }
}