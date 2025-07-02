using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IFC.Entities.Abstract.Segments;
using IFC.EntitiesExtensions;
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
            StartPipeEntity pipeEntity = new StartPipeEntity();
            pipeEntity.Name = pipeSegment.Name ?? string.Empty;

            XbimMatrix3D matrix3D = pipeSegment.ObjectPlacement.ToMatrix3D();
            XbimVector3D coordinates = matrix3D.Translation;
            XbimVector3D pipeProjection = GetPipeProjection(pipeSegment);
            
            IfcNodeEntity[] nodeEntities = new IfcNodeEntity[]
            {
                IfcNodeEntity.CreateFromIfc(coordinates, 0),
                IfcNodeEntity.CreateFromIfc(coordinates + pipeProjection, 0)
            };

            IIfcPropertySet? psetStart = pipeSegment.PropertySets.FirstOrDefault(set => set.Name == nameof(Pset_Start));
            if (psetStart != null)
            {
                UpdatePipeFromPsetStart(psetStart, ref pipeEntity);
                return new IfcPipeSegmentEntity(pipeEntity, nodeEntities);
            }

            IIfcPropertySet? psetTypeCommon = pipeSegment.PropertySets.FirstOrDefault(set => set.Name == nameof(Pset_PipeSegmentTypeCommon));
            if (psetTypeCommon != null)
                UpdatePipeFromPsetTypeCommon(psetTypeCommon, ref pipeEntity);

            IIfcElementQuantity? elementQuantity = pipeSegment.ElementQuantities.FirstOrDefault(quantity => quantity.Name == nameof(Qto_PipeSegmentBaseQuantities));
            if (elementQuantity != null)
                UpdatePipeFromQtoSegmentBase(elementQuantity, pipeProjection, ref pipeEntity);

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
            
            if (data.TryGetValue(nameof(pipeEntity.ManufacturingTechnologyEnum), out string manufacturingTechnology))
            {
                bool isValidTechnology = Enum.TryParse(data["ManufacturingTechnologyEnum"], out StartManufacturingTechnologyEnum manufacturingTechnologyEnum);
                pipeEntity.ManufacturingTechnologyEnum = isValidTechnology ? manufacturingTechnologyEnum : StartManufacturingTechnologyEnum.SEAMLESS;
            }

            if (data.TryGetValue(nameof(pipeEntity.Diameter), out string diameter))
                pipeEntity.Diameter = LengthProperty.CreateFromSi(GetPropertyValue(diameter));
            if (data.TryGetValue(nameof(pipeEntity.MaterialName), out string materialName))
                pipeEntity.MaterialName = materialName;
            if (data.TryGetValue(nameof(pipeEntity.MillTolerance), out string millTolerance))
                pipeEntity.MillTolerance = LengthProperty.CreateFromSi(GetPropertyValue(millTolerance));
            if (data.TryGetValue(nameof(pipeEntity.CorrosionAllowance), out string corrosionAllowance))
                pipeEntity.CorrosionAllowance = LengthProperty.CreateFromSi(GetPropertyValue(corrosionAllowance));
            if (data.TryGetValue(nameof(pipeEntity.PipeUnitWeight), out string pipeUnitWeight))
                pipeEntity.PipeUnitWeight = MassUnitProperty.CreateFromSi(GetPropertyValue(pipeUnitWeight));
            if (data.TryGetValue(nameof(pipeEntity.InsulationUnitWeight), out string insulationWeight))
                pipeEntity.InsulationUnitWeight = MassUnitProperty.CreateFromSi(GetPropertyValue(insulationWeight));
            if (data.TryGetValue(nameof(pipeEntity.ProductUnitWeight), out string productUnitWeight))
                pipeEntity.ProductUnitWeight = MassUnitProperty.CreateFromSi(GetPropertyValue(productUnitWeight));
            if (data.TryGetValue(nameof(pipeEntity.LongitudinalWeldJointFactor), out string longitudinalWeldJointFactor))
                pipeEntity.LongitudinalWeldJointFactor = FactorProperty.CreateFromSi(GetPropertyValue(longitudinalWeldJointFactor));
            if (data.TryGetValue(nameof(pipeEntity.StrengthFactorOfTheTraverseWeld), out string strengthFactorOfTraverseWeld))
                pipeEntity.StrengthFactorOfTheTraverseWeld = FactorProperty.CreateFromSi(GetPropertyValue(strengthFactorOfTraverseWeld));
            if (data.TryGetValue(nameof(pipeEntity.AdditionalWeightLoad), out string additionalWeightLoad))
                pipeEntity.AdditionalWeightLoad = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoad));
            if (data.TryGetValue(nameof(pipeEntity.AdditionalWeightLoadAlongTheXAxis), out string additionalWeightLoadAlongTheXAxis))
                pipeEntity.AdditionalWeightLoadAlongTheXAxis = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoadAlongTheXAxis));
            if (data.TryGetValue(nameof(pipeEntity.AdditionalWeightLoadAlongTheYAxis), out string additionalWeightLoadAlongTheYAxis))
                pipeEntity.AdditionalWeightLoadAlongTheYAxis = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoadAlongTheYAxis));
            if (data.TryGetValue(nameof(pipeEntity.AdditionalWeightLoadAlongTheZAxis), out string additionalWeightLoadAlongTheZAxis))
                pipeEntity.AdditionalWeightLoadAlongTheZAxis = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoadAlongTheZAxis));
            if (data.TryGetValue(nameof(pipeEntity.ProjectionAlongOXAxis), out string projectionAlongOXAxis))
                pipeEntity.ProjectionAlongOXAxis = LengthProperty.CreateFromSi(GetPropertyValue(projectionAlongOXAxis));
            if (data.TryGetValue(nameof(pipeEntity.ProjectionAlongOYAxis), out string projectionAlongOYAxis))
                pipeEntity.ProjectionAlongOYAxis = LengthProperty.CreateFromSi(GetPropertyValue(projectionAlongOYAxis));
            if (data.TryGetValue(nameof(pipeEntity.ProjectionAlongOZAxis), out string projectionAlongOZAxis))
                pipeEntity.ProjectionAlongOZAxis = LengthProperty.CreateFromSi(GetPropertyValue(projectionAlongOZAxis));
            if (data.TryGetValue(nameof(pipeEntity.XCoord), out string xCoord))
                pipeEntity.XCoord = LengthProperty.CreateFromSi(GetPropertyValue(xCoord));
            if (data.TryGetValue(nameof(pipeEntity.YCoord), out string yCoord))
                pipeEntity.YCoord = LengthProperty.CreateFromSi(GetPropertyValue(yCoord));
            if (data.TryGetValue(nameof(pipeEntity.ZCoord), out string zCoord))
                pipeEntity.ZCoord = LengthProperty.CreateFromSi(GetPropertyValue(zCoord));
        }

        private static void UpdatePipeFromPsetTypeCommon(IIfcPropertySet psetTypeCommon, ref StartPipeEntity pipeEntity)
        {
            Pset_PipeSegmentTypeCommon pset = Pset_PipeSegmentTypeCommon.CreateFromPropertySet(psetTypeCommon);
            pipeEntity.Diameter = LengthProperty.CreateFromSi(pset.NominalDiameter);
            pipeEntity.WallThickness = LengthProperty.CreateFromSi(pset.NominalDiameter - pset.InnerDiameter);
        }

        private static void UpdatePipeFromQtoSegmentBase(IIfcElementQuantity elementQuantity, XbimVector3D direction, ref StartPipeEntity pipeEntity)
        {
            Qto_PipeSegmentBaseQuantities qto = Qto_PipeSegmentBaseQuantities.CreateFromQuantitySet(elementQuantity);
            XbimVector3D newProjection = direction * qto.Length;
            pipeEntity.ProjectionAlongOXAxis = LengthProperty.CreateFromSi(newProjection.X);
            pipeEntity.ProjectionAlongOYAxis = LengthProperty.CreateFromSi(newProjection.Y);
            pipeEntity.ProjectionAlongOZAxis = LengthProperty.CreateFromSi(newProjection.Z);
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

        private static XbimVector3D GetPipeProjection(IfcPipeSegment pipeSegment)
        {
            XbimMatrix3D matrix3D = pipeSegment.ObjectPlacement.ToMatrix3D();
            XbimVector3D direction = XbimVector3D.Zero;
            
            IfcRepresentationItem[] representationItems = GetRepresentationItems(pipeSegment).ToArray();
            foreach (IfcRepresentationItem representationItem in representationItems)
            {
                if (representationItem is IfcExtrudedAreaSolid areaSolid)
                {
                    direction = matrix3D.Transform(areaSolid.ExtrudedDirection.XbimVector3D() * areaSolid.Depth);
                    break;
                }
            }
            
            return direction;
        }
    }
}