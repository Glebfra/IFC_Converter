using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractVertexBallExpansionJointEntity : IfcAbstractExpansionJointEntity
    {
        public abstract int NumSegments { get; }
        public abstract double Diameter { get; }
        
        protected IfcAbstractVertexBallExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateIfcEntity<IfcPipeFitting>(model);
            ClipPipes();
            return pipeFitting;
        }
        
        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;

            IfcFacetedBrep brep = IfcVertexGeometry.CreateSphere(model, Diameter / 2, XbimVector3D.Zero, NumSegments, VectorExtensions.X, VectorExtensions.Y);
            AddShapeRepresentation(model, pipeFitting, brep);

            return pipeFitting;
        }
    }
}