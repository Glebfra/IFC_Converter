using System;
using IFC.Entities.Abstract;
using IFC.Tools;
using Start.Entities.Segments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Entities.Segments
{
    public sealed class IfcConeElementEntity : IfcAbstractSegmentEntity
    {
        public override XbimVector3D Direction { get; }
        public override double Diameter { get; }
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        
        public double SecondDiameter { get; }

        private const int _numSegments = 32;
        private const double _angleStep = 2 * Math.PI / _numSegments;
        
        private StartConeElementEntity _startConeElementEntity;
        private IfcPipeSegment _pipeSegment;
        
        public IfcConeElementEntity(StartConeElementEntity startConeElementEntity, IfcNodeEntity[] ifcNodeEntities) 
            : base(startConeElementEntity, ifcNodeEntities)
        {
            _startConeElementEntity = startConeElementEntity;
            Coordinates = NodeEntities[0].ObjectMatrix3D.Translation;
            XbimVector3D nodesDirection = ifcNodeEntities[1].ObjectMatrix3D.Translation - Coordinates;
            XbimVector3D pipeProjection = new XbimVector3D(
                startConeElementEntity.ProjectionAlongOXAxis,
                startConeElementEntity.ProjectionAlongOYAxis,
                startConeElementEntity.ProjectionAlongOZAxis
            );
            Direction = (pipeProjection * XbimVector3D.DotProduct(nodesDirection, pipeProjection)).Normalized() * pipeProjection.Length;
            Length = Direction.Length;
            Direction = Direction.Normalized();
            
            XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
            XbimVector3D forward = Direction.Normalized();
            if (forward == WorldUp || forward == -1 * WorldUp) 
                WorldUp = new XbimVector3D(0, 1, 0);
            XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);
            
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(Coordinates, forward, up);
            
            Diameter = startConeElementEntity.Diameter;
            SecondDiameter = startConeElementEntity.SecondDiameter;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            IfcCartesianPoint[] firstCircle = CreateCircle(model, Diameter / 2, 0);
            IfcCartesianPoint[] secondCircle = CreateCircle(model, SecondDiameter / 2, Length);
            IfcFacetedBrep facetedBrep = CreateFacetedBrep(model, firstCircle, secondCircle);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, facetedBrep);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);

            _pipeSegment = model.Instances.New<IfcPipeSegment>(segment =>
            {
                segment.Tag = Tag;
                segment.Name = _startConeElementEntity.Name;
                segment.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
                segment.ObjectPlacement = objectPlacement.LocalPlacement;
                segment.Representation = shape;
            });
            AddProperties(model, _pipeSegment);

            return _pipeSegment;
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