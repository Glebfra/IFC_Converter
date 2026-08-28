using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Converters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Joints
{
    [StartElement(StartElementTypeEnum.LATERAL_EXPANSION_JOINT)]
    public sealed class StartLateralExpansionJointEntity : StartAbstractExpansionJointEntity
    {
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> AllowableAxialExpansion { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.AxialFlexibility)]
        [JsonConverter(typeof(JsonStartConverter<FlexibilityValueProperty<double>>))]
        public IStartValueProperty<double> AxialFlexibility { get; set; } = new FlexibilityValueProperty<double>();

        [JsonProperty(StartPropertyName.EffectiveArea)]
        [JsonConverter(typeof(JsonStartConverter<AreaValueProperty<double>>))]
        public IStartValueProperty<double> EffectiveArea { get; set; } = new AreaValueProperty<double>();

        [JsonProperty(StartPropertyName.EffectiveDiameter)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> EffectiveDiameter { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.StiffnessTempFactor)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> StiffnessTempFactor { get; set; } = new FactorValueProperty<double>();

        [JsonProperty(StartPropertyName.AllowableCorrFactor)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> AllowableCorrFactor { get; set; } = new FactorValueProperty<double>();

        //TODO get measurements
        [JsonProperty(StartPropertyName.AxialStiffness)]
        public double AxialStiffness { get; set; }
    }
}