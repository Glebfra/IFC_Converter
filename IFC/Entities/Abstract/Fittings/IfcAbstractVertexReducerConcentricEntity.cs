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
    public abstract class IfcAbstractVertexReducerConcentricEntity : IfcAbstractReducerEntity
    {
        public abstract ActionProperty<int> NumSegments { get; }

        protected IfcAbstractVertexReducerConcentricEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
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
            IfcAxisSettings axisSettings = new IfcAxisSettings(XbimVector3D.Zero, VectorExtensions.X, VectorExtensions.Y);
            IfcFacetedBrep facetedBrep = IfcGeometry.CreateClippedCone(
                model, Diameters[0] / 2, Diameters[1] / 2, Length, 
                NumSegments, axisSettings, XbimVector3D.Zero
            );

            return new IfcRepresentationItem[] { facetedBrep };
        }

        private void ClipPipes()
        {
            IfcAbstractSegmentEntity[] abstractSegmentEntities = ConnectedEntities
                .OfType<IfcAbstractSegmentEntity>()
                .ToArray();
            
            abstractSegmentEntities[1].Clip(NodeEntity, Length);
        }
    }
}