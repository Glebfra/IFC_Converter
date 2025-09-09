using System;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Segments;
using IFC.Extensions;
using IFC.PropertySets;
using IFCConverter.Extensions.Entities;
using IFCConverter.Extensions.Entities.Segments;
using IFCConverter.Extensions.PropertySets;
using IFCConverter.Extensions.Tools;
using IFCConverter.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFCConverter.Importers
{
    internal class AvevaImporter : StandardImporter
    {
        public AvevaImporter(IModel model) : base(model) { }
        
        public override IfcElement[] GetPipeSegments(IfcProduct[] products)
        {
            return GetElementByType(products, "TUBING");
        }

        public override IfcElement[] GetBends(IfcProduct[] products)
        {
            return GetElementByType(products, "ELBOW");
        }
        
        public override IfcElement[] GetTees(IfcProduct[] products)
        {
            return GetElementByType(products, "TEE");
        }

        public override IfcPipeSegmentEntity[] CreatePipeSegments(IfcElement[] pipes)
        {
            IfcPipeSegmentEntity[] pipeSegmentEntities = new IfcPipeSegmentEntity[pipes.Length];
            for (int i = 0; i < pipes.Length; i++)
            {
                IfcElement pipe = pipes[i];
                IfcLabel name = pipe.Name ?? new IfcLabel("");
                IfcIdentifier tag = pipe.Tag ?? new IfcIdentifier("");
                
                IfcExtrudedAreaSolid? extrudedAreaSolid = null;
                foreach (IfcRepresentation ifcRepresentation in pipe.Representation.Representations)
                {
                    extrudedAreaSolid = ifcRepresentation.Items.OfType<IfcExtrudedAreaSolid>().FirstOrDefault();
                    if (extrudedAreaSolid != null)
                        break;
                }
                if (extrudedAreaSolid == null)
                    throw new NullReferenceException("Pipe does not have IfcExtrudedAreaSolid representation.");
                
                XbimMatrix3D shapeMatrix3D = extrudedAreaSolid.Position.ToObjectMatrix3D().RescaleTranslation(_LengthUnit.Power);
                double length = GetPipeLength(extrudedAreaSolid) * _LengthUnit.Power;
                double diameter = GetPipeDiameter(extrudedAreaSolid) * _LengthUnit.Power;
                
                IPropertySet[] propertySets = pipe.GetPropertySets().ToArray();

                IfcPipeSegmentEntity pipeSegmentEntity = new IfcPipeSegmentEntity(name, tag, shapeMatrix3D, length, diameter);
                pipeSegmentEntity.PropertySets.AddRange(propertySets);
                pipeSegmentEntities[i] = pipeSegmentEntity;
            }

            return pipeSegmentEntities;
        }

        public override IfcCadBendEntity[] CreateBends(IfcElement[] bends, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            IfcCadBendEntity[] bendEntities = new IfcCadBendEntity[bends.Length];
            for (int i = 0; i < bends.Length; i++)
            {
                IfcElement bend = bends[i];
                IfcLabel name = bend.Name ?? new IfcLabel("");
                IfcIdentifier tag = bend.Tag ?? new IfcIdentifier("");
                
                IPropertySet[] propertySets = bend.GetPropertySets().ToArray();
                
                AVEVA_EntityParameters? avevaEntityParameters = propertySets.OfType<AVEVA_EntityParameters>().FirstOrDefault();
                if (avevaEntityParameters == null)
                    throw new Exception("Bend does not have AVEVA_EntityParameters property set.");
                
                AVEVA_Pset? avevaPset = propertySets.OfType<AVEVA_Pset>().FirstOrDefault();
                if (avevaPset == null)
                    throw new Exception("Bend does not have AVEVA_Pset property set.");

                XbimVector3D coordinates = avevaPset.GetPosition() * _LengthUnit.Power;
                IfcAbstractSegmentEntity[] nearestSegments = abstractSegmentEntities.GetNearestSegments(coordinates, 2).ToArray();
                XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(coordinates, nearestSegments, out double angle);
                double pipeRadius = Math.Max(nearestSegments[0].Diameter, nearestSegments[1].Diameter);

                IndexedResult<IfcNodeEntity>[] nearestNodes = nearestSegments
                    .Select(segment => segment.NodeEntities.GetNearestNode(coordinates))
                    .ToArray();

                XbimVector3D[] points = nearestNodes
                    .Select(node => node.Object.ObjectMatrix3D.Translation)
                    .ToArray();
                double bendRadius = GetBendRadius(points, angle);
                double length = bendRadius * angle;

                IfcCadBendEntity bendEntity = new IfcCadBendEntity(name, tag, objectMatrix3D, length, angle, bendRadius, pipeRadius);
                bendEntity.PropertySets.AddRange(propertySets);
                bendEntity.ConnectedEntities.AddRange(nearestSegments);
                bendEntities[i] = bendEntity;

                double clipLength = bendRadius * Math.Tan(angle / 2);
                for (int j = 0; j < nearestSegments.Length; j++)
                {
                    nearestSegments[j].Clip(bendEntity.NodeEntity, -clipLength);
                    nearestSegments[j].NodeEntities[nearestNodes[j].Index] = bendEntity.NodeEntity;
                }
            }

            return bendEntities;
        }

        public override IfcWeldedTeeEntity[] CreateWeldedTees(IfcElement[] tees, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            IfcWeldedTeeEntity[] weldedTeeEntities = new IfcWeldedTeeEntity[tees.Length];
            for (int i = 0; i < tees.Length; i++)
            {
                IfcElement tee = tees[i];
                IfcLabel name = tee.Name ?? new IfcLabel("");
                IfcIdentifier tag = tee.Tag ?? new IfcIdentifier("");

                IPropertySet[] propertySets = tee.GetPropertySets().ToArray();
                
                AVEVA_EntityParameters? avevaEntityParameters = propertySets.OfType<AVEVA_EntityParameters>().FirstOrDefault();
                if (avevaEntityParameters == null)
                    throw new Exception("Bend does not have AVEVA_EntityParameters property set.");
                
                AVEVA_Pset? avevaPset = propertySets.OfType<AVEVA_Pset>().FirstOrDefault();
                if (avevaPset == null)
                    throw new Exception("Bend does not have AVEVA_Pset property set.");

                XbimVector3D coordinates = avevaPset.GetPosition() * _LengthUnit.Power;
                IfcAbstractSegmentEntity[] nearestSegments = abstractSegmentEntities.GetNearestSegments(coordinates, 3).ToArray();
                XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(coordinates, nearestSegments,
                    out double angle, 
                    out IfcAbstractSegmentEntity headPipe,
                    out IfcAbstractSegmentEntity[] branchPipes
                );
                double branchDiameter = Math.Max(branchPipes[0].Diameter, branchPipes[1].Diameter);
                double headDiameter = headPipe.Diameter;

                IfcExtrudedAreaSolid[] extrudedAreaSolids = tee.Representation.Representations
                    .SelectMany(representation => representation.Items)
                    .Cast<IfcExtrudedAreaSolid>()
                    .ToArray();
                IfcExtrudedAreaSolid headExtrudedAreaSolid = extrudedAreaSolids
                    .First(solid => solid.Position.ToObjectMatrix3D().Forward.IsParallel(headPipe.ObjectMatrix3D.Value.Forward));
                IfcExtrudedAreaSolid branchExtrudedAreaSolid = extrudedAreaSolids
                    .First(solid => solid.Position.ToObjectMatrix3D().Forward.IsNormal(headPipe.ObjectMatrix3D.Value.Forward, 1e-3));

                double length = branchExtrudedAreaSolid.Depth * _LengthUnit.Power;
                double height = headExtrudedAreaSolid.Depth * _LengthUnit.Power;

                IfcWeldedTeeEntity weldedTeeEntity = new IfcWeldedTeeEntity(name, tag, objectMatrix3D, length, branchDiameter, headDiameter, height, angle);
                weldedTeeEntity.PropertySets.AddRange(propertySets);
                weldedTeeEntity.ConnectedEntities.AddRange(nearestSegments);
                weldedTeeEntities[i] = weldedTeeEntity;

                IfcNodeEntity weldedTeeNode = weldedTeeEntity.NodeEntity;
                foreach (IfcAbstractSegmentEntity branchPipe in branchPipes)
                {
                    branchPipe.Clip(weldedTeeNode, -length / 2);
                    IndexedResult<IfcNodeEntity> branchNodeResult = branchPipe.NodeEntities.GetNearestNode(weldedTeeNode);
                    branchPipe.NodeEntities[branchNodeResult.Index] = weldedTeeNode;
                }
                headPipe.Clip(weldedTeeNode, -height);
                IndexedResult<IfcNodeEntity> headNodeResult = headPipe.NodeEntities.GetNearestNode(weldedTeeNode);
                headPipe.NodeEntities[headNodeResult.Index] = weldedTeeNode;
            }
            return weldedTeeEntities;
        }

        protected static IfcElement[] GetElementByType(IfcProduct[] products, IfcText type)
        {
            return products
                .Select(product => new { Product = product, PropertySets = product.GetPropertySets() })
                .Where(item =>
                {
                    AVEVA_EntityParameters? pset = item.PropertySets.OfType<AVEVA_EntityParameters>().FirstOrDefault();
                    return pset?.E3DType.Value == type;
                })
                .Select(item => item.Product)
                .Cast<IfcElement>()
                .ToArray();
        }

        protected static double GetPipeLength(IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            return extrudedAreaSolid.Depth;
        }

        protected static double GetPipeDiameter(IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            IfcCircleProfileDef? circleProfileDef = extrudedAreaSolid.SweptArea as IfcCircleProfileDef;
            if (circleProfileDef == null)
                throw new Exception("Pipe profile is not a circle.");
            return circleProfileDef.Radius * 2;
        }

        private static double GetBendRadius(XbimVector3D[] points, double angle)
        {
            return (points[1] - points[0]).Length / (2 * Math.Sin(angle / 2));
        }
    }
}