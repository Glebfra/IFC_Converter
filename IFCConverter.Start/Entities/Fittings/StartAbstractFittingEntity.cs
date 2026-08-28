using System.Linq;
using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Converters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Fittings
{
    public abstract class StartAbstractFittingEntity : StartAbstractEntity, IStartFittingEntity, IStartOneNodeEntity
    {
        [JsonProperty(StartPropertyName.Weight)]
        [JsonConverter(typeof(JsonStartConverter<MassValueProperty<double>>))]
        public IStartValueProperty<double> Weight { get; set; } = new MassValueProperty<double>();

        [JsonIgnore] public Vector<double> Position { get; set; } = default;

        [JsonIgnore] [StartIgnore] public IStartNodeEntity Node => ConnectedEntities.OfType<IStartNodeEntity>().First();

        [JsonProperty(StartPropertyName.Name)] public override string Name { get; set; } = string.Empty;
    }
}