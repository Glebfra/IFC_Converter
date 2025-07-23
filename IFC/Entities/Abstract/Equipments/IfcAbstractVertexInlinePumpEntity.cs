using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Equipments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Equipments
{
    #if NEW
    
    
    
    #else
    
    public abstract class IfcAbstractVertexInlinePumpEntity : IfcAbstractEquipmentEntity
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double Angle { get; protected set; }
        public abstract double Diameter { get; protected set; }

        public override Colour Colour { get; protected set; } = Colour.FromHEX("5b1c6a");

        private readonly StartInlinePumpEntity _inlinePump;
        private IfcPump? _pump;
        
        protected IfcAbstractVertexInlinePumpEntity(StartInlinePumpEntity inlinePump, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(inlinePump, nodeEntity, segmentEntities)
        {
            _inlinePump = inlinePump;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[2];
            
            XbimVector3D displacement = Length / 2 * VectorExtensions.Forward;
            
            IfcCartesianPoint[] firstCircle = IfcVertexGeometry.CreateCircle(model, Diameter / 2, displacement.Negated(), NumSegments);
            IfcCartesianPoint[] secondCircle = IfcVertexGeometry.CreateCircle(model, Diameter / 2, displacement, NumSegments);
            foreach (IfcCartesianPoint secondCirclePoint in secondCircle)
                secondCirclePoint.RotateAroundYAxis(Angle);
            
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, XbimVector3D.Zero);
            
            representationItems[0] = IfcVertexGeometry.CreateCone(model, firstCircle, topPoint);
            representationItems[1] = IfcVertexGeometry.CreateCone(model, secondCircle, topPoint);
            
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, representationItems);
            
            _pump = model.Instances.New<IfcPump>(fitting =>
            {
                fitting.PredefinedType = IfcPumpTypeEnum.SUMPPUMP;
                fitting.Name = _inlinePump.Name;
                fitting.Representation = shape;
                fitting.Tag = Tag;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });

            AddProperties(model, _pump);
            ClipPipes();

            return _pump;
        }

        private void ClipPipes()
        {
            foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in AbstractSegmentEntities)
            {
                ifcAbstractSegmentEntity.Clip(NodeEntity, Length / 2);
            }
        }
    }

    #endif
}