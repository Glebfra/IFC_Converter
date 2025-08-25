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

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractVertexLateralExpansionJointEntity : IfcAbstractExpansionJointEntity
    {
        public abstract ActionProperty<double> Diameter { get; }
        public abstract ActionProperty<double> Angle { get; }
        public abstract ActionProperty<int> NumSegments { get; }
        
        protected IfcAbstractVertexLateralExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

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
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;

            IEnumerable<IfcRepresentationItem> representationItems = CreateShape(model);
            AddShapeRepresentation(model, pipeFitting, representationItems);

            return pipeFitting;
        }

        private IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            XbimVector3D firstDisplacement = VectorExtensions.Forward.Negated() * (Length / 2);
            XbimVector3D secondDisplacement = XbimVector3D.Multiply(VectorExtensions.Forward * (Length / 2), MatrixExtensions.My(Angle));

            IfcFacetedBrep[] brep = new IfcFacetedBrep[]
            {
                IfcVertexGeometry.CreateSphere(model, Diameter / 2, firstDisplacement, NumSegments, VectorExtensions.X, VectorExtensions.Y),
                IfcVertexGeometry.CreateSphere(model, Diameter / 2, secondDisplacement, NumSegments, VectorExtensions.X, VectorExtensions.Y)
            };

            return brep;
        }
    }
}