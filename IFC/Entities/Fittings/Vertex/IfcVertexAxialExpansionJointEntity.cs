using System;
using IFC.Entities.Abstract;
using IFC.Tools;
using Start.Entities.Fittings;
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
    public sealed class IfcVertexAxialExpansionJointEntity : IfcAbstractAxialExpansionJointEntity
    {
        private readonly int _numSegments;
        private readonly double _angleStep;

        private StartAxialExpansionJointEntity _startAxialExpansionJointEntity;
        private IfcPipeFitting _pipeFitting;
        
        public IfcVertexAxialExpansionJointEntity(
            StartAxialExpansionJointEntity startAxialExpansionJointEntity, IfcNodeEntity ifcNodeEntity, 
            IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities, int numSegments) 
            : base(startAxialExpansionJointEntity, ifcNodeEntity, ifcAbstractSegmentEntities)
        {
            _numSegments = numSegments;
            _angleStep = 2 * Math.PI / _numSegments;
            _startAxialExpansionJointEntity = startAxialExpansionJointEntity;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            IfcCartesianPoint[] firstCircle = CreateCircle(model, _Radiuses[0], -Length / 2);
            IfcCartesianPoint[] secondCircle = CreateCircle(model, _Radiuses[0], 0);
            IfcCartesianPoint[] thirdCircle = CreateCircle(model, _Radiuses[1], 0);
            IfcCartesianPoint[] fourthCircle = CreateCircle(model, _Radiuses[1], Length / 2);

            IfcFacetedBrep[] facetedBreps = new IfcFacetedBrep[]
            {
                CreateFacetedBrep(model, firstCircle, secondCircle),
                CreateFacetedBrep(model, secondCircle, thirdCircle),
                CreateFacetedBrep(model, thirdCircle, fourthCircle)
            };
            
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, facetedBreps);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Tag = Tag;
                fitting.Name = _startAxialExpansionJointEntity.Name;
            });
            ClipPipes();
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