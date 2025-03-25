using System;
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
    [IfcEntityType(true, StartElementType.BALL_EXPANSION_JOINT)]
    public class IfcVertexBallExpansionJointEntity : IfcAbstractFittingEntity
    {
        public double Length { get; }
        public double Radius { get; }
        
        private readonly int _numSegments;
        private readonly double _angleStep;
        
        private StartBallExpansionJointEntity _startBallExpansion;
        private IfcPipeFitting _pipeFitting;
        
        public IfcVertexBallExpansionJointEntity(StartBallExpansionJointEntity startBallExpansion, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities, int numSegments) 
            : base(startBallExpansion, ifcNodeEntity, ifcAbstractSegmentEntities)
        {
            _numSegments = numSegments;
            _angleStep = 2 * Math.PI / _numSegments;
            
            _startBallExpansion = startBallExpansion;
            
            Length = _startBallExpansion.Length;
            Radius = Length;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IfcCartesianPoint[,] points = CreateSphere(model);
            IfcFacetedBrep brep = CreateFacetedBrep(model, points);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, brep);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Tag = Tag;
                fitting.Name = _startBallExpansion.Name;
            });
            
            ClipPipes();
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
        
        private IfcFacetedBrep CreateFacetedBrep(IModel model, IfcCartesianPoint[,] points)
        {
            IfcFace[] faces = new IfcFace[_numSegments * _numSegments];
            int facesIndex = 0;
            for (int i = 0; i < _numSegments; i++)
            {
                for (int j = 0; j < _numSegments; j++)
                {
                    IfcCartesianPoint p1 = points[i, j];
                    IfcCartesianPoint p2 = points[i, (j + 1) % _numSegments];
                    IfcCartesianPoint p3 = points[(i + 1) % _numSegments, (j + 1) % _numSegments];
                    IfcCartesianPoint p4 = points[(i + 1) % _numSegments, j];
                    faces[facesIndex++] = IfcVertexGeometry.CreateRectangleFace(model, p1, p2, p3, p4);
                }
            }

            return model.Instances.New<IfcFacetedBrep>(brep =>
            {
                brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
            });
        }
        
        private IfcCartesianPoint[,] CreateSphere(IModel model)
        {
            IfcCartesianPoint[,] points = new IfcCartesianPoint[_numSegments, _numSegments];
            for (int i = 0; i < _numSegments; i++)
            {
                for (int j = 0; j < _numSegments; j++)
                {
                    double x = Radius * Math.Cos(_angleStep * i) * Math.Cos(_angleStep * j);
                    double y = Radius * Math.Cos(_angleStep * i) * Math.Sin(_angleStep * j);
                    double z = Radius * Math.Sin(_angleStep * i);
                    points[i, j] = IfcAxis.CreatePoint(model, new XbimVector3D(x, y, z));
                }
            }

            return points;
        }
        
        private void ClipPipes()
        {
            foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in _IfcAbstractSegmentEntities)
            {
                ifcAbstractSegmentEntity.Clip(IfcNodeEntity, Length / 2);
            }
        }
    }
}