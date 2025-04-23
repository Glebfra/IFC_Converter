using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartLateralExpansionJointEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        public double AllowableAxialExpansion { get; set; }
        
        [JsonProperty(StartPropertyName.Weight)]
        public double Weight { get; set; }
        
        [JsonProperty(StartPropertyName.AxialFlexibility)]
        public double AxialFlexibility { get; set; }
        
        [JsonProperty(StartPropertyName.EffectiveArea)]
        public double EffectiveArea { get; set; }
        
        [JsonProperty(StartPropertyName.Length)]
        public double Length { get; set; }
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }
        
        [JsonProperty(StartPropertyName.EffectiveDiameter)]
        public double EffectiveDiameter { get; set; }
        
        [JsonProperty(StartPropertyName.StiffnessTempFactor)]
        public double StiffnessTempFactor { get; set; }
        
        [JsonProperty(StartPropertyName.AllowableCorrFactor)]
        public double AllowableCorrFactor { get; set; }
        
        [JsonProperty(StartPropertyName.AxialStiffness)]
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