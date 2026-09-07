using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Converters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using IFCConverter.Utils.Mathematics;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Fittings
{
    public abstract class StartAbstractTeeEntity : StartAbstractFittingEntity,
        IStartOneNodeEntity, IStartFittingEntity, IStartMaterializedEntity, IStartClippingEntity
    {
        private readonly IStartSegmentEntity[] _mainSegments = new IStartSegmentEntity[2];
        private IStartSegmentEntity _headSegment;

        [JsonIgnore] public abstract double HeadLength { get; }

        [JsonIgnore] public abstract double MainLength { get; }

        [JsonIgnore] public double MainDiameter => MainSegments.Select(segment => segment.Diameter.SIProperty).Max();

        [JsonIgnore] public double HeadDiameter => HeadSegment.Diameter.SIProperty;

        [JsonIgnore]
        [StartIgnore]
        public IStartSegmentEntity HeadSegment
        {
            get
            {
                if (!(_headSegment is null))
                    return _headSegment;

                FilterSegments();
                return _headSegment ?? throw new InvalidOperationException("Head segment could not be determined");
            }
        }

        [JsonIgnore]
        [StartIgnore]
        public IEnumerable<IStartSegmentEntity> MainSegments
        {
            get
            {
                if (_mainSegments[0] != null && _mainSegments[1] != null)
                    return _mainSegments;

                FilterSegments();
                if (_mainSegments[0] == null || _mainSegments[1] == null)
                    throw new InvalidOperationException("Main segments could not be determined");

                return _mainSegments;
            }
        }

        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        [JsonConverter(typeof(JsonStartConverter<EnumProperty<StartManufacturingTechnologyEnum>>))]
        public IStartEnumProperty<StartManufacturingTechnologyEnum> ManufacturingTechnologyEnum { get; set; } =
            new EnumProperty<StartManufacturingTechnologyEnum>();

        [JsonProperty(StartPropertyName.WallThickness)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> HeaderThickness { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.MillTolerance)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> MillTolerance { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.HeaderLength)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> HeaderLength { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.BranchWallThickness)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> BranchWallThickness { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.MillToleranceForBranch)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> MillToleranceForBranch { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.BranchHeight)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> BranchHeight { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.PadThickness)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> PadThickness { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.PadWidth)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> PadWidth { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.CrotchHeight)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> CrotchHeight { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.CrotchThickness)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> CrotchThickness { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.LongitudinalWeldJointFactor)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> StrengthFactorOfLongitudinalWeldSeamOnPressure { get; set; } =
            new FactorValueProperty<double>();

        [JsonProperty(StartPropertyName.CrotchRadius)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> CrotchRadius { get; set; } = new LengthValueProperty<double>();

        public void ClipEntity(IStartClippableEntity clippable)
        {
            if (!(clippable is IStartSegmentEntity segmentEntity))
                throw new NotImplementedException("Clipping is only implemented for segments");

            if (IsAttachedToHeadSegment(segmentEntity))
                clippable.Clip(Position, HeadLength);
            else if (IsAttachedToMainSegment(segmentEntity))
                clippable.Clip(Position, MainLength / 2);
        }

        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;

        [Pure]
        private bool IsAttachedToHeadSegment(IStartSegmentEntity segmentEntity)
        {
            return HeadSegment.Equals(segmentEntity);
        }

        [Pure]
        private bool IsAttachedToMainSegment(IStartSegmentEntity segmentEntity)
        {
            return MainSegments.Any(segment => segment.Equals(segmentEntity));
        }

        private void FilterSegments()
        {
            IStartSegmentEntity[] startSegmentEntities = ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();

            for (int i = 0; i < 3; i++)
            for (int j = i + 1; j < 3; j++)
            {
                if (!startSegmentEntities[i].Projection.IsParallel(startSegmentEntities[j].Projection))
                    continue;

                _mainSegments[0] = startSegmentEntities[i];
                _mainSegments[1] = startSegmentEntities[j];
                _headSegment = startSegmentEntities[3 - (i + j)];
            }
        }
    }
}