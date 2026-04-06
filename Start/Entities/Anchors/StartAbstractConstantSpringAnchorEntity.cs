using Newtonsoft.Json;
using Start.API;
using Start.Converters;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities.Anchors
{
    public abstract class StartAbstractConstantSpringAnchorEntity : StartAbstractAnchorEntity
    {
        [JsonProperty(StartPropertyName.FrictionMoment)]
        [JsonConverter(typeof(JsonStartConverter<MomentValueProperty<double>>))]
        public IStartValueProperty<double> FrictionMoment { get; set; } = new MomentValueProperty<double>();

        [JsonProperty(StartPropertyName.SupportsNumber)]
        public int SupportsNumber { get; set; }
    }
}