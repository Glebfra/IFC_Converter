using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartTorsionExpansionJointEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        public double AllowableAxialExpansion { get; set; }

        [JsonProperty(StartPropertyName.FrictionMoment)]
        public double FrictionMoment { get; set; }
        
        [JsonProperty(StartPropertyName.Length)]
        public double Length { get; set; }
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Allowable Axial Expansion", AllowableAxialExpansion.ToString("F5"));
            dictionary.Add("Friction Moment", FrictionMoment.ToString("F5"));
            dictionary.Add("Length", Length.ToString("F5"));
            dictionary.Add("Name", Name);

            return dictionary;
        }
    }
}