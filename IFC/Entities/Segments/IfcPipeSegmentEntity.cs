using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IFC.Entities.Abstract.Segments;
using IFC.EntitiesExtensions;
using IFC.Exceptions;
using IFC.Extensions;
using IFC.Tools;
using Start.API;
using Start.Entities.Segments;
using Start.StartProperties;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Segments
{
    public sealed class IfcPipeSegmentEntity : IfcAbstractPipeSegmentEntity
    {
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public override Colour Colour { get; protected set; } = Colour.FromHEX("bebebe");
        public override double Length { get; protected set; }
        public override double Diameter { get; protected set; }
        public override ActionProperty<double> RealLength { get; protected set; }
        public override ActionProperty<double> OuterSurfaceArea { get; protected set; }
        public override ActionProperty<XbimVector3D> Coordinates { get; protected set; }
        public override XbimVector3D Direction { get; }

        public IfcPipeSegmentEntity(StartPipeEntity pipeEntity, IfcNodeEntity[] nodeEntities) 
            : base(pipeEntity, nodeEntities)
        {
            Coordinates = new ActionProperty<XbimVector3D>(nodeEntities[0].ObjectMatrix3D.Translation);
            XbimVector3D nodesDirection = nodeEntities[1].ObjectMatrix3D.Translation - Coordinates.Value;
            XbimVector3D pipeProjection = new XbimVector3D(
                pipeEntity.ProjectionAlongOXAxis.SIProperty,
                pipeEntity.ProjectionAlongOYAxis.SIProperty,
                pipeEntity.ProjectionAlongOZAxis.SIProperty
            );
            Direction = (pipeProjection * XbimVector3D.DotProduct(nodesDirection, pipeProjection)).Normalized() * pipeProjection.Length;
            RealLength = new ActionProperty<double>(Direction.Length);
            Length = pipeProjection.Length;

            XbimVector3D forward = Direction.Normalized();
            ObjectMatrix3D = MatrixExtensions.CreateWorld(Coordinates.Value, forward);
            
            Diameter = pipeEntity.Diameter.SIProperty;
            OuterSurfaceArea = new ActionProperty<double>(MathExtensions.CalculateCylinderArea(Diameter / 2, RealLength.Value));
            
            RealLength.OnValueChange += () => OuterSurfaceArea.Value = MathExtensions.CalculateCylinderArea(Diameter / 2, RealLength.Value);
        }

        public static IfcPipeSegmentEntity? CreateFromIfc(IfcPipeSegment pipeSegment)
        {
            XbimMatrix3D matrix3D = pipeSegment.ObjectPlacement.ToMatrix3D();
            StartPipeEntity pipeEntity = new StartPipeEntity();
            pipeEntity.Name = pipeSegment.Name ?? string.Empty;

            XbimVector3D coordinates = matrix3D.Translation;
            XbimVector3D direction = GetPipeDirection(pipeSegment);
            if (direction == XbimVector3D.Zero)
                throw new IfcConvertException("Cannot find direction of pipe segment");

            IIfcPropertySet? psetStart = pipeSegment.PropertySets.FirstOrDefault(set => set.Name == nameof(Pset_Start));
            if (psetStart != null)
                UpdatePipeFromPsetStart(psetStart, ref pipeEntity);

            IIfcPropertySet? psetTypeCommon = pipeSegment.PropertySets.FirstOrDefault(set => set.Name == nameof(Pset_PipeSegmentTypeCommon));
            if (psetTypeCommon != null)
                UpdatePipeFromPsetTypeCommon(psetTypeCommon, ref pipeEntity);

            IIfcElementQuantity? elementQuantity = pipeSegment.ElementQuantities.FirstOrDefault(quantity => quantity.Name == nameof(Qto_PipeSegmentBaseQuantities));
            if (elementQuantity != null)
                UpdatePipeFromQtoSegmentBase(elementQuantity, direction, ref pipeEntity);

            IfcNodeEntity[] nodeEntities = new IfcNodeEntity[]
            {
                IfcNodeEntity.CreateFromIfc(coordinates, 1),
                IfcNodeEntity.CreateFromIfc(coordinates + direction, 2)
            };

            return new IfcPipeSegmentEntity(pipeEntity, nodeEntities);
        }

        private static void UpdatePipeFromPsetStart(IIfcPropertySet psetStart, ref StartPipeEntity pipeEntity)
        {
            double GetPropertyValue(string rawValue)
            {
                Regex regex = new Regex(@"(\d+.\d+)|\d+");
                Match match = regex.Match(rawValue);
                return Convert.ToDouble(match.Value);
            }

            Pset_Start pset = Pset_Start.CreateFromPropertySet(psetStart);
            Dictionary<string, string> data = pset.Data;
            bool isValidTechnology = Enum.TryParse(data["ManufacturingTechnologyEnum"], out StartManufacturingTechnologyEnum manufacturingTechnologyEnum);

            pipeEntity.MaterialName = data["MaterialName"];
            pipeEntity.MillTolerance = new LengthProperty(GetPropertyValue(data["MillTolerance"]));
            pipeEntity.CorrosionAllowance = new LengthProperty(GetPropertyValue(data["CorrosionAllowance"]));
            pipeEntity.PipeUnitWeight = new MassUnitProperty(GetPropertyValue(data["PipeUnitWeight"]));
            pipeEntity.InsulationUnitWeight = new MassUnitProperty(GetPropertyValue(data["InsulationUnitWeight"]));
            pipeEntity.ProductUnitWeight = new MassUnitProperty(GetPropertyValue(data["ProductUnitWeight"]));
            pipeEntity.ManufacturingTechnologyEnum = isValidTechnology ? manufacturingTechnologyEnum : StartManufacturingTechnologyEnum.SEAMLESS;
            pipeEntity.LongitudinalWeldJointFactor = new FactorProperty(GetPropertyValue(data["LongitudinalWeldJointFactor"]));
            pipeEntity.StrengthFactorOfTheTraverseWeld = new FactorProperty(GetPropertyValue(data["StrengthFactorOfTheTraverseWeld"]));
            pipeEntity.AdditionalWeightLoad = new MassUnitProperty(GetPropertyValue(data["AdditionalWeightLoad"]));
            pipeEntity.AdditionalWeightLoadAlongTheXAxis = new MassUnitProperty(GetPropertyValue(data["AdditionalWeightLoadAlongTheXAxis"]));
            pipeEntity.AdditionalWeightLoadAlongTheYAxis = new MassUnitProperty(GetPropertyValue(data["AdditionalWeightLoadAlongTheYAxis"]));
            pipeEntity.AdditionalWeightLoadAlongTheZAxis = new MassUnitProperty(GetPropertyValue(data["AdditionalWeightLoadAlongTheZAxis"]));
            pipeEntity.ProjectionAlongOXAxis = new LengthProperty(GetPropertyValue(data["ProjectionAlongOXAxis"]));
            pipeEntity.ProjectionAlongOYAxis = new LengthProperty(GetPropertyValue(data["ProjectionAlongOYAxis"]));
            pipeEntity.ProjectionAlongOZAxis = new LengthProperty(GetPropertyValue(data["ProjectionAlongOZAxis"]));
            pipeEntity.XCoord = new LengthProperty(GetPropertyValue(data["XCoord"]));
            pipeEntity.YCoord = new LengthProperty(GetPropertyValue(data["YCoord"]));
            pipeEntity.ZCoord = new LengthProperty(GetPropertyValue(data["ZCoord"]));
        }

        private static void UpdatePipeFromPsetTypeCommon(IIfcPropertySet psetTypeCommon, ref StartPipeEntity pipeEntity)
        {
            Pset_PipeSegmentTypeCommon pset = Pset_PipeSegmentTypeCommon.CreateFromPropertySet(psetTypeCommon);
            pipeEntity.Diameter = new LengthProperty(pset.NominalDiameter);
            pipeEntity.WallThickness = new LengthProperty(pset.NominalDiameter - pset.InnerDiameter);
        }

        private static void UpdatePipeFromQtoSegmentBase(IIfcElementQuantity elementQuantity, XbimVector3D direction, ref StartPipeEntity pipeEntity)
        {
            Qto_PipeSegmentBaseQuantities qto = Qto_PipeSegmentBaseQuantities.CreateFromQuantitySet(elementQuantity);
            pipeEntity.ProjectionAlongOXAxis = new LengthProperty(direction.X * qto.Length);
            pipeEntity.ProjectionAlongOYAxis = new LengthProperty(direction.Y * qto.Length);
            pipeEntity.ProjectionAlongOZAxis = new LengthProperty(direction.Z * qto.Length);
        }

        private static IEnumerable<IfcRepresentationItem> GetRepresentationItems(IfcPipeSegment pipeSegment)
        {
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            foreach (IfcRepresentation representation in pipeSegment.Representation.Representations)
            {
                representationItems.AddRange(representation.Items);
            }

            return representationItems;
        }

        private static XbimVector3D GetPipeDirection(IfcPipeSegment pipeSegment)
        {
            XbimMatrix3D matrix3D = pipeSegment.ObjectPlacement.ToMatrix3D();
            XbimVector3D direction = XbimVector3D.Zero;
            
            IfcRepresentationItem[] representationItems = GetRepresentationItems(pipeSegment).ToArray();
            foreach (IfcRepresentationItem representationItem in representationItems)
            {
                if (representationItem is IfcExtrudedAreaSolid areaSolid)
                {
                    direction = matrix3D.Transform(areaSolid.ExtrudedDirection.XbimVector3D());
                    break;
                }
            }
            
            return direction;
        }
    }
}