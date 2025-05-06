using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Anchors
{
    public class StartConstantForceSupportEntity : StartAbstractAnchorEntity
    {
        [JsonProperty(StartPropertyName.FrictionMoment)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty FrictionMoment { get; set; } = MomentProperty.Zero;

        [JsonProperty(StartPropertyName.SupportsNumber)]
        [JsonConverter(typeof(StartPropertyJsonConverter<NumberProperty, int>))]
        public NumberProperty SupportsNumber { get; set; } = NumberProperty.Zero;

        [JsonProperty(StartPropertyName.AnchorSupportWeight)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassProperty, double>))]
        public MassProperty Weight { get; set; } = MassProperty.Zero;
    }
}