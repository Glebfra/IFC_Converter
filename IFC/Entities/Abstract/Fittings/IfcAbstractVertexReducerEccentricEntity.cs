using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract.Segments;
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
    public abstract class IfcAbstractVertexReducerEccentricEntity : IfcAbstractReducerEntity
    {
        public abstract double DisplacementLength { get; }
        public abstract ActionProperty<double>[] Diameters { get; }
        public abstract ActionProperty<int> NumSegments { get; }

        protected IfcAbstractVertexReducerEccentricEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

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
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.TRANSITION;

            IEnumerable<IfcRepresentationItem> representationItems = CreateShape(model);
            AddShapeRepresentation(model, pipeFitting, representationItems);

            return pipeFitting;
        }

        private IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            IfcAxisSettings axisSettings = new IfcAxisSettings(VectorExtensions.X, VectorExtensions.Y);
            XbimVector3D displacement = VectorExtensions.Y.Negated() * DisplacementLength;
            IfcFacetedBrep facetedBrep = IfcVertexGeometry.CreateClippedCone(model, Diameters[0] / 2, Diameters[1] / 2, Length, NumSegments, axisSettings, displacement);
            return new IfcRepresentationItem[] { facetedBrep };
        }

        private void ClipPipes()
        {
            IfcAbstractSegmentEntity[] segmentEntities = ConnectedEntities.OfType<IfcAbstractSegmentEntity>().ToArray();
            segmentEntities[1].Clip(NodeEntity, Length);
        }
    }
}