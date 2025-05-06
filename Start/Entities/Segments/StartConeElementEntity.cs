using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;

namespace Start.Entities.Segments
{
    public class StartConeElementEntity : StartPipeEntity
    {
        [JsonProperty(StartPropertyName.ConeElementSecondDiameter)]
        public double SecondDiameter { get; set; }
    }
}