using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;

namespace Start.Entities.Abstract
{
    public abstract class StartAbstractFittingEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.Weight)]
        public double Weight { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                { "Weight", Weight.ToString("F5") },
            };
            
            return dictionary;
        }
    }
}