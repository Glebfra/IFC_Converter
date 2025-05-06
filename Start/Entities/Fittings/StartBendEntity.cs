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
    }
}