using System.Collections.Generic;
using Newtonsoft.Json;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartBallExpansionJointEntity : StartAbstractEntity
    {
        [JsonProperty("2")]
        public double AllowableAxialExpansion { get; set; }
        
        [JsonProperty("34")]
        public double Weight { get; set; }
        
        [JsonProperty("145")]
        public double Length { get; set; }
        
        [JsonProperty("449")]
        public string Name { get; set; }
        
        [JsonProperty("1094")]
        public double FrictionMoment1 { get; set; }
        
        [JsonProperty("1095")]
        public double Pressure1 { get; set; }
        
        [JsonProperty("1096")]
        public double FrictionMoment2 { get; set; }
        
        [JsonProperty("1097")]
        public double Pressure2 { get; set; }
        
        [JsonProperty("1098")]
        public double FrictionMoment3 { get; set; }
        
        [JsonProperty("1099")]
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