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
        public int NumSegments;
        public double AngleStep;
        public double CircleRadius;
        public int TorusNumSegments;
        public double TorusAngleStep;
        public double TorusRadius;
        
        private StartBendEntity _bendEntity;
        private IfcPipeFitting _pipeFitting;

        public IfcVertexBendEntity(StartBendEntity bendEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities) 
            : base(bendEntity, ifcNodeEntity, ifcAbstractSegmentEntities)
        {
            _bendEntity = bendEntity;

            NumSegments = 32;
            AngleStep = Angle / AngleStep;
            TorusNumSegments = 10;
            TorusAngleStep = Angle / (TorusNumSegments - 1);
            TorusRadius = _bendEntity.Radius;
            CircleRadius = Math.Min(_IfcAbstractSegmentEntities[0].Diameter / 2, _IfcAbstractSegmentEntities[1].Diameter / 2);
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
            IfcCartesianPoint[,] ifcCartesianPoints = new IfcCartesianPoint[TorusNumSegments, NumSegments];
            for (int i = 0; i < TorusNumSegments; i++)
            {
                for (int j = 0; j < NumSegments; j++)
                {
                    double x = (TorusRadius + CircleRadius * Math.Cos(j * AngleStep)) * Math.Cos(i * TorusAngleStep);
                    double y = CircleRadius * Math.Sin(j * AngleStep);
                    double z = (TorusRadius + CircleRadius * Math.Cos(j * AngleStep)) * Math.Sin(i * TorusAngleStep);
                    ifcCartesianPoints[i, j] = IfcAxis.CreatePoint(model, new XbimVector3D(x, y, z));
                }
            }

            return ifcCartesianPoints;
        }
        
        private IfcFacetedBrep CreateFacetedBrep(IModel model, IfcCartesianPoint[,] points)
        {
            IfcFace[] faces = new IfcFace[(TorusNumSegments - 1) * NumSegments];
            int facesIndex = 0;
            for (int i = 0; i < TorusNumSegments - 1; i++)
            {
                for (int j = 0; j < NumSegments; j++)
                {
                    IfcCartesianPoint p1 = points[i, j];
                    IfcCartesianPoint p2 = points[i, (j + 1) % NumSegments];
                    IfcCartesianPoint p3 = points[i + 1, (j + 1) % NumSegments];
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