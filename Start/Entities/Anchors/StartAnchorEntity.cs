using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Anchors
{
    public class StartAnchorEntity : StartAbstractAnchorEntity
    {
        [JsonProperty(StartPropertyName.Mx)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty Mx { get; set; } = MomentProperty.Zero;
        
        [JsonProperty(StartPropertyName.My)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty My { get; set; } = MomentProperty.Zero;
        
        [JsonProperty(StartPropertyName.Mz)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty Mz { get; set; } = MomentProperty.Zero;
    }
}