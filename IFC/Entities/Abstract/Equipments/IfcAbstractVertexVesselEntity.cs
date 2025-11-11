using System.Collections.Generic;
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
    public abstract class IfcAbstractVertexVesselEntity : IfcAbstractEquipmentEntity
    {
        public abstract ActionProperty<int> NumSegments { get; }
        public abstract ActionProperty<double> Diameter { get; }

        public override ActionProperty<Colour> Colour { get; } = Tools.Colour.FromHEX("695689");
        
        protected IfcAbstractVertexVesselEntity(XbimMatrix3D objectMatrix) : base(objectMatrix) { }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcTank discreteAccessory = CreateIfcEntity<IfcTank>(model);
            return discreteAccessory;
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcTank, IInstantiableEntity
        {
            T chiller = base.CreateIfcEntity<T>(model);
            chiller.PredefinedType = IfcTankTypeEnum.VESSEL;
            
            IEnumerable<IfcRepresentationItem> representationItems = CreateFlange(model);
            AddShapeRepresentation(model, chiller, representationItems);
            
            return chiller;
        }

        private IEnumerable<IfcRepresentationItem> CreateFlange(IModel model)
        {
            XbimVector3D firstCircleDisplacement = Length / 2 * VectorExtensions.Forward.Negated();
            
            IfcCartesianPoint[][] circles = new IfcCartesianPoint[][]
            {
                IfcGeometry.CreateCircle(model, Diameter * 0.66, firstCircleDisplacement, NumSegments),
                IfcGeometry.CreateCircle(model, Diameter * 0.66, XbimVector3D.Zero, NumSegments),
                IfcGeometry.CreateCircle(model, Diameter * 0.55, XbimVector3D.Zero, NumSegments),
                IfcGeometry.CreateCircle(model, Diameter * 0.5, firstCircleDisplacement.Negated(), NumSegments)
            };

            return new IfcRepresentationItem[]
            {
                IfcGeometry.CreateClippedCone(model, circles[0], circles[1]),
                IfcGeometry.CreateClippedCone(model, circles[2], circles[3])
            };
        }
    }
}