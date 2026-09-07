using System.Linq;
using IFCConverter.Start.API;
using IFCConverter.Start.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Anchors
{
    public abstract class StartAbstractAnchorEntity : StartAbstractEntity,
        IStartAnchorEntity, IStartOneNodeEntity
    {
        [JsonProperty(StartPropertyName.Name)] public override string Name { get; set; } = string.Empty;

        [JsonIgnore] public Vector<double> Position { get; set; } = default;

        [JsonIgnore] public IStartNodeEntity Node => ConnectedEntities.OfType<IStartNodeEntity>().First();
    }
}