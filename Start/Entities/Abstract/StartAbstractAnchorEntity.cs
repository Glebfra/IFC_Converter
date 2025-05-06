using Newtonsoft.Json;
using Start.API;
using Start.StartProperties;

namespace Start.Entities.Abstract
{
    public class StartAbstractAnchorEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.CheckAllowableLoads)]
        [JsonConverter(typeof(StartPropertyJsonConverter<NumberProperty, int>))]
        public NumberProperty CheckAllowableLoads { get; set; } = NumberProperty.Zero;

        [JsonProperty(StartPropertyName.AllowableLoadsInLocalAxes)]
        [JsonConverter(typeof(StartPropertyJsonConverter<NumberProperty, int>))]
        public NumberProperty AllowableLoadsInLocalAxes { get; set; } = NumberProperty.Zero;

        [JsonProperty(StartPropertyName.Fx)]
        [JsonConverter(typeof(StartPropertyJsonConverter<ForceProperty, double>))]
        public ForceProperty Fx { get; set; } = ForceProperty.Zero;

        [JsonProperty(StartPropertyName.Fy)]
        [JsonConverter(typeof(StartPropertyJsonConverter<ForceProperty, double>))]
        public ForceProperty Fy { get; set; } = ForceProperty.Zero;

        [JsonProperty(StartPropertyName.Fz)]
        [JsonConverter(typeof(StartPropertyJsonConverter<ForceProperty, double>))]
        public ForceProperty Fz { get; set; } = ForceProperty.Zero;
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; } = string.Empty;
    }
}