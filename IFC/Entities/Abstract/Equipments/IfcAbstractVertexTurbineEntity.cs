using System.Collections.Generic;
using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Equipments
{
    public abstract class IfcAbstractVertexTurbineEntity : IfcAbstractEquipmentEntity
    {
        public abstract ActionProperty<double>[] Diameters { get; }
        public abstract ActionProperty<int> NumSegments { get; }
        
        public override ActionProperty<Colour> Colour { get; } = Tools.Colour.FromHEX("695689");
        
        protected IfcAbstractVertexTurbineEntity(XbimMatrix3D objectMatrix) : base(objectMatrix) { }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcFan discreteAccessory = CreateIfcEntity<IfcFan>(model);
            return discreteAccessory;
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcFan, IInstantiableEntity
        {
            T chiller = base.CreateIfcEntity<T>(model);
            chiller.PredefinedType = IfcFanTypeEnum.TUBEAXIAL;

            IEnumerable<IfcRepresentationItem> representationItems = CreateFlange(model);
            AddShapeRepresentation(model, chiller, representationItems);
            
            return chiller;
        }
        
        private IEnumerable<IfcFacetedBrep> CreateFlange(IModel model)
        {
            XbimVector3D[] displacements = new XbimVector3D[]
            {
                0.5 * Length * VectorExtensions.Z,
                0.3 * Length * VectorExtensions.Z,
                0.1 * Length * VectorExtensions.Z,
            };
            
            IfcCartesianPoint[][] circles = new IfcCartesianPoint[][]
            {
                IfcGeometry.CreateCircle(model, Diameters[0] * 0.5, displacements[0].Negated(), NumSegments),
                IfcGeometry.CreateCircle(model, Diameters[0] * 0.55, displacements[1].Negated(), NumSegments),
                IfcGeometry.CreateCircle(model, Diameters[0] * 0.75, displacements[1].Negated(), NumSegments),
                IfcGeometry.CreateCircle(model, Diameters[0] * 0.75, displacements[2].Negated(), NumSegments),
                
                IfcGeometry.CreateCircle(model, Diameters[1] * 0.5, displacements[0], NumSegments),
                IfcGeometry.CreateCircle(model, Diameters[1] * 0.55, displacements[1], NumSegments),
                IfcGeometry.CreateCircle(model, Diameters[1] * 0.75, displacements[1], NumSegments),
                IfcGeometry.CreateCircle(model, Diameters[1] * 0.75, displacements[2], NumSegments),
            };

            IfcFacetedBrep[] facetedBreps = new IfcFacetedBrep[]
            {
                IfcGeometry.CreateClippedCone(model, circles[0], circles[1]),
                IfcGeometry.CreateClippedCone(model, circles[1], circles[2]),
                IfcGeometry.CreateClippedCone(model, circles[2], circles[3]),
                IfcGeometry.CreateClippedCone(model, circles[4], circles[5]),
                IfcGeometry.CreateClippedCone(model, circles[5], circles[6]),
                IfcGeometry.CreateClippedCone(model, circles[6], circles[7]),
            };

            return facetedBreps;
        }
    }
}