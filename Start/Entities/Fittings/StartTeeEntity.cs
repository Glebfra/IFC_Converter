using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartTeeEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.WallThickness)] 
        public double HeaderThickness { get; set; }
        
        [JsonProperty(StartPropertyName.MillTolerance)] 
        public double MillTolerance { get; set; }
        
        [JsonProperty(StartPropertyName.HeaderLength)] 
        public double HeaderLength { get; set; }
        
        [JsonProperty(StartPropertyName.BranchWallThickness)]
        public double BranchWallThickness { get; set; }
        
        [JsonProperty(StartPropertyName.MillToleranceForBranch)] 
        public double MillToleranceForBranch { get; set; }
        
        [JsonProperty(StartPropertyName.BranchHeight)] 
        public double BranchHeight { get; set; }
        
        [JsonProperty(StartPropertyName.PadThickness)] 
        public double PadThickness { get; set; }
        
        [JsonProperty(StartPropertyName.PadWidth)] 
        public double PadWidth { get; set; }
        
        [JsonProperty(StartPropertyName.CrotchHeight)]
        public double CrotchHeight { get; set; }
        
        [JsonProperty(StartPropertyName.CrotchThickness)]
        public double CrotchThickness { get; set; }

        [JsonProperty(StartPropertyName.LongitudinalWeldJointFactor)] 
        public double StrengthFactorOfLongitudinalWeldSeamOnPressure { get; set; }
        
        [JsonProperty(StartPropertyName.CrotchRadius)]
        public double CrotchRadius { get; set; }
        
        [JsonProperty(StartPropertyName.PipeName)]
        public string Name { get; set; }
    }
}