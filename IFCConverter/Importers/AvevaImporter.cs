using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Fittings.Vertex;
using IFC.Entities.Segments;
using IFC.Extensions;
using IFC.PropertySets;
using IFCConverter.Extensions.Entities;
using IFCConverter.Extensions.Entities.Segments;
using IFCConverter.Extensions.Tools;
using IFCConverter.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;

namespace IFCConverter.Importers
{
    internal class AvevaImporter : StandardImporter
    {
        public AvevaImporter(IModel model) : base(model) { }
        
        public override IfcElement[] GetPipeSegments(IfcProduct[] products) => GetElementByType(products, "TUBING");
        public override IfcElement[] GetBends(IfcProduct[] products) => GetElementByType(products, "ELBOW");
        public override IfcElement[] GetTees(IfcProduct[] products) => GetElementByType(products, "TEE");
        public override IfcElement[] GetReducers(IfcProduct[] products) => GetElementByType(products, "REDUCER");

        public override IfcPipeSegmentEntity[] CreatePipeSegments(IfcElement[] pipes)
        {
            IfcPipeSegmentEntity[] pipeSegmentEntities = new IfcPipeSegmentEntity[pipes.Length];
            for (int i = 0; i < pipes.Length; i++)
            {
                IfcElement pipe = pipes[i];
                IfcLabel name = pipe.Name ?? new IfcLabel("");
                IfcIdentifier tag = pipe.Tag ?? new IfcIdentifier("");
                
                IfcExtrudedAreaSolid extrudedAreaSolid = pipe
                    .GetRepresentationItems()
                    .OfType<IfcExtrudedAreaSolid>()
                    .First();

                XbimMatrix3D shapeMatrix3D = extrudedAreaSolid.Position.ToObjectMatrix3D().RescaleTranslation(_LengthUnit.Power);
                double length = extrudedAreaSolid.GetLength() * _LengthUnit.Power;
                double diameter = extrudedAreaSolid.GetCircleRadius() * 2 * _LengthUnit.Power;
                
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

                IfcRevolvedAreaSolid revolvedAreaSolid = bend
                    .GetRepresentationItems()
                    .OfType<IfcRevolvedAreaSolid>()
                    .First();
                XbimVector3D[] boundPoints = revolvedAreaSolid.GetBoundPoints();
                boundPoints = boundPoints
                    .Select(point => point * _LengthUnit.Power)
                    .ToArray();
                IfcAbstractSegmentEntity[] connectedSegments = abstractSegmentEntities
                    .GetConnectedSegments(boundPoints)
                    .ToArray();

                XbimVector3D coordinates = avevaPset.GetPosition() * _LengthUnit.Power;
                double bendRadius = revolvedAreaSolid.GetRadius() * _LengthUnit.Power;
                double angle = revolvedAreaSolid.GetAngle();
                XbimMatrix3D objectMatrix3D;
                double pipeRadius;

                if (connectedSegments.Length is 2)
                {
                    objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(coordinates, connectedSegments, out angle);
                    pipeRadius = Math.Max(connectedSegments[0].Diameter, connectedSegments[1].Diameter);
                }
                else if (connectedSegments.Length is 1)
                {
                    objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(coordinates);
                    pipeRadius = connectedSegments[0].Diameter;
                }
                else
                {
                    throw new NullReferenceException($"Cannot find connected segments to {nameof(bend)}");
                }

                double length = bendRadius * angle;
                
                IfcCadBendEntity bendEntity = new IfcCadBendEntity(name, tag, objectMatrix3D, length, angle, bendRadius, pipeRadius);
                bendEntity.PropertySets.AddRange(propertySets);
                bendEntity.ConnectedEntities.AddRange(connectedSegments);
                bendEntities[i] = bendEntity;
                
                double clipLength = bendRadius * Math.Tan(angle / 2);
                foreach (IfcAbstractSegmentEntity connectedSegment in connectedSegments)
                {
                    IfcNodeEntity bendNodeEntity = bendEntity.NodeEntity;
                    IndexedResult<IfcNodeEntity> connectedNodeEntity = connectedSegment.NodeEntities.GetNearestNode(bendNodeEntity);
                    connectedSegment.Clip(bendNodeEntity, -clipLength);
                    connectedSegment.NodeEntities[connectedNodeEntity.Index] = bendNodeEntity;
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

                IfcExtrudedAreaSolid[] extrudedAreaSolids = tee
                    .GetRepresentationItems()
                    .OfType<IfcExtrudedAreaSolid>()
                    .ToArray();
                XbimVector3D[] boundPoints = extrudedAreaSolids
                    .SelectMany(solid => solid.GetBoundPoints())
                    .ToArray();
                boundPoints = boundPoints
                    .Select(point => point * _LengthUnit.Power)
                    .ToArray();
                
                IfcAbstractSegmentEntity[] connectedSegments = abstractSegmentEntities
                    .GetConnectedSegments(boundPoints)
                    .ToArray();

                XbimVector3D coordinates = avevaPset.GetPosition() * _LengthUnit.Power;
                XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(coordinates, connectedSegments,
                    out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes
                );
                
                double branchDiameter = Math.Max(branchPipes[0].Diameter, branchPipes[1].Diameter);
                double headDiameter = headPipe.Diameter;
                
                IfcExtrudedAreaSolid headExtrudedAreaSolid = extrudedAreaSolids
                    .First(solid => solid.Position.ToObjectMatrix3D().Forward.IsParallel(headPipe.ObjectMatrix3D.Value.Forward));
                IfcExtrudedAreaSolid branchExtrudedAreaSolid = extrudedAreaSolids
                    .First(solid => solid.Position.ToObjectMatrix3D().Forward.IsNormal(headPipe.ObjectMatrix3D.Value.Forward, 1e-2));

                double length = branchExtrudedAreaSolid.GetLength() * _LengthUnit.Power;
                double height = headExtrudedAreaSolid.GetLength() * _LengthUnit.Power;
                
                IfcWeldedTeeEntity weldedTeeEntity = new IfcWeldedTeeEntity(name, tag, objectMatrix3D, length, branchDiameter, headDiameter, height, angle);
                weldedTeeEntity.PropertySets.AddRange(propertySets);
                weldedTeeEntity.ConnectedEntities.AddRange(connectedSegments);
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

        public override IfcVertexReducerEccentricEntity[] CreateEccentricReducers(IfcElement[] reducers, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            IfcVertexReducerEccentricEntity[] eccentricReducerEntities = new IfcVertexReducerEccentricEntity[reducers.Length];
            for (int i = 0; i < reducers.Length; i++)
            {
                IfcElement reducer = reducers[i];
                IfcLabel name = reducer.Name ?? new IfcLabel("");
                IfcIdentifier tag = reducer.Tag ?? new IfcIdentifier("");
                
                IPropertySet[] propertySets = reducer.GetPropertySets().ToArray();
                
                AVEVA_EntityParameters? avevaEntityParameters = propertySets.OfType<AVEVA_EntityParameters>().FirstOrDefault();
                if (avevaEntityParameters == null)
                    throw new Exception("Bend does not have AVEVA_EntityParameters property set.");
                
                AVEVA_Pset? avevaPset = propertySets.OfType<AVEVA_Pset>().FirstOrDefault();
                if (avevaPset == null)
                    throw new Exception("Bend does not have AVEVA_Pset property set.");

                IfcTriangulatedFaceSet? faceSet = reducer
                    .GetRepresentationItems()
                    .OfType<IfcTriangulatedFaceSet>()
                    .FirstOrDefault();
                if (faceSet == null)
                    throw new NullReferenceException("Reducer does not have IfcTriangulatedFaceSet representation.");
                
                ReducerProperties reducerProperties = faceSet.GetReducerProperties(avevaPset);
                XbimVector3D[] boundPoints = reducerProperties.BoundPoints
                    .Select(point => point * _LengthUnit.Power)
                    .ToArray();
                double[] diameters = reducerProperties.Radiuses
                    .Select(radius => radius * _LengthUnit.Power * 2)
                    .ToArray();

                IfcAbstractSegmentEntity[] connectedSegments = abstractSegmentEntities
                    .GetConnectedSegments(boundPoints)
                    .ToArray();
                
                XbimVector3D coordinates = reducerProperties.Center * _LengthUnit.Power;
                XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateReducerEccentricObjectMatrix(coordinates, connectedSegments, out double displacementLength);
                double largeDiameter = reducerProperties.Radiuses[0] * 2;
                double smallDiameter = reducerProperties.Radiuses[1] * 2;

                IfcVertexReducerEccentricEntity eccentricReducerEntity = new IfcVertexReducerEccentricEntity(
                    name, tag, objectMatrix3D, reducerProperties.Length, displacementLength, diameters, 16
                );
                
                eccentricReducerEntity.PropertySets.AddRange(propertySets);
                eccentricReducerEntity.ConnectedEntities.AddRange(connectedSegments);
                eccentricReducerEntities[i] = eccentricReducerEntity;
            }

            return eccentricReducerEntities;
        }

        private static IfcElement[] GetElementByType(IEnumerable<IfcProduct> products, IfcText type)
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
    }
}