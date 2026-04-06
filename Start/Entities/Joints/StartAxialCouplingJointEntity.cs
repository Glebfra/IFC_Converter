using Newtonsoft.Json;
using Start.API;
using Start.Attributes;
using Start.Converters;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities.Joints
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