using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using IFC.Tools.Geometry;
using IFC.Tools.Shape;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractVertexValveEntity : IfcAbstractFittingEntity
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double Diameter { get; protected set; }
        public abstract double Angle { get; protected set; }

        private readonly StartArmatureEntity _armatureEntity;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractVertexValveEntity(StartArmatureEntity armatureEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(armatureEntity, nodeEntity, segmentEntities)
        {
            _armatureEntity = armatureEntity;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            XbimVector3D displacement = Length / 2 * VectorExtensions.Forward;

            IfcCartesianPoint[] firstCircle = IfcVertexGeometry.CreateCircle(model, Diameter / 2, displacement.Negated(), NumSegments);
            IfcCartesianPoint[] secondCircle = IfcVertexGeometry.CreateCircle(model, Diameter / 2, displacement, NumSegments);
            foreach (IfcCartesianPoint secondCirclePoint in secondCircle)
                secondCirclePoint.RotateAroundYAxis(Angle);

            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, XbimVector3D.Zero);
            IfcFacetedBrep lowerBrep = IfcVertexGeometry.CreateCone(model, firstCircle, topPoint);
            IfcFacetedBrep upperBrep = IfcVertexGeometry.CreateCone(model, secondCircle, topPoint);
            IfcBooleanResult result = IfcGeometry.CreateBooleanResult(model, lowerBrep, upperBrep, IfcBooleanOperator.UNION);
            ColourEntity(model, result);
            
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, result, IfcRepresentationType.Brep, IfcRepresentationIdentifier.Body);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Name = _armatureEntity.Name;
                fitting.Representation = shape;
                fitting.Tag = Tag;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            
            AddProperties(model, _pipeFitting);
            ClipPipes();

            return _pipeFitting;
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