using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;

namespace Start.Entities
{
    [StartEntityType(StartElementType.AXIAL_EXPANSION_JOINT, StartElementType.AXIAL_EXPANSION_SLIP_JOINT)]
    public class StartAxialExpansionJointEntity : StartAbstractEntity
    {
        [JsonProperty("2")]
        public double AllowableAxialExpansion { get; set; }
        
        [JsonProperty("34")]
        public double Weight { get; set; }
        
        [JsonProperty("75")]
        public double AxialFlexibility { get; set; }
        
        [JsonProperty("76")]
        public double EffectiveArea { get; set; }
        
        [JsonProperty("145")]
        public double Length { get; set; }
        
        [JsonProperty("449")]
        public string Name { get; set; }
        
        [JsonProperty("1289")]
        public double EffectiveDiameter { get; set; }
        
        [JsonProperty("1293")]
        public double StiffnessTempFactor { get; set; }
        
        [JsonProperty("1294")]
        public double AllowableCorrFactor { get; set; }
        
        [JsonProperty("1405")]
        public double AxialStiffness { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                { "Allowable Axial Expansion", AllowableAxialExpansion.ToString("F5") },
                { "Weight", Weight.ToString("F5") },
                { "Axial Flexibility", AxialFlexibility.ToString("F5") },
                { "Effective Area", EffectiveArea.ToString("F5") },
                { "Length", Length.ToString("F5") },
                { "Name", Name },
                { "Effective Diameter", EffectiveDiameter.ToString("F5") },
                { "Stiffness Temp Factor", StiffnessTempFactor.ToString("F5") },
                { "Allowable Corr Factor", AllowableCorrFactor.ToString("F5") },
                { "Axial Stiffness", AxialStiffness.ToString("F5") },
            };

            return dictionary;
        }
    }
}