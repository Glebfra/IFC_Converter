using System.Collections.Generic;
using Newtonsoft.Json;

namespace Start.Entities
{
    public class StartNodeEntity : IStartEntity
    {
        [JsonProperty("61")] 
        public double AdditionalLoadFromWeight { get; set; }
        
        [JsonProperty("225")] 
        public string Name { get; set; }
        
        [JsonProperty("227")] 
        public string Description { get; set; }

        public double XCoord;
        public double YCoord;
        public double ZCoord;

        public Dictionary<string, string> GetData()
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