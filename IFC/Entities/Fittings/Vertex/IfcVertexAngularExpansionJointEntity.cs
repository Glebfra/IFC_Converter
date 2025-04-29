using System;
using IFC.Entities.Abstract;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Entities.Fittings.Vertex
{
    public class IfcVertexAngularExpansionJointEntity : IfcAbstractFittingEntity
    {
        public double Radius { get; }
        public sealed override double Length { get; protected set; }

        private readonly int _numSegments;
        private readonly double _angleStep;

        private StartAngularExpansionJointEntity _startAngularExpansion;
        private IfcPipeFitting _pipeFitting;
        
        public IfcVertexAngularExpansionJointEntity(StartAngularExpansionJointEntity startAngularExpansion, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(startAngularExpansion, ifcNodeEntity, abstractSegmentEntities)
        {
            _numSegments = numSegments;
            _angleStep = 2 * Math.PI / _numSegments;
            
            _startAngularExpansion = startAngularExpansion;

            Length = _startAngularExpansion.Length;
            Radius = Length / 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            XbimMatrix3D My = MatrixExtensions.My(Angle);
            
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            XbimVector3D firstExtrudeDirection = VectorExtensions.Forward.Negated();
            XbimVector3D secondExtrudeDirection = XbimVector3D.Multiply(firstExtrudeDirection, My).Negated();
            
            XbimVector3D firstProfileRefDirection = VectorExtensions.Right;
            XbimVector3D secondProfileRefDirection = XbimVector3D.Multiply(firstProfileRefDirection, My).Negated();

            IfcCartesianPoint[,] points = CreateSphere(model);
            
            IfcRepresentationItem[] extrudedAreaSolids = new IfcRepresentationItem[3];
            extrudedAreaSolids[0] = CreateBranch(model, firstExtrudeDirection, firstProfileRefDirection);
            extrudedAreaSolids[1] = CreateBranch(model, secondExtrudeDirection, secondProfileRefDirection);
            extrudedAreaSolids[2] = CreateFacetedBrep(model, points);
            
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, extrudedAreaSolids);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);

            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Tag = Tag;
                fitting.Name = _startAngularExpansion.Name;
            });
            
            ClipPipes();
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }

        private IfcExtrudedAreaSolid CreateBranch(IModel model, XbimVector3D extrudeDirection, XbimVector3D refDirection)
        {
            IfcDirection firstExtrudedDirection = IfcAxis.CreateDirection(model, extrudeDirection);
            IfcCircleProfileDef firstProfileDef = IfcGeometry.CreateCircleProfileDef(model, AbstractSegmentEntities[0].OuterDiameter / 2, XbimVector3D.Zero, refDirection);
            return CreateExtrudedArea(model, firstProfileDef, firstExtrudedDirection, Length / 2);
        }
        
        private IfcExtrudedAreaSolid CreateExtrudedArea(IModel model, IfcProfileDef profileDef, IfcDirection direction, double length)
        {
            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.Depth = length;
                solid.ExtrudedDirection = direction;
                solid.SweptArea = profileDef;
            });
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
            foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in AbstractSegmentEntities)
            {
                ifcAbstractSegmentEntity.Clip(NodeEntity, Length / 2);
            }
        }
    }
}