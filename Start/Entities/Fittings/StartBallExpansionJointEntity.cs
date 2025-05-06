using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartBallExpansionJointEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        public double AllowableAxialExpansion { get; set; }

        [JsonProperty(StartPropertyName.Length)]
        public double Length { get; set; }
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }
        
        [JsonProperty(StartPropertyName.FrictionMoment1)]
        public double FrictionMoment1 { get; set; }
        
        [JsonProperty(StartPropertyName.Pressure1)]
        public double Pressure1 { get; set; }
        
        [JsonProperty(StartPropertyName.FrictionMoment2)]
        public double FrictionMoment2 { get; set; }
        
        [JsonProperty(StartPropertyName.Pressure2)]
        public double Pressure2 { get; set; }
        
        [JsonProperty(StartPropertyName.FrictionMoment3)]
        public double FrictionMoment3 { get; set; }
        
        [JsonProperty(StartPropertyName.Pressure3)]
        public double Pressure3 { get; set; }
    }
}