using System.Collections.Generic;
using Newtonsoft.Json;
using Start.Entities.Abstract;

namespace Start.Entities.Anchors
{
    public class StartAnchorEntity : StartAbstractAnchorEntity
    {
        [JsonProperty("269")] 
        public double Mx { get; set; }
        
        [JsonProperty("270")] 
        public double My { get; set; }
        
        [JsonProperty("271")] 
        public double Mz { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Mx", Mx.ToString("F5"));
            dictionary.Add("My", My.ToString("F5"));
            dictionary.Add("Mz", Mz.ToString("F5"));

            return dictionary;
        }
    }
}