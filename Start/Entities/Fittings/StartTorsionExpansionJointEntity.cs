using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartTorsionExpansionJointEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        public double AllowableAxialExpansion { get; set; }
        
        [JsonProperty(StartPropertyName.Weight)]
        public double Weight { get; set; }
        
        [JsonProperty(StartPropertyName.FrictionMoment)]
        public double FrictionMoment { get; set; }
        
        [JsonProperty(StartPropertyName.Length)]
        public double Length { get; set; }
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                { "Allowable Axial Expansion", AllowableAxialExpansion.ToString("F5") },
                { "Weight", Weight.ToString("F5") },
                { "Friction Moment", FrictionMoment.ToString("F5") },
                { "Length", Length.ToString("F5") },
                { "Name", Name },
            };
            
            return dictionary;
        }
    }
}