using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Entities.Abstract.Anchors
{
    public abstract class IfcAbstractNonFixedSupportEntity : IfcAbstractAnchorEntity
    {
        public abstract ActionProperty<double> Diameter { get; }

        protected IfcAbstractNonFixedSupportEntity(XbimMatrix3D objectMatrix) : base(objectMatrix) { }

        protected abstract IEnumerable<IfcRepresentationItem> CreateAnchorModel(IModel model, XbimVector3D displacement);

        protected IEnumerable<IfcRepresentationItem> CreateAnchor(IModel model, XbimVector3D normalDisplacement)
        {
            IfcAbstractSegmentEntity? abstractSegmentEntity = ConnectedEntities.OfType<IfcAbstractSegmentEntity>().FirstOrDefault();
            if (abstractSegmentEntity == null)
                throw new Exception($"Cannot find connected segment to {nameof(IfcAbstractNonFixedSupportEntity)}");
            bool isVertical = abstractSegmentEntity.ObjectMatrix3D.Value.Forward.IsParallel(VectorExtensions.Z);
            return isVertical ? CreateVerticalAnchor(model, normalDisplacement) : CreateHorizontalAnchor(model);
        }

        private IEnumerable<IfcRepresentationItem> CreateHorizontalAnchor(IModel model)
        {
            return CreateAnchorModel(model, XbimVector3D.Zero);
        }
        
        private IEnumerable<IfcRepresentationItem> CreateVerticalAnchor(IModel model, XbimVector3D normalDisplacement)
        {
            XbimVector3D tangentDisplacement = Diameter * VectorExtensions.Right;

            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            representationItems.AddRange(CreateAnchorModel(model, tangentDisplacement.Negated() + normalDisplacement));
            representationItems.AddRange(CreateAnchorModel(model, tangentDisplacement + normalDisplacement));

            return representationItems;
        }
    }
}