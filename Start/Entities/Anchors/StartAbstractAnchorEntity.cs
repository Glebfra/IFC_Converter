using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using Start.API;
using Start.Interfaces;

namespace Start.Entities.Anchors
{
    public abstract class StartAbstractAnchorEntity : StartAbstractEntity,
        IStartAnchorEntity, IStartOneNodeEntity
    {
        [JsonProperty(StartPropertyName.Name)] public override string Name { get; set; } = string.Empty;

        [JsonIgnore] public Vector<double> Position { get; set; } = default!;

        [JsonIgnore] public IStartNodeEntity Node => ConnectedEntities.OfType<IStartNodeEntity>().First();
    }
}