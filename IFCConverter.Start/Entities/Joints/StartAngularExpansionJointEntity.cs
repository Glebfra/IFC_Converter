using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Converters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Joints
{
    [StartElement(StartElementTypeEnum.ANGULAR_EXPANSION_JOINT)]
    public sealed class StartAngularExpansionJointEntity : StartAbstractExpansionJointEntity
    {
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> AllowableAxialExpansion { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.AxialFlexibility)]
        [JsonConverter(typeof(JsonStartConverter<FlexibilityValueProperty<double>>))]
        public IStartValueProperty<double> AxialFlexibility { get; set; } = new FlexibilityValueProperty<double>();

        [JsonProperty(StartPropertyName.StiffnessTempFactor)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> StiffnessTempFactor { get; set; } = new FactorValueProperty<double>();

        [JsonProperty(StartPropertyName.AllowableCorrFactor)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> AllowableCorrFactor { get; set; } = new FactorValueProperty<double>();

        [JsonProperty(StartPropertyName.AxialStiffness)]
        [JsonConverter(typeof(JsonStartConverter<StiffnessValueProperty<double>>))]
        public IStartValueProperty<double> AxialStiffness { get; set; } = new StiffnessValueProperty<double>();

        [JsonProperty(StartPropertyName.Weight)]
        [JsonConverter(typeof(JsonStartConverter<MassValueProperty<double>>))]
        public IStartValueProperty<double> Weight { get; set; } = new MassValueProperty<double>();

        [JsonProperty(StartPropertyName.ExpansionJointType)]
        [JsonConverter(typeof(JsonStartConverter<EnumProperty<ExpansionJointTypeEnum>>))]
        public IStartEnumProperty<ExpansionJointTypeEnum> ExpansionJointType { get; set; } =
            new EnumProperty<ExpansionJointTypeEnum>();
    }
}