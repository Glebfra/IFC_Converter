using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Converters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Joints
{
    [StartElement(StartElementTypeEnum.AXIAL_COUPLING_JOINT)]
    public sealed class StartAxialCouplingJointEntity : StartAbstractExpansionJointEntity
    {
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> AllowableAxialExpansion { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.FrictionForce)]
        [JsonConverter(typeof(JsonStartConverter<ForceValueProperty<double>>))]
        public IStartValueProperty<double> FrictionForce { get; set; } = new ForceValueProperty<double>();
    }
}