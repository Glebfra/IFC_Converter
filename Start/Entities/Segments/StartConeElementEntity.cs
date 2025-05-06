using Newtonsoft.Json;
using Start.API;
using Start.StartProperties;

namespace Start.Entities.Segments
{
    public class StartConeElementEntity : StartPipeEntity
    {
        [JsonProperty(StartPropertyName.ConeElementSecondDiameter)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty SecondDiameter { get; set; } = LengthProperty.Zero;
    }
}