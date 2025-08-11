using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractCapEntity : IfcAbstractFittingEntity
    {
        public abstract double Diameter { get; }
        
        protected IfcAbstractCapEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateIfcEntity<IfcPipeFitting>(model);
            return pipeFitting;
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.OBSTRUCTION;
            
            IfcRepresentationItem representationItem = IfcGeometry.CreateCylinder(model, Diameter / 2, Length, XbimVector3D.Zero);
            AddShapeRepresentation(model, pipeFitting, representationItem);

            return pipeFitting;
        }
    }
}