using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Anchors
{
    public class StartSlidingSupportEntity : StartAbstractAnchorEntity
    {
        [JsonProperty(StartPropertyName.FrictionMoment)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty FrictionMoment { get; set; } = MomentProperty.Zero;
    }
}