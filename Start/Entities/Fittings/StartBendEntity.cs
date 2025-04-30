using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartBendEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.WallThickness)]
        public double WallThickness { get; set; }
    
        [JsonProperty(StartPropertyName.MillTolerance)]
        public double MillTolerance { get; set; }

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
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Wall Thickness", WallThickness.ToString("F5"));
            dictionary.Add("Mill Tolerance", MillTolerance.ToString("F5"));
            dictionary.Add("Manufacturing Technology", ManufacturingTechnologyEnum.ToString());
            dictionary.Add("Radius", Radius.ToString("F5"));
            dictionary.Add("Ovalization Coefficient", OvalizationCoefficient.ToString("F5"));
            dictionary.Add("Number Of Milters", NumberOfMilters.ToString());
            dictionary.Add("Mill Tolerance Outside", MillToleranceOutside.ToString("F5"));
            dictionary.Add("Name", Name);

            return dictionary;
        }
    }
}