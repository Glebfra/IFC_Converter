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
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Fittings.Vertex
{
    public class IfcVertexValveEntity : IfcAbstractFittingEntity
    {
        public sealed override double Length { get; protected set; }

        private readonly int _numSegments;
        
        private readonly StartArmatureEntity _armatureEntity;
        private IfcPipeFitting _pipeFitting;

        public IfcVertexValveEntity(StartArmatureEntity armatureEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments)
            : base(armatureEntity, nodeEntity, abstractSegmentEntities)
        {
            _armatureEntity = armatureEntity;
            _numSegments = numSegments;
            
            Length = _armatureEntity.Length.SIProperty;
            Diameter = Math.Max(abstractSegmentEntities[0].OuterDiameter, abstractSegmentEntities[1].OuterDiameter) * 1.5;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            XbimVector3D displacement = Length / 2 * VectorExtensions.Forward;

            IfcCartesianPoint[] firstCircle = IfcVertexGeometry.CreateCircle(model, Diameter / 2, displacement.Negated(), _numSegments);
            IfcCartesianPoint[] secondCircle = IfcVertexGeometry.CreateCircle(model, Diameter / 2, displacement, _numSegments);
            foreach (IfcCartesianPoint secondCirclePoint in secondCircle)
                secondCirclePoint.RotateAroundYAxis(Angle);

            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, XbimVector3D.Zero);
            IfcFacetedBrep lowerBrep = IfcVertexGeometry.CreateCone(model, firstCircle, topPoint);
            IfcFacetedBrep upperBrep = IfcVertexGeometry.CreateCone(model, secondCircle, topPoint);
            IfcBooleanResult result = IfcGeometry.CreateBooleanResult(model, lowerBrep, upperBrep, IfcBooleanOperator.UNION);
            
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, result);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Name = _armatureEntity.Name;
                fitting.Representation = shape;
                fitting.Tag = Tag;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            AbstractSegmentEntities[0].Clip(NodeEntity, Length / 2);
            AbstractSegmentEntities[1].Clip(NodeEntity, Length / 2);
        
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
    }
}