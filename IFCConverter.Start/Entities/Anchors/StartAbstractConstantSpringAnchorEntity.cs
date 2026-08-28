using IFCConverter.Start.API;
using IFCConverter.Start.Converters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Anchors
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