using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    [StartEntityType(StartElementType.TORSION_EXPANSION_JOINT)]
    public class StartTorsionExpansionJointEntity : StartAbstractEntity
    {
        [JsonProperty("2")]
        public double AllowableAxialExpansion { get; set; }
        
        [JsonProperty("34")]
        public double Weight { get; set; }
        
        [JsonProperty("72")]
        public double FrictionMoment { get; set; }
        
        [JsonProperty("145")]
        public double Length { get; set; }
        
        [JsonProperty("449")]
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