using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities
{
    public class StartNodeEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.AdditionalWeightLoad)] 
        public double AdditionalLoadFromWeight { get; set; }
        
        [JsonProperty(StartPropertyName.PipeName)] 
        public string Name { get; set; }
        
        [JsonProperty(StartPropertyName.Description)] 
        public string Description { get; set; }

        [JsonProperty(StartPropertyName.XCoord)]
        public double XCoord;
        
        [JsonProperty(StartPropertyName.YCoord)]
        public double YCoord;
        
        [JsonProperty(StartPropertyName.ZCoord)]
        public double ZCoord;

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                { "Name", Name },
                { "Description", Description },
                { "Additional Load from Weight", AdditionalLoadFromWeight.ToString("F5") },
                { "X Coordinate", XCoord.ToString("F5") },
                { "Y Coordinate", YCoord.ToString("F5") },
                { "Z Coordinate", ZCoord.ToString("F5") }
            };

            return dictionary;
        }
    }
}