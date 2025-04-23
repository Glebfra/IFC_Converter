using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartTeeEntity : StartAbstractEntity
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
        
        [JsonProperty(StartPropertyName.Weight)] 
        public double Weight { get; set; }
        
        [JsonProperty(StartPropertyName.LongitudinalWeldJointFactor)] 
        public double StrengthFactorOfLongitudinalWeldSeamOnPressure { get; set; }
        
        [JsonProperty(StartPropertyName.CrotchRadius)]
        public double CrotchRadius { get; set; }
        
        [JsonProperty(StartPropertyName.PipeName)]
        public string Name { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "Name", Name },
                { "Header Thickness", HeaderThickness.ToString("F5") },
                { "Mill Tolerance", MillTolerance.ToString("F5") },
                { "Header Length", HeaderLength.ToString("F5") },
                { "Branch Height", BranchHeight.ToString("F5") },
                { "Weight", Weight.ToString("F5") },
                { "Strength Factor Of Longitudinal Weld Seam On Pressure", StrengthFactorOfLongitudinalWeldSeamOnPressure.ToString("F5") },
                { "Branch Wall Thickness", BranchWallThickness.ToString("F5") },
                { "Mill Tolerance For Branch", MillToleranceForBranch.ToString("F5") },
                { "Pad Thickness", PadThickness.ToString("F5") },
                { "Pad Width", PadWidth.ToString("F5") },
                { "Crotch Radius", CrotchRadius.ToString("F5") },
                { "Crotch Thickness", CrotchThickness.ToString("F5") },
                { "Crotch Height", CrotchHeight.ToString("F5") },
            };

            return data;
        }
    }
}