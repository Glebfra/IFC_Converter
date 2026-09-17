using System.Linq;
using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Converters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using IFCConverter.Utils.Mathematics;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Joints
{
    public abstract class StartAbstractExpansionJointEntity : StartAbstractEntity, IStartFittingEntity
    {
        [JsonProperty(StartPropertyName.Length)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public virtual IStartValueProperty<double> Length { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.Name)] public override string Name { get; set; } = string.Empty;

        [JsonIgnore] public FixedVector<Dim3> Position { get; set; } = default;

        [JsonIgnore] [StartIgnore] public IStartNodeEntity Node => ConnectedEntities.OfType<IStartNodeEntity>().First();
    }
}