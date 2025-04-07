using System.Collections.Generic;
using Newtonsoft.Json;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartNonstandardExpansionJointEntity : StartAbstractEntity
    {
        [JsonProperty("76")]
        public double EffectiveArea { get; set; }
        
        [JsonProperty("145")]
        public double Length { get; set; }
        
        [JsonProperty("449")]
        public string Name { get; set; }
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                { "Effective Area", EffectiveArea.ToString("F5") },
                { "Length", Length.ToString("F5") },
                { "Name", Name },
            };

            return dictionary;
        }
    }
}