using Newtonsoft.Json;
using Start.API;
using Start.StartProperties;

namespace Start.Entities.Abstract
{
    public abstract class StartAbstractFittingEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; } = string.Empty;
        
        [JsonProperty(StartPropertyName.Weight)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassProperty, double>))]
        public MassProperty Weight { get; set; } = MassProperty.Zero;
    }
}