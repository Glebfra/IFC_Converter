using System;
using System.Linq;
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

namespace IFC.Entities.Fittings
{
    public sealed class IfcReducerEccentricEntity : IfcAbstractFittingEntity
    {
        public double Length { get; }
        
        private const int _numSegments = 32;
        private const double _angleStep = 2 * Math.PI / _numSegments;
        private readonly double _pipeDisplacement;
        private readonly StartReducerEntity _reducerEntity;
        private IfcPipeFitting _pipeFitting;

        public IfcReducerEccentricEntity(StartReducerEntity reducerEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities)
            : base(reducerEntity, nodeEntity, ifcAbstractSegmentEntities)
        {
            _reducerEntity = reducerEntity;
            _IfcAbstractSegmentEntities = _IfcAbstractSegmentEntities
                .OrderBy(entity => entity.Diameter)
                .ToArray();

            XbimVector3D coordinates = IfcNodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = CalculateForwardVector();
            XbimVector3D up = CalculateUpVector();
            _pipeDisplacement = up.Length;
            up = up.Normalized();
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
            
            Length = _reducerEntity.LengthOfConicalPart;
        }
    
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IfcCartesianPoint[] lowerCircle = CreateCircle(model, _IfcAbstractSegmentEntities[0].Diameter / 2, 0, 0);
            IfcCartesianPoint[] upperCircle = CreateCircle(model, _IfcAbstractSegmentEntities[1].Diameter / 2, Length, _pipeDisplacement);
            IfcFacetedBrep facetedBrep = CreateFacetedBrep(model, lowerCircle, upperCircle);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, facetedBrep);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.TRANSITION;
                fitting.Tag = Tag;
                fitting.Name = _reducerEntity.Name;
            });
            _IfcAbstractSegmentEntities[1].Clip(IfcNodeEntity, Length);
            
            MovePipe(_IfcAbstractSegmentEntities[1]);
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }

        private XbimVector3D CalculateUpVector()
        {
            XbimVector3D coordinates = IfcNodeEntity.ObjectMatrix3D.Translation;
            IfcNodeEntity[] pipeNodeEntities = _IfcAbstractSegmentEntities[1].NodeEntities
                .OrderBy(entity => (entity.ObjectMatrix3D.Translation - coordinates).Length)
                .ToArray();
            return (
                pipeNodeEntities[1].ObjectMatrix3D.Translation - 
                pipeNodeEntities[0].ObjectMatrix3D.Translation - 
                IfcAxis.GetDirectionToPipe(_IfcAbstractSegmentEntities[1], coordinates)
            );
        }

        private XbimVector3D CalculateForwardVector()
        {
            XbimVector3D coordinates = IfcNodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D directionToPipe = IfcAxis.GetDirectionToPipe(_IfcAbstractSegmentEntities[1], coordinates);
            return directionToPipe.Normalized();
        }

        private IfcCartesianPoint[] CreateCircle(IModel model, double radius, double height, double displacement)
        {
            IfcCartesianPoint[] points = new IfcCartesianPoint[_numSegments];
            for (int i = 0; i < _numSegments; i++)
            {
                XbimVector3D point = new XbimVector3D(
                    radius * Math.Cos(_angleStep * i),
                    radius * Math.Sin(_angleStep * i) - displacement,
                    height
                );
                points[i] = IfcAxis.CreatePoint(model, point);
            }

            return points;
        }
        
        private static IfcFacetedBrep CreateFacetedBrep(IModel model, IfcCartesianPoint[] lowerPoints, IfcCartesianPoint[] upperPoints)
        {
            IfcFace[] faces = new IfcFace[_numSegments + 2];
            int facesIndex = 0;
            for (int i = 0; i < _numSegments; i++)
            {
                IfcCartesianPoint p1 = lowerPoints[i];
                IfcCartesianPoint p2 = lowerPoints[(i + 1) % _numSegments];
                IfcCartesianPoint p3 = upperPoints[(i + 1) % _numSegments];
                IfcCartesianPoint p4 = upperPoints[i];
                faces[facesIndex++] = IfcGeometry.CreateRectangleFace(model, p1, p2, p3, p4);
            }
            faces[facesIndex++] = IfcGeometry.CreatePolygonFace(model, lowerPoints);
            faces[facesIndex++] = IfcGeometry.CreatePolygonFace(model, upperPoints);

            return model.Instances.New<IfcFacetedBrep>(brep =>
            {
                brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
            });
        }
        
        private void MovePipe(IfcAbstractSegmentEntity ifcAbstractSegmentEntity)
        {
            if ((ifcAbstractSegmentEntity.NodeEntities[0].ObjectMatrix3D.Translation - IfcNodeEntity.ObjectMatrix3D.Translation).Length <
                (ifcAbstractSegmentEntity.NodeEntities[1].ObjectMatrix3D.Translation - IfcNodeEntity.ObjectMatrix3D.Translation).Length)
            {
                ifcAbstractSegmentEntity.Coordinates += ObjectMatrix3D.Up * _pipeDisplacement;
            }
        }
    }
}