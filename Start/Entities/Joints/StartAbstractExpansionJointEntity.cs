using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using Start.API;
using Start.Attributes;
using Start.Converters;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities.Joints
{
    public abstract class StartAbstractExpansionJointEntity : StartAbstractEntity,
        IStartOneNodeEntity, IStartClippingEntity
    {
        [JsonProperty(StartPropertyName.Length)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public virtual IStartValueProperty<double> Length { get; set; } = new LengthValueProperty<double>();

        public void ClipEntity(IStartClippableEntity clippable)
        {
            clippable.Clip(Position, Length.SIProperty / 2);
        }

        [JsonProperty(StartPropertyName.Name)] public override string Name { get; set; } = string.Empty;

        [JsonIgnore] public Vector<double> Position { get; set; } = default!;

        [JsonIgnore] [StartIgnore] public IStartNodeEntity Node => ConnectedEntities.OfType<IStartNodeEntity>().First();
    }
}