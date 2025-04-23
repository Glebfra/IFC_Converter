using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartBallExpansionJointEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        public double AllowableAxialExpansion { get; set; }
        
        [JsonProperty(StartPropertyName.Weight)]
        public double Weight { get; set; }
        
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

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                { "Allowable Axial Expansion", AllowableAxialExpansion.ToString("F5") },
                { "Weight", Weight.ToString("F5") },
                { "Length", Length.ToString("F5") },
                { "Name", Name },
                { "Friction Moment 1", FrictionMoment1.ToString("F5") },
                { "Friction Moment 2", FrictionMoment2.ToString("F5") },
                { "Friction Moment 3", FrictionMoment3.ToString("F5") },
                { "Pressure 1", Pressure1.ToString("F5") },
                { "Pressure 2", Pressure2.ToString("F5") },
                { "Pressure 3", Pressure3.ToString("F5") },
            };

            return dictionary;
        }
    }
}