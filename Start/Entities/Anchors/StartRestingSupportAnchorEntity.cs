using Newtonsoft.Json;
using Start.API;
using Start.Attributes;
using Start.Converters;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities.Anchors
{
    [StartElement(StartElementTypeEnum.RESTING_SUPPORT)]
    public sealed class StartRestingSupportAnchorEntity : StartAbstractAnchorEntity
    {
        [JsonProperty(StartPropertyName.Fx)]
        [JsonConverter(typeof(JsonStartConverter<ForceValueProperty<double>>))]
        public IStartValueProperty<double> Fx { get; set; } = new ForceValueProperty<double>();

        [JsonProperty(StartPropertyName.Fy)]
        [JsonConverter(typeof(JsonStartConverter<ForceValueProperty<double>>))]
        public IStartValueProperty<double> Fy { get; set; } = new ForceValueProperty<double>();

        [JsonProperty(StartPropertyName.Fz)]
        [JsonConverter(typeof(JsonStartConverter<ForceValueProperty<double>>))]
        public IStartValueProperty<double> Fz { get; set; } = new ForceValueProperty<double>();

        [JsonProperty(StartPropertyName.FrictionMoment)]
        [JsonConverter(typeof(JsonStartConverter<MomentValueProperty<double>>))]
        public IStartValueProperty<double> FrictionMoment { get; set; } = new MomentValueProperty<double>();
    }
}