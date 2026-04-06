using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using Start.API;
using Start.Attributes;
using Start.Converters;
using Start.Extensions;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public abstract class StartAbstractReducerEntity : StartAbstractFittingEntity,
        IStartOneNodeEntity, IStartClippingEntity, IStartConeEntity, IStartMaterializedEntity
    {
        [JsonProperty(StartPropertyName.ConicalPartLength)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> LengthOfConicalPart { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.WallThickness)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> ThicknessAtMaxDiameterPoint { get; set; } =
            new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.MillTolerance)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> MillToleranceAtDMax { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        [JsonConverter(typeof(JsonStartConverter<EnumProperty<StartManufacturingTechnologyEnum>>))]
        public IStartEnumProperty<StartManufacturingTechnologyEnum> ManufacturingTechnologyEnum { get; set; } =
            new EnumProperty<StartManufacturingTechnologyEnum>();

        [JsonProperty(StartPropertyName.AngleBetweenEccentricityVectorAndZmAxis)]
        [JsonConverter(typeof(JsonStartConverter<AngleValueProperty<double>>))]
        public IStartValueProperty<double> AngleBetweenEccentricityVectorAndZmAxis { get; set; } =
            new AngleValueProperty<double>();

        [JsonProperty(StartPropertyName.MillToleranceAtDMin)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> MillToleranceAtDMin { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.ReducerMillTolerance)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> MillTolerance { get; set; } = new LengthValueProperty<double>();

        [JsonIgnore] public IStartValueProperty<double> MaxDiameter => SegmentWithMaxDiameter.Diameter;

        [JsonIgnore] public IStartValueProperty<double> MinDiameter => SegmentWithMinDiameter.Diameter;

        [JsonIgnore]
        [StartIgnore]
        public IStartSegmentEntity SegmentWithMinDiameter =>
            ConnectedEntities.OfType<IStartSegmentEntity>().OrderBy(segment => segment.Diameter).First();

        [JsonIgnore]
        [StartIgnore]
        public IStartSegmentEntity SegmentWithMaxDiameter =>
            ConnectedEntities.OfType<IStartSegmentEntity>().OrderByDescending(segment => segment.Diameter).First();

        public void ClipEntity(IStartClippableEntity clippable)
        {
            if (clippable.Equals(SegmentWithMaxDiameter) &&
                SegmentWithMaxDiameter is IStartClippableEntity clippableEntity
               )
                clippableEntity.Clip(Position, LengthOfConicalPart.SIProperty);
        }

        [JsonIgnore]
        [StartIgnore]
        public IEnumerable<Vector<double>> Points => new[]
        {
            SegmentWithMinDiameter.GetNearestPosition(Position),
            SegmentWithMaxDiameter.GetNearestPosition(Position)
        };

        [JsonIgnore]
        [StartIgnore]
        public IEnumerable<double> Diameters => new[] { MinDiameter.SIProperty, MaxDiameter.SIProperty };

        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
    }
}