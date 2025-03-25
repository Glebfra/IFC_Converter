using System;
using System.Linq;
using IFC.Entities.Abstract;
using IFC.Tools;
using Start.API;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Entities.Fittings.Vertex
{
    [IfcEntityType(true, StartElementType.REDUCER_CONCENTRIC)]
    public sealed class IfcVertexReducerConcentricEntity : IfcAbstractFittingEntity
    {
        public double Length { get; }
        
        private readonly int _numSegments;
        private readonly double _angleStep;
        private IfcPipeFitting? _pipeFitting;
        private readonly StartReducerEntity _reducerEntity;

        public IfcVertexReducerConcentricEntity(StartReducerEntity reducerEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities, int numSegments)
            : base(reducerEntity, nodeEntity, ifcAbstractSegmentEntities)
        {
            _numSegments = numSegments;
            _angleStep = 2 * Math.PI / _numSegments;
            
            _reducerEntity = reducerEntity;

            XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D directionToPipe = IfcAxis.GetDirectionToPipe(ifcAbstractSegmentEntities[1], coordinates);
        
            XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
            XbimVector3D forward = directionToPipe.Normalized();
            if (forward == WorldUp)
                WorldUp = new XbimVector3D(0, 1, 0);
            else if (forward == -1 * WorldUp)
                WorldUp = new XbimVector3D(0, -1, 0);
            XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp).Normalized();
        
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
            Length = _reducerEntity.LengthOfConicalPart;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            double[] radiuses = _IfcAbstractSegmentEntities.Select(entity => entity.Diameter / 2).ToArray();

            double displacement1 = radiuses[0] > radiuses[1] ? -Length : 0;
            double displacement2 = radiuses[1] > radiuses[0] ? Length : 0;
            IfcCartesianPoint[] lowerCircle = CreateCircle(model, radiuses[0], displacement1);
            IfcCartesianPoint[] upperCircle = CreateCircle(model, radiuses[1], displacement2);
            IfcFacetedBrep facetedBrep = CreateFacetedBrep(model, lowerCircle, upperCircle);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, facetedBrep);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.TRANSITION;
                fitting.Tag = Tag;
                fitting.Name = _reducerEntity.Name;
            });
            _IfcAbstractSegmentEntities[0].Clip(IfcNodeEntity, Math.Abs(displacement1));
            _IfcAbstractSegmentEntities[1].Clip(IfcNodeEntity, Math.Abs(displacement2));

            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
    
        private IfcCartesianPoint[] CreateCircle(IModel model, double radius, double height)
        {
            IfcCartesianPoint[] points = new IfcCartesianPoint[_numSegments];
            for (int i = 0; i < _numSegments; i++)
            {
                XbimVector3D point = new XbimVector3D(radius * Math.Cos(_angleStep * i), radius * Math.Sin(_angleStep * i), height);
                points[i] = IfcAxis.CreatePoint(model, point);
            }

            return points;
        }
    
        private IfcFacetedBrep CreateFacetedBrep(IModel model, IfcCartesianPoint[] lowerPoints, IfcCartesianPoint[] upperPoints)
        {
            IfcFace[] faces = new IfcFace[_numSegments + 2];
            int facesIndex = 0;
            for (int i = 0; i < _numSegments; i++)
            {
                IfcCartesianPoint p1 = lowerPoints[i];
                IfcCartesianPoint p2 = lowerPoints[(i + 1) % _numSegments];
                IfcCartesianPoint p3 = upperPoints[(i + 1) % _numSegments];
                IfcCartesianPoint p4 = upperPoints[i];
                faces[facesIndex++] = IfcVertexGeometry.CreateRectangleFace(model, p1, p2, p3, p4);
            }
            faces[facesIndex++] = IfcVertexGeometry.CreatePolygonFace(model, lowerPoints);
            faces[facesIndex++] = IfcVertexGeometry.CreatePolygonFace(model, upperPoints);

            return model.Instances.New<IfcFacetedBrep>(brep =>
            {
                brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
            });
        }
    }
}