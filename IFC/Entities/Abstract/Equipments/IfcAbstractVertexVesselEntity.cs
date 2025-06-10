using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using IFC.Tools.Geometry;
using IFC.Tools.Shape;
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
    public abstract class IfcAbstractVertexVesselEntity : IfcAbstractEquipmentEntity
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double Diameter { get; protected set; }

        public override Colour Colour { get; protected set; } = Colour.FromHEX("695689");

        private readonly StartVesselEntity _vessel;
        private IfcTank? _tank;
        
        protected IfcAbstractVertexVesselEntity(StartVesselEntity vessel, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(vessel, nodeEntity, segmentEntities)
        {
            _vessel = vessel;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[2];
            XbimVector3D firstCircleDisplacement = Length / 2 * VectorExtensions.Forward.Negated();
            
            IfcCartesianPoint[][] circles = new IfcCartesianPoint[][]
            {
                IfcVertexGeometry.CreateCircle(model, Diameter * 0.66, firstCircleDisplacement, NumSegments),
                IfcVertexGeometry.CreateCircle(model, Diameter * 0.66, XbimVector3D.Zero, NumSegments),
                IfcVertexGeometry.CreateCircle(model, Diameter * 0.55, XbimVector3D.Zero, NumSegments),
                IfcVertexGeometry.CreateCircle(model, Diameter * 0.5, firstCircleDisplacement.Negated(), NumSegments)
            };
            
            representationItems[0] = IfcVertexGeometry.CreateClippedCone(model, circles[0], circles[1]);
            representationItems[1] = IfcVertexGeometry.CreateClippedCone(model, circles[2], circles[3]);
            
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, representationItems, IfcRepresentationType.Brep, IfcRepresentationIdentifier.Body);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, representationItems);
            
            _tank = model.Instances.New<IfcTank>(fitting =>
            {
                fitting.Name = _vessel.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcTankTypeEnum.VESSEL;
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });

            AddProperties(model, _tank);

            return _tank;
        }
    }
}