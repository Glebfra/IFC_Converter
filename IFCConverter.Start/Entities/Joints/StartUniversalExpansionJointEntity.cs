using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Converters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Joints
{
    [StartElement(StartElementTypeEnum.UNIVERSAL_EXPANSION_JOINT)]
    public sealed class StartUniversalExpansionJointEntity : StartAbstractExpansionJointEntity
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

        //TODO get measurements
        [JsonProperty(StartPropertyName.ShearCompliance)]
        public double ShearCompliance { get; set; }

        //TODO get measurements
        [JsonProperty(StartPropertyName.PermissibleLateralMovement)]
        public double PermissibleLateralMovement { get; set; }

        [JsonProperty(StartPropertyName.StiffnessTempFactor)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> StiffnessTempFactor { get; set; } = new FactorValueProperty<double>();

        [JsonProperty(StartPropertyName.AllowableCorrFactor)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> AllowableCorrFactor { get; set; } = new FactorValueProperty<double>();

        [JsonProperty(StartPropertyName.StiffnessAngleFactor)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> StiffnessAngleFactor { get; set; } = new FactorValueProperty<double>();

        [JsonProperty(StartPropertyName.AngleAllowableCorrFactor)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> AngleAllowableCorrFactor { get; set; } = new FactorValueProperty<double>();

        [JsonProperty(StartPropertyName.StiffnessShearFactor)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> StiffnessShearFactor { get; set; } = new FactorValueProperty<double>();

        [JsonProperty(StartPropertyName.ShearAllowableCorrFactor)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> ShearAllowableCorrFactor { get; set; } = new FactorValueProperty<double>();
    }
}