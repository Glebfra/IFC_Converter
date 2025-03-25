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
    [IfcEntityType(true, StartElementType.TORSION_EXPANSION_JOINT)]
    public class IfcVertexTorsionExpansionJointEntity : IfcAbstractFittingEntity
    {
        public double Length { get; }
        public double Radius { get; }
        
        private readonly int _numSegments;
        private readonly double _angleStep;
        
        private StartTorsionExpansionJointEntity _torsionExpansionJoint;
        private IfcPipeFitting _pipeFitting;
        
        public IfcVertexTorsionExpansionJointEntity(StartTorsionExpansionJointEntity torsionExpansionJoint, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities, int numSegments) 
            : base(torsionExpansionJoint, ifcNodeEntity, ifcAbstractSegmentEntities)
        {
            _numSegments = numSegments;
            _angleStep = 2 * Math.PI / _numSegments;

            _torsionExpansionJoint = torsionExpansionJoint;
            
            Radius = Diameter / 2;
            Length = _torsionExpansionJoint.Length;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            double displacementStep = Length / 5;
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IfcCartesianPoint[][] cartesianPoints = new IfcCartesianPoint[8][];
            cartesianPoints[0] = CreateCircle(model, Radius, 2.5 * displacementStep);
            cartesianPoints[1] = CreateCircle(model, Radius * 1.2, 1.5 * displacementStep);
            cartesianPoints[2] = CreateCircle(model, Radius * 2, 1.5 * displacementStep);
            cartesianPoints[3] = CreateCircle(model, Radius * 2, 0.5 * displacementStep);
            cartesianPoints[4] = CreateCircle(model, Radius * 2, -0.5 * displacementStep);
            cartesianPoints[5] = CreateCircle(model, Radius * 2, -1.5 * displacementStep);
            cartesianPoints[6] = CreateCircle(model, Radius * 1.2, -1.5 * displacementStep);
            cartesianPoints[7] = CreateCircle(model, Radius, -2.5 * displacementStep);
            
            IfcFacetedBrep[] brep = new IfcFacetedBrep[4];
            brep[0] = CreateFacetedBrep(model, cartesianPoints[0], cartesianPoints[1]);
            brep[1] = CreateFacetedBrep(model, cartesianPoints[2], cartesianPoints[3]);
            brep[2] = CreateFacetedBrep(model, cartesianPoints[4], cartesianPoints[5]);
            brep[3] = CreateFacetedBrep(model, cartesianPoints[6], cartesianPoints[7]);
            
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, brep);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Name = _torsionExpansionJoint.Name;
                fitting.Tag = Tag;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
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
                double x = radius * Math.Cos(_angleStep * i);
                double y = radius * Math.Sin(_angleStep * i);
                points[i] = IfcAxis.CreatePoint(model, new XbimVector3D(x, y, height));
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
        
        private void ClipPipes()
        {
            foreach (IfcAbstractSegmentEntity ifcPipeEntity in _IfcAbstractSegmentEntities)
            {
                ifcPipeEntity.Clip(IfcNodeEntity, Length / 2);
            }
        }
    }
}