using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartReducerEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.ConicalPartLength)]
        public double LengthOfConicalPart { get; set; }
        
        [JsonProperty(StartPropertyName.Diameter)] 
        public double MaxDiameter { get; set; }
        
        [JsonProperty(StartPropertyName.MinDiameter)]
        public double MinDiameter { get; set; }
        
        [JsonProperty(StartPropertyName.WallThickness)] 
        public double ThicknessAtMaxDiameterPoint { get; set; }
        
        [JsonProperty(StartPropertyName.MillTolerance)]
        public double MillToleranceAtDMax { get; set; }

        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }
        
        [JsonProperty(StartPropertyName.AngleBetweenEccentricityVectorAndZmAxis)]
        public double AngleBetweenEccentricityVectorAndZmAxis { get; set; }
        
        [JsonProperty(StartPropertyName.PipeName)]
        public string Name { get; set; }
        
        [JsonProperty(StartPropertyName.MillToleranceAtDMin)]
        public double MillToleranceAtDMin { get; set; }
        
        [JsonProperty(StartPropertyName.ReducerMillTolerance)]
        public double MillTolerance { get; set; }
    }
}