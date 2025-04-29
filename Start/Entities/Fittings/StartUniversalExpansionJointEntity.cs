using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartUniversalExpansionJointEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        public double AllowableAxialExpansion { get; set; }

        [JsonProperty(StartPropertyName.AxialFlexibility)]
        public double AxialFlexibility { get; set; }
        
        [JsonProperty(StartPropertyName.EffectiveArea)]
        public double EffectiveArea { get; set; }
        
        [JsonProperty(StartPropertyName.Length)]
        public double Length { get; set; }
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }
        
        [JsonProperty(StartPropertyName.ShearCompliance)]
        public double ShearCompliance { get; set; }
        
        [JsonProperty(StartPropertyName.PermissibleLateralMovement)]
        public double PermissibleLateralMovement { get; set; }

        [JsonProperty(StartPropertyName.StiffnessTempFactor)]
        public double StiffnessTempFactor { get; set; }
        
        [JsonProperty(StartPropertyName.AllowableCorrFactor)]
        public double AllowableCorrFactor { get; set; }
        
        [JsonProperty(StartPropertyName.StiffnessAngleFactor)]
        public double StiffnessAngleFactor { get; set; }
        
        [JsonProperty(StartPropertyName.AngleAllowableCorrFactor)]
        public double AngleAllowableCorrFactor { get; set; }
        
        [JsonProperty(StartPropertyName.StiffnessShearFactor)]
        public double StiffnessShearFactor { get; set; }
        
        [JsonProperty(StartPropertyName.ShearAllowableCorrFactor)]
        public double ShearAllowableCorrFactor { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Allowable Axial Expansion", AllowableAxialExpansion.ToString("F5"));
            dictionary.Add("Axial Flexibility", AxialFlexibility.ToString("F5"));
            dictionary.Add("Effective Area", EffectiveArea.ToString("F5"));
            dictionary.Add("Length", Length.ToString("F5"));
            dictionary.Add("Name", Name);
            dictionary.Add("Stiffness Temp Factor", StiffnessTempFactor.ToString("F5"));
            dictionary.Add("Allowable Corr Factor", AllowableCorrFactor.ToString("F5"));
            dictionary.Add("Stiffness Angle Factor", StiffnessAngleFactor.ToString("F5"));
            dictionary.Add("Angle Allowable Corr Factor", AngleAllowableCorrFactor.ToString("F5"));
            dictionary.Add("Stiffness Shear Factor", StiffnessShearFactor.ToString("F5"));
            dictionary.Add("Shear Allowable Corr Factor", ShearAllowableCorrFactor.ToString("F5"));

            return dictionary;
        }
    }
}