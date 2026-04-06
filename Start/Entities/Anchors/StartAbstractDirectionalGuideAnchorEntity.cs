using Newtonsoft.Json;
using Start.API;
using Start.Converters;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities.Anchors
{
    public abstract class StartAbstractDirectionalGuideAnchorEntity : StartAbstractAnchorEntity
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
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> FrictionFactor { get; set; } = new FactorValueProperty<double>();
    }
}