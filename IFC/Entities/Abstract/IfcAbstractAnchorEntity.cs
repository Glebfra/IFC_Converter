using System.Collections.Generic;
using IFC.Entities.Interfaces;
using IFC.Extensions;
using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractAnchorEntity : IfcAbstractEntity, IIfcOneNodeEntity
    {
        public IfcNodeEntity NodeEntity { get; }
        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }

        protected IfcAbstractSegmentEntity[] _IfcAbstractSegmentEntities;
        protected double _PipeDiameter;
        protected bool _IsVertical;

        private StartAbstractEntity _abstractEntity;
        
        public IfcAbstractAnchorEntity(StartAbstractEntity abstractEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(abstractEntity)
        {
            _abstractEntity = abstractEntity;
            NodeEntity = nodeEntity;
            
            _IfcAbstractSegmentEntities = segmentEntities;
            _PipeDiameter = segmentEntities[0].OuterDiameter;
            _IsVertical = segmentEntities[0].ObjectMatrix3D.Forward.IsParallel(VectorExtensions.Z);

            XbimVector3D forward = new XbimVector3D(0, 0, 1);
            XbimVector3D up = new XbimVector3D(0, 1, 0);
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(NodeEntity.ObjectMatrix3D.Translation, forward, up);
        }

        protected abstract IEnumerable<IfcRepresentationItem> CreateAnchorModel(IModel model, XbimVector3D displacement);

        protected IEnumerable<IfcRepresentationItem> CreateAnchor(IModel model, XbimVector3D normalDisplacement)
        {
            return _IsVertical ? CreateVerticalAnchor(model, normalDisplacement) : CreateHorizontalAnchor(model);
        }

        private IEnumerable<IfcRepresentationItem> CreateHorizontalAnchor(IModel model)
        {
            return CreateAnchorModel(model, XbimVector3D.Zero);
        }
        
        private IEnumerable<IfcRepresentationItem> CreateVerticalAnchor(IModel model, XbimVector3D normalDisplacement)
        {
            XbimVector3D tangentDisplacement = _PipeDiameter * VectorExtensions.Right;

            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            representationItems.AddRange(CreateAnchorModel(model, tangentDisplacement.Negated() + normalDisplacement));
            representationItems.AddRange(CreateAnchorModel(model, tangentDisplacement + normalDisplacement));

            return representationItems;
        }

        protected override void AddProperties(IModel model, IfcProduct product)
        {
            base.AddProperties(model, product);
            
            #region DEBUG_ANCHOR
            #if DEBUG
            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "DEBUG_ANCHOR";
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "Is Vertical";
                        value.NominalValue = new IfcText(_IsVertical.ToString());
                    }));
                });
            });
            #endif
            #endregion
        }
    }
}