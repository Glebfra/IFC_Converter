using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartNonstandardExpansionJointEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.EffectiveArea)]
        public double EffectiveArea { get; set; }
        
        [JsonProperty(StartPropertyName.Length)]
        public double Length { get; set; }
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Effective Area", EffectiveArea.ToString("F5"));
            dictionary.Add("Length", Length.ToString("F5"));
            dictionary.Add("Name", Name);

            return dictionary;
        }
    }
}