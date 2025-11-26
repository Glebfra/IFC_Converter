using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Equipments
{
    public abstract class IfcAbstractVertexInlinePumpEntity : IfcAbstractEquipmentEntity
    {
        public abstract ActionProperty<int> NumSegments { get; }
        public abstract ActionProperty<double> Angle { get; }
        public abstract ActionProperty<double> Diameter { get; }

        public override ActionProperty<Colour> Colour { get; } = Tools.Colour.FromHEX("5b1c6a");
        
        protected IfcAbstractVertexInlinePumpEntity(XbimMatrix3D objectMatrix) : base(objectMatrix) { }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPump discreteAccessory = CreateIfcEntity<IfcPump>(model);
            return discreteAccessory;
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPump, IInstantiableEntity
        {
            T pump = base.CreateIfcEntity<T>(model);
            pump.PredefinedType = IfcPumpTypeEnum.SUMPPUMP;
            
            XbimVector3D displacement = Length / 2 * VectorExtensions.Forward;
            IfcCartesianPoint[] firstCircle = IfcGeometry.CreateCircle(model, Diameter / 2, displacement.Negated(), NumSegments);
            IfcCartesianPoint[] secondCircle = IfcGeometry.CreateCircle(model, Diameter / 2, displacement, NumSegments);
            foreach (IfcCartesianPoint secondCirclePoint in secondCircle)
                secondCirclePoint.RotateAroundYAxis(Angle);
            
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, XbimVector3D.Zero);

            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[]
            {
                IfcGeometry.CreateCone(model, firstCircle, topPoint),
                IfcGeometry.CreateCone(model, secondCircle, topPoint)
            };
            AddShapeRepresentation(model, pump, representationItems);
            
            return pump;
        }
    }
}