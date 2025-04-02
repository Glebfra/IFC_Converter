using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartTeeEntity : StartAbstractEntity
    {
        [JsonProperty("9")] 
        public double HeaderThickness { get; set; }
        
        [JsonProperty("13")] 
        public double MillTolerance { get; set; }
        
        [JsonProperty("16")] 
        public double HeaderLength { get; set; }
        
        [JsonProperty("18")]
        public double BranchWallThickness { get; set; }
        
        [JsonProperty("20")] 
        public double MillToleranceForBranch { get; set; }
        
        [JsonProperty("21")] 
        public double BranchHeight { get; set; }
        
        [JsonProperty("22")] 
        public double PadThickness { get; set; }
        
        [JsonProperty("23")] 
        public double PadWidth { get; set; }
        
        [JsonProperty("24")]
        public double CrotchHeight { get; set; }
        
        [JsonProperty("25")]
        public double CrotchThickness { get; set; }
        
        [JsonProperty("34")] 
        public double Weight { get; set; }
        
        [JsonProperty("38")] 
        public double StrengthFactorOfLongitudinalWeldSeamOnPressure { get; set; }
        
        [JsonProperty("174")]
        public double CrotchRadius { get; set; }
        
        [JsonProperty("225")]
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