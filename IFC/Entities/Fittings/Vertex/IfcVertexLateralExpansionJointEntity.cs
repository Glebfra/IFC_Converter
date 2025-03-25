using System;
using IFC.Entities.Abstract;
using IFC.Extensions;
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
    [IfcEntityType(true, StartElementType.LATERAL_EXPANSION_JOINT)]
    public class IfcVertexLateralExpansionJointEntity : IfcAbstractFittingEntity
    {
        public double Length { get; }
        
        private readonly int _numSegments;
        private readonly double _angleStep;
        private readonly double _radius;
        
        private StartLateralExpansionJointEntity _lateralExpansionJoint;
        private IfcPipeFitting _pipeFitting;
        
        public IfcVertexLateralExpansionJointEntity(StartLateralExpansionJointEntity lateralExpansionJoint, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities, int numSegments) 
            : base(lateralExpansionJoint, ifcNodeEntity, ifcAbstractSegmentEntities)
        {
            _numSegments = numSegments;
            _angleStep = 2 * Math.PI / _numSegments;

            _lateralExpansionJoint = lateralExpansionJoint;

            Length = _lateralExpansionJoint.Length;
            _radius = Length;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            XbimVector3D firstDisplacement = VectorExtensions.Forward.Negated() * (Length / 2);
            IfcCartesianPoint[,] firstSphere = CreateSphere(model, firstDisplacement, 0);
            XbimVector3D secondDisplacement = XbimVector3D.Multiply(VectorExtensions.Forward * (Length / 2), MatrixExtensions.My(Angle));
            IfcCartesianPoint[,] secondSphere = CreateSphere(model, secondDisplacement, Angle);

            IfcFacetedBrep[] brep = new IfcFacetedBrep[2];
            brep[0] = CreateFacetedBrep(model, firstSphere);
            brep[1] = CreateFacetedBrep(model, secondSphere);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, brep);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _lateralExpansionJoint.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
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

        private IfcCartesianPoint[,] CreateSphere(IModel model, XbimVector3D point, double angle)
        {
            IfcCartesianPoint[,] points = new IfcCartesianPoint[_numSegments, _numSegments];
            for (int i = 0; i < _numSegments; i++)
            {
                for (int j = 0; j < _numSegments; j++)
                {
                    double x = point.X + _radius * Math.Cos(_angleStep * i) * Math.Cos(_angleStep * j);
                    double y = point.Y + _radius * Math.Cos(_angleStep * i) * Math.Sin(_angleStep * j);
                    double z = point.Z + _radius * Math.Sin(_angleStep * i);
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