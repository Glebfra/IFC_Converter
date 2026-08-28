using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Converters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Joints
{
    [StartElement(StartElementTypeEnum.BALL_EXPANSION_JOINT)]
    public sealed class StartBallExpansionJointEntity : StartAbstractExpansionJointEntity
    {
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> AllowableAxialExpansion { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.FrictionMoment1)]
        [JsonConverter(typeof(JsonStartConverter<MomentValueProperty<double>>))]
        public IStartValueProperty<double> FrictionMoment1 { get; set; } = new MomentValueProperty<double>();

        [JsonProperty(StartPropertyName.Pressure1)]
        [JsonConverter(typeof(JsonStartConverter<PressureValueProperty<double>>))]
        public IStartValueProperty<double> Pressure1 { get; set; } = new PressureValueProperty<double>();

        [JsonProperty(StartPropertyName.FrictionMoment2)]
        [JsonConverter(typeof(JsonStartConverter<MomentValueProperty<double>>))]
        public IStartValueProperty<double> FrictionMoment2 { get; set; } = new MomentValueProperty<double>();

        [JsonProperty(StartPropertyName.Pressure2)]
        [JsonConverter(typeof(JsonStartConverter<PressureValueProperty<double>>))]
        public IStartValueProperty<double> Pressure2 { get; set; } = new PressureValueProperty<double>();

        [JsonProperty(StartPropertyName.FrictionMoment3)]
        [JsonConverter(typeof(JsonStartConverter<MomentValueProperty<double>>))]
        public IStartValueProperty<double> FrictionMoment3 { get; set; } = new MomentValueProperty<double>();

        [JsonProperty(StartPropertyName.Pressure3)]
        [JsonConverter(typeof(JsonStartConverter<PressureValueProperty<double>>))]
        public IStartValueProperty<double> Pressure3 { get; set; } = new PressureValueProperty<double>();
    }
}