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
    public abstract class IfcAbstractVertexTorsionExpansionJointEntity : IfcAbstractExpansionJointEntity
    {
        public abstract ActionProperty<double> Diameter { get; }
        public abstract ActionProperty<int> NumSegments { get; }

        protected IfcAbstractVertexTorsionExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
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

            IEnumerable<IfcRepresentationItem> representationItems = CreateShape(model);
            AddShapeRepresentation(model, pipeFitting, representationItems);

            return pipeFitting;
        }

        private IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            double displacementStep = Length / 4;
            XbimVector3D[] displacements = new XbimVector3D[]
            {
                VectorExtensions.Z * 2 * displacementStep,
                VectorExtensions.Z * 1 * displacementStep,
            };
            double[] radiuses = { Diameter * 0.5, Diameter * 0.6, Diameter };

            IfcFacetedBrep[] facetedBreps = new IfcFacetedBrep[]
            {
                IfcGeometry.CreateClippedCone(
                    model, radiuses[1], radiuses[0], displacementStep, 
                    NumSegments, new IfcAxisSettings(displacements[0], VectorExtensions.X, VectorExtensions.Y)
                ),
                IfcGeometry.CreateClippedCone(
                    model, radiuses[2], radiuses[2], displacementStep, 
                    NumSegments, new IfcAxisSettings(displacements[1], VectorExtensions.X, VectorExtensions.Y)
                ),
                IfcGeometry.CreateClippedCone(
                    model, radiuses[2], radiuses[2], displacementStep, 
                    NumSegments, new IfcAxisSettings(displacements[1].Negated(), VectorExtensions.X, VectorExtensions.Y)
                ),
                IfcGeometry.CreateClippedCone(
                    model, radiuses[0], radiuses[1], displacementStep, 
                    NumSegments, new IfcAxisSettings(displacements[0].Negated(), VectorExtensions.X, VectorExtensions.Y)
                ),
            };

            return facetedBreps;
        }
    }
}