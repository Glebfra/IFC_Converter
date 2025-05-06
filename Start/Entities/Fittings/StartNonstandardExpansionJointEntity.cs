using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public class StartNonstandardExpansionJointEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.EffectiveArea)]
        [JsonConverter(typeof(StartPropertyJsonConverter<AreaProperty, double>))]
        public AreaProperty EffectiveArea { get; set; } = AreaProperty.Zero;
        
        [JsonProperty(StartPropertyName.Length)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty Length { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; } = string.Empty;
    }
}