using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using Start.API;
using Start.Attributes;
using Start.Converters;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public abstract class StartAbstractFittingEntity : StartAbstractEntity, IStartFittingEntity, IStartOneNodeEntity
    {
        [JsonProperty(StartPropertyName.Weight)]
        [JsonConverter(typeof(JsonStartConverter<MassValueProperty<double>>))]
        public IStartValueProperty<double> Weight { get; set; } = new MassValueProperty<double>();

        [JsonIgnore] public Vector<double> Position { get; set; } = default!;

        [JsonIgnore] [StartIgnore] public IStartNodeEntity Node => ConnectedEntities.OfType<IStartNodeEntity>().First();

        [JsonProperty(StartPropertyName.Name)] public override string Name { get; set; } = string.Empty;
    }
}