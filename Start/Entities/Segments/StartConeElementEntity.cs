using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;

namespace Start.Entities.Segments
{
    public class StartConeElementEntity : StartPipeEntity
    {
        [JsonProperty(StartPropertyName.ConeElementSecondDiameter)]
        public double SecondDiameter { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Second Diameter", SecondDiameter.ToString("F5"));

            return dictionary;
        }
    }
}