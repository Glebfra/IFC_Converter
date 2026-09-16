using System.Linq;
using IFCConverter.Start.API;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Anchors
{
    public abstract class StartAbstractAnchorEntity : StartAbstractEntity,
        IStartAnchorEntity, IStartOneNodeEntity
    {
        [JsonProperty(StartPropertyName.Name)] public override string Name { get; set; } = string.Empty;

        [JsonIgnore] public FixedVector<Dim3> Position { get; set; } = default;

        [JsonIgnore] public IStartNodeEntity Node => ConnectedEntities.OfType<IStartNodeEntity>().First();
    }
}