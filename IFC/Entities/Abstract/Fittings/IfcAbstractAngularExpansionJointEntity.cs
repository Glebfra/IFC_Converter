using System;
using IFC.Entities.Abstract.Segments;
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

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractVertexAngularExpansionJointEntity : IfcAbstractExpansionJoint
    {
        public abstract double Radius { get; protected set; }
        public abstract double Angle { get; protected set; }
        public abstract int NumSegments { get; protected set; }

        private readonly StartAngularExpansionJointEntity _angularExpansion;
        private IfcPipeFitting? _pipeFitting;
        
        public IfcAbstractVertexAngularExpansionJointEntity(StartAngularExpansionJointEntity angularExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(angularExpansion, nodeEntity, segmentEntities)
        {
            _angularExpansion = angularExpansion;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            XbimMatrix3D My = MatrixExtensions.My(Angle);
            
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            XbimVector3D firstExtrudeDirection = VectorExtensions.Forward.Negated();
            XbimVector3D secondExtrudeDirection = XbimVector3D.Multiply(firstExtrudeDirection, My).Negated();
            
            XbimVector3D firstProfileRefDirection = VectorExtensions.Right;
            XbimVector3D secondProfileRefDirection = XbimVector3D.Multiply(firstProfileRefDirection, My).Negated();

            IfcCartesianPoint[,] points = IfcVertexGeometry.CreateSpherePoints(model, Radius, XbimVector3D.Zero, NumSegments, VectorExtensions.X, VectorExtensions.Y);
            
            IfcRepresentationItem[] extrudedAreaSolids = new IfcRepresentationItem[3];
            extrudedAreaSolids[0] = CreateBranch(model, firstExtrudeDirection, firstProfileRefDirection);
            extrudedAreaSolids[1] = CreateBranch(model, secondExtrudeDirection, secondProfileRefDirection);
            extrudedAreaSolids[2] = CreateFacetedBrep(model, points);
            
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, extrudedAreaSolids);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, extrudedAreaSolids);

            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Tag = Tag;
                fitting.Name = _angularExpansion.Name;
            });
            
            ClipPipes();
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
        
        private IfcExtrudedAreaSolid CreateBranch(IModel model, XbimVector3D extrudeDirection, XbimVector3D refDirection)
        {
            IfcDirection firstExtrudedDirection = IfcAxis.CreateDirection(model, extrudeDirection);
            IfcCircleProfileDef firstProfileDef = IfcGeometry.CreateCircleProfileDef(model, AbstractSegmentEntities[0].Diameter / 2, XbimVector3D.Zero, refDirection);
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
            IfcFace[] faces = new IfcFace[NumSegments * NumSegments];
            int facesIndex = 0;
            for (int i = 0; i < NumSegments; i++)
            {
                for (int j = 0; j < NumSegments; j++)
                {
                    IfcCartesianPoint p1 = points[i, j];
                    IfcCartesianPoint p2 = points[i, (j + 1) % NumSegments];
                    IfcCartesianPoint p3 = points[(i + 1) % NumSegments, (j + 1) % NumSegments];
                    IfcCartesianPoint p4 = points[(i + 1) % NumSegments, j];
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