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
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractVertexBendEntity : IfcAbstractBendEntity
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double AngleStep { get; protected set; }
        public abstract double BendAngleStep { get; protected set; }

        private readonly StartBendEntity _bendEntity;
        private IfcPipeFitting? _pipeFitting;
        
        public IfcAbstractVertexBendEntity(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(bendEntity, nodeEntity, segmentEntities)
        {
            _bendEntity = bendEntity;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            ObjectMatrix3D = ObjectMatrix3D.Translate(CalculateCircleCenter());
            
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IfcCartesianPoint[,] ifcCartesianPoints = CreatePoints(model);
            IfcFacetedBrep brep = CreateFacetedBrep(model, ifcCartesianPoints);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, brep, IfcRepresentationType.Brep, IfcRepresentationIdentifier.Body);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, brep);
            
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
            IfcCartesianPoint[,] ifcCartesianPoints = new IfcCartesianPoint[NumSegments, NumSegments];
            for (int i = 0; i < NumSegments; i++)
            {
                for (int j = 0; j < NumSegments; j++)
                {
                    double x = (BendRadius + PipeRadius * Math.Cos(j * AngleStep)) * Math.Cos(i * BendAngleStep);
                    double y = PipeRadius * Math.Sin(j * AngleStep);
                    double z = (BendRadius + PipeRadius * Math.Cos(j * AngleStep)) * Math.Sin(i * BendAngleStep);
                    ifcCartesianPoints[i, j] = IfcAxis.CreatePoint(model, new XbimVector3D(x, y, z));
                }
            }

            return ifcCartesianPoints;
        }
        
        private IfcFacetedBrep CreateFacetedBrep(IModel model, IfcCartesianPoint[,] points)
        {
            IfcFace[] faces = new IfcFace[(NumSegments - 1) * NumSegments];
            int facesIndex = 0;
            for (int i = 0; i < NumSegments - 1; i++)
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