using System;
using IFC.Entities.Abstract;
using IFC.Tools;
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
    public sealed class IfcVertexBendEntity : IfcAbstractBendEntity
    {
        private readonly int _numSegments;
        private readonly double _angleStep;
        private readonly double _torusAngleStep;
        private readonly double _torusRadius;
        private readonly double _circleRadius;
        
        private StartBendEntity _bendEntity;
        private IfcPipeFitting _pipeFitting;

        public IfcVertexBendEntity(StartBendEntity bendEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities, params object[] args) 
            : base(bendEntity, ifcNodeEntity, ifcAbstractSegmentEntities)
        {
            _numSegments = args[0] is int ? (int)args[0] : 0;
            _angleStep = 2 * Math.PI / _numSegments;
            
            _bendEntity = bendEntity;

            _torusAngleStep = Angle / (_numSegments - 1);
            _torusRadius = _bendEntity.Radius;
            _circleRadius = Math.Min(_IfcAbstractSegmentEntities[0].Diameter / 2, _IfcAbstractSegmentEntities[1].Diameter / 2);
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IfcCartesianPoint[,] ifcCartesianPoints = CreatePoints(model);
            IfcFacetedBrep brep = CreateFacetedBrep(model, ifcCartesianPoints);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, brep);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _bendEntity.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });

            AddProperties(model, _pipeFitting);
            ClipConnectedPipes();

            return _pipeFitting;
        }
        
        private IfcCartesianPoint[,] CreatePoints(IModel model)
        {
            IfcCartesianPoint[,] ifcCartesianPoints = new IfcCartesianPoint[_numSegments, _numSegments];
            for (int i = 0; i < _numSegments; i++)
            {
                for (int j = 0; j < _numSegments; j++)
                {
                    double x = (_torusRadius + _circleRadius * Math.Cos(j * _angleStep)) * Math.Cos(i * _torusAngleStep);
                    double y = _circleRadius * Math.Sin(j * _angleStep);
                    double z = (_torusRadius + _circleRadius * Math.Cos(j * _angleStep)) * Math.Sin(i * _torusAngleStep);
                    ifcCartesianPoints[i, j] = IfcAxis.CreatePoint(model, new XbimVector3D(x, y, z));
                }
            }

            return ifcCartesianPoints;
        }
        
        private IfcFacetedBrep CreateFacetedBrep(IModel model, IfcCartesianPoint[,] points)
        {
            IfcFace[] faces = new IfcFace[(_numSegments - 1) * _numSegments];
            int facesIndex = 0;
            for (int i = 0; i < _numSegments - 1; i++)
            {
                for (int j = 0; j < _numSegments; j++)
                {
                    IfcCartesianPoint p1 = points[i, j];
                    IfcCartesianPoint p2 = points[i, (j + 1) % _numSegments];
                    IfcCartesianPoint p3 = points[i + 1, (j + 1) % _numSegments];
                    IfcCartesianPoint p4 = points[i + 1, j];
                    faces[facesIndex++] = IfcVertexGeometry.CreateRectangleFace(model, p1, p2, p3, p4);
                }
            }

            return model.Instances.New<IfcFacetedBrep>(brep =>
            {
                brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
            });
        }
    }
}