using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using Start.API;
using Start.Converters;
using Start.Extensions;
using Start.Interfaces;
using Start.StartProperties;
using Utils;

namespace Start.Entities.Fittings
{
    public abstract class StartAbstractBendEntity : StartAbstractFittingEntity,
        IStartOneNodeEntity, IStartFittingEntity, IStartMaterializedEntity, IStartClippingEntity
    {
        [JsonProperty(StartPropertyName.WallThickness)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> WallThickness { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.MillTolerance)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> MillTolerance { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        [JsonConverter(typeof(JsonStartConverter<EnumProperty<StartManufacturingTechnologyEnum>>))]
        public IStartEnumProperty<StartManufacturingTechnologyEnum> ManufacturingTechnologyEnum { get; set; } =
            new EnumProperty<StartManufacturingTechnologyEnum>();

        [JsonProperty(StartPropertyName.Radius)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> Radius { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.OvalizationCoefficient)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> OvalizationCoefficient { get; set; } = new FactorValueProperty<double>();

        [JsonProperty(StartPropertyName.NumberOfMilters)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<int>>))]
        public IStartValueProperty<int> NumberOfMilters { get; set; } = new FactorValueProperty<int>();

        [JsonProperty(StartPropertyName.MillToleranceOutside)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> MillToleranceOutside { get; set; } = new LengthValueProperty<double>();

        public void ClipEntity(IStartClippableEntity clippable)
        {
            clippable.Clip(Position, GetClipLength());
        }

        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;

        private double GetClipLength()
        {
            IStartSegmentEntity[] segmentEntities = ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            Vector<double> firstDir = segmentEntities[0].GetProjectionFromPoint(Position).Negate();
            Vector<double> secondDir = segmentEntities[1].GetProjectionFromPoint(Position);
            double angle = firstDir.Angle(secondDir);
            return MathExtensions.CalculateTorusSegmentLength(Radius.SIProperty, angle);
        }
    }
}