using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartBendEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.WallThickness)]
        public double WallThickness { get; set; }
    
        [JsonProperty(StartPropertyName.MillTolerance)]
        public double MillTolerance { get; set; }
    
        [JsonProperty(StartPropertyName.Weight)]
        public double Weight { get; set; }
    
        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }
    
        [JsonProperty(StartPropertyName.Radius)]
        public double Radius { get; set; }
    
        [JsonProperty(StartPropertyName.OvalizationCoefficient)]
        public double OvalizationCoefficient { get; set; }
    
        [JsonProperty(StartPropertyName.NumberOfMilters)]
        public int NumberOfMilters { get; set; }
    
        [JsonProperty(StartPropertyName.MillToleranceOutside)]
        public double MillToleranceOutside { get; set; }
    
        [JsonProperty(StartPropertyName.PipeName)]
        public string Name { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new()
            {
                { "Wall Thickness", WallThickness.ToString("F5") },
                { "Mill Tolerance", MillTolerance.ToString("F5") },
                { "Weight", Weight.ToString("F5") },
                { "Manufacturing Technology", ManufacturingTechnologyEnum.ToString() },
                { "Radius", Radius.ToString("F5") },
                { "Ovalization Coefficient", OvalizationCoefficient.ToString("F5") },
                { "Number Of Milters", NumberOfMilters.ToString() },
                { "Mill Tolerance Outside", MillToleranceOutside.ToString("F5") },
                { "Name", Name }
            };

            return dictionary;
        }
    }
}