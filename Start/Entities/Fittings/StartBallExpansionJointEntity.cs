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

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Allowable Axial Expansion", AllowableAxialExpansion.ToString("F5"));
            dictionary.Add("Length", Length.ToString("F5"));
            dictionary.Add("Name", Name);
            dictionary.Add("Friction Moment 1", FrictionMoment1.ToString("F5"));
            dictionary.Add("Friction Moment 2", FrictionMoment2.ToString("F5"));
            dictionary.Add("Friction Moment 3", FrictionMoment3.ToString("F5"));
            dictionary.Add("Pressure 1", Pressure1.ToString("F5"));
            dictionary.Add("Pressure 2", Pressure2.ToString("F5"));
            dictionary.Add("Pressure 3", Pressure3.ToString("F5"));

            return dictionary;
        }
    }
}