using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Anchors
{
    public class StartGuideDoubleDirectionSupportEntity : StartAbstractAnchorEntity
    {
        [JsonProperty(StartPropertyName.AnchorSupportWeight)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassProperty, double>))]
        public MassProperty Weight { get; set; } = MassProperty.Zero;
    }
}