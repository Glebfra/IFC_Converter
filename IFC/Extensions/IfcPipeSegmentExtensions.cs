using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IFC.Entities;
using IFC.Entities.Segments;
using IFC.PropertySets;
using Start.API;
using Start.Entities.Segments;
using Start.Extensions;
using Start.StartProperties;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;

namespace IFC.Extensions
{
    #if NEW
    
    
    
    #else
    
    public static class IfcPipeSegmentExtensions
    {
        public static IfcPipeSegmentEntity CreateFromIfc(IfcPipeSegment pipeSegment)
        {
            StartPipeEntity pipeEntity = new StartPipeEntity();
            pipeEntity.Name = pipeSegment.Name ?? string.Empty;

            XbimMatrix3D matrix3D = pipeSegment.ObjectPlacement.ToMatrix3D();
            IfcRepresentationItem[] representationItems = pipeSegment.GetRepresentationItems().ToArray();
            IfcNodeEntity[] nodeEntities = CreateNodeEntities(representationItems, matrix3D);

            XbimVector3D pipeProjection = nodeEntities[1].ObjectMatrix3D.Translation - nodeEntities[0].ObjectMatrix3D.Translation;
            pipeEntity.SetProjection(pipeProjection);

            IIfcPropertySet? psetStart = pipeSegment.PropertySets.FirstOrDefault(set => set.Name == nameof(Pset_Start));
            if (psetStart != null)
                UpdatePipeFromPsetStart(psetStart, ref pipeEntity);

            IIfcPropertySet? psetTypeCommon = pipeSegment.PropertySets.FirstOrDefault(set => set.Name == nameof(Pset_PipeSegmentTypeCommon));
            if (psetTypeCommon != null)
            {
                UpdatePipeFromPsetTypeCommon(psetTypeCommon, ref pipeEntity);
            }

            return new IfcPipeSegmentEntity(pipeEntity, nodeEntities);
        }

        public static XbimVector3D ReplaceNearestNodeAndRescale(this IfcPipeSegmentEntity pipeSegmentEntity, IfcNodeEntity nodeEntity)
        {
            XbimVector3D displacement = pipeSegmentEntity.ReplaceNearestNode(nodeEntity);
            double length = displacement.Length;
            StartPipeEntity startPipeEntity = (StartPipeEntity)pipeSegmentEntity.StartAbstractEntity;
            XbimVector3D oldProjection = startPipeEntity.GetProjection();
            startPipeEntity.SetProjection(oldProjection + oldProjection.Normalized() * length);

            return displacement;
        }

        private static void UpdatePipeFromPsetStart(IIfcPropertySet psetStart, ref StartPipeEntity pipeEntity)
        {
            double GetPropertyValue(string rawValue)
            {
                Regex regex = new Regex(@"-(\d+,\d+)|-(\d+.\d+)|-\d+|(\d+,\d+)|(\d+.\d+)|\d+");
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
        }

        private static void UpdatePipeFromPsetTypeCommon(IIfcPropertySet psetTypeCommon, ref StartPipeEntity pipeEntity)
        {
            Pset_PipeSegmentTypeCommon pset = Pset_PipeSegmentTypeCommon.CreateFromPropertySet(psetTypeCommon);
            pipeEntity.Diameter = LengthProperty.CreateFromSi(pset.NominalDiameter);
            pipeEntity.WallThickness = LengthProperty.CreateFromSi(pset.NominalDiameter - pset.InnerDiameter);
        }

        private static bool UpdateFromCircleProfileDef()
        {
            throw new NotImplementedException();
        }

        private static bool TryGetPositionFromExtrudedAreaSolid(IfcRepresentationItem representationItem, out XbimVector3D coordinates, out XbimVector3D direction)
        {
            coordinates = XbimVector3D.Zero;
            direction = XbimVector3D.Zero;
            
            if (representationItem is not IfcExtrudedAreaSolid areaSolid) 
                return false;
            
            if (areaSolid.Position != null)
            {
                XbimMatrix3D areaSolidMatrix3D = areaSolid.Position.ToMatrix3D();
                direction = areaSolidMatrix3D.Transform(areaSolid.ExtrudedDirection.XbimVector3D()).Normalized() * areaSolid.Depth;
                coordinates += areaSolidMatrix3D.Translation;
                return true;
            }
            
            direction = areaSolid.ExtrudedDirection.XbimVector3D() * areaSolid.Depth;
            return true;
        }

        private static bool TryGetPositionFromRightCircularCylinder(IfcRepresentationItem representationItem, out XbimVector3D coordinates, out XbimVector3D direction)
        {
            coordinates = XbimVector3D.Zero;
            direction = XbimVector3D.Zero;
            
            if (representationItem is not IfcRightCircularCylinder rightCircularCylinder) 
                return false;
            
            if (rightCircularCylinder.Position != null)
            {
                XbimMatrix3D cylinderMatrix = rightCircularCylinder.Position.ToMatrix3D();
                direction = cylinderMatrix.Forward * rightCircularCylinder.Height;
                coordinates += rightCircularCylinder.Position.Location.ToXbimVector3D();
                return true;
            }

            return false;
        }

        private static IfcNodeEntity[] CreateNodeEntities(IEnumerable<IfcRepresentationItem> representationItems, XbimMatrix3D matrix3D)
        {
            XbimVector3D coordinates = matrix3D.Translation;
            XbimVector3D direction = matrix3D.Forward;
            
            foreach (IfcRepresentationItem representationItem in representationItems)
            {
                if (TryGetPositionFromExtrudedAreaSolid(representationItem, out XbimVector3D areaCoordinates, out XbimVector3D areaDirection))
                {
                    coordinates += matrix3D.Transform(areaCoordinates);
                    direction = matrix3D.Transform(areaDirection);
                    break;
                }

                if (TryGetPositionFromRightCircularCylinder(representationItem, out XbimVector3D circularCoordinates, out XbimVector3D circularDirection))
                {
                    coordinates += matrix3D.Transform(circularCoordinates);
                    direction = matrix3D.Transform(circularDirection);
                    break;
                }
            }

            return new IfcNodeEntity[]
            {
                IfcNodeEntity.CreateFromIfc(coordinates),
                IfcNodeEntity.CreateFromIfc(coordinates + direction)
            };
        }
    }

    #endif
}