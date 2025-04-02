using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    [StartEntityType(StartElementType.UNIVERSAL_EXPANSION_JOINT)]
    public class StartUniversalExpansionJointEntity : StartAbstractEntity
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
        
        [JsonProperty("693")]
        public double ShearCompliance { get; set; }
        
        [JsonProperty("694")]
        public double PermissibleLateralMovement { get; set; }

        [JsonProperty("1293")]
        public double StiffnessTempFactor { get; set; }
        
        [JsonProperty("1294")]
        public double AllowableCorrFactor { get; set; }
        
        [JsonProperty("1416")]
        public double StiffnessAngleFactor { get; set; }
        
        [JsonProperty("1417")]
        public double AngleAllowableCorrFactor { get; set; }
        
        [JsonProperty("1418")]
        public double StiffnessShearFactor { get; set; }
        
        [JsonProperty("1419")]
        public double ShearAllowableCorrFactor { get; set; }

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
                { "Stiffness Temp Factor", StiffnessTempFactor.ToString("F5") },
                { "Allowable Corr Factor", AllowableCorrFactor.ToString("F5") },
                { "Stiffness Angle Factor", StiffnessAngleFactor.ToString("F5") },
                { "Angle Allowable Corr Factor", AngleAllowableCorrFactor.ToString("F5") },
                { "Stiffness Shear Factor", StiffnessShearFactor.ToString("F5") },
                { "Shear Allowable Corr Factor", ShearAllowableCorrFactor.ToString("F5") },
            };

            return dictionary;
        }
    }
}