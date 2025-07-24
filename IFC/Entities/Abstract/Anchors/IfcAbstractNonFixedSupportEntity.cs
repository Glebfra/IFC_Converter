using System.Collections.Generic;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Entities.Abstract.Anchors
{
    #if NEW

    public abstract class IfcAbstractNonFixedSupportEntity : IfcAbstractAnchorEntity
    {
        protected bool _IsVertical;
        public abstract ActionProperty<double> Diameter { get; }

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
            XbimVector3D tangentDisplacement = Diameter * VectorExtensions.Right;

            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            representationItems.AddRange(CreateAnchorModel(model, tangentDisplacement.Negated() + normalDisplacement));
            representationItems.AddRange(CreateAnchorModel(model, tangentDisplacement + normalDisplacement));

            return representationItems;
        }
    }
    
    #else
    
    public abstract class IfcAbstractNonFixedSupportEntity : IfcAbstractAnchorEntity
    {
        public abstract double Diameter { get; protected set; }

        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }

        protected bool _IsVertical;
        
        protected IfcAbstractNonFixedSupportEntity(StartAbstractEntity startAbstractEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(startAbstractEntity, nodeEntity, segmentEntities)
        {
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
            XbimVector3D tangentDisplacement = Diameter * VectorExtensions.Right;

            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            representationItems.AddRange(CreateAnchorModel(model, tangentDisplacement.Negated() + normalDisplacement));
            representationItems.AddRange(CreateAnchorModel(model, tangentDisplacement + normalDisplacement));

            return representationItems;
        }
    }

    #endif
}