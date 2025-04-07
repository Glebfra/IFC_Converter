using System.Collections.Generic;
using Newtonsoft.Json;
using Start.Entities.Abstract;

namespace Start.Entities.Anchors
{
    public class StartSlidingSupportEntity : StartAbstractAnchorEntity
    {
        [JsonProperty("72")]
        public double FrictionMoment { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Friction Moment", FrictionMoment.ToString("F5"));

            return dictionary;
        }
    }
}