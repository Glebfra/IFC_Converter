using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Converters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Anchors
{
    [StartElement(StartElementTypeEnum.ANCHOR)]
    public sealed class StartFixedAnchorEntity : StartAbstractAnchorEntity
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

        [JsonProperty(StartPropertyName.Mx)]
        [JsonConverter(typeof(JsonStartConverter<MomentValueProperty<double>>))]
        public IStartValueProperty<double> Mx { get; set; } = new MomentValueProperty<double>();

        [JsonProperty(StartPropertyName.My)]
        [JsonConverter(typeof(JsonStartConverter<MomentValueProperty<double>>))]
        public IStartValueProperty<double> My { get; set; } = new MomentValueProperty<double>();

        [JsonProperty(StartPropertyName.Mz)]
        [JsonConverter(typeof(JsonStartConverter<MomentValueProperty<double>>))]
        public IStartValueProperty<double> Mz { get; set; } = new MomentValueProperty<double>();
    }
}