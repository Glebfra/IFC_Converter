using System.Collections.Generic;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Anchors;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedComponentElements;

namespace IFC.Entities.Abstract.Anchors
{
    #if NEW
    
    
    
    #else
    
    public abstract class IfcAbstractFixedSupportEntity : IfcAbstractAnchorEntity
    {
        public abstract double XDim { get; protected set; }
        public abstract double YDim { get; protected set; }
        
        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }

        private readonly StartAnchorEntity _anchor;
        private IfcDiscreteAccessory? _discreteAccessory;
        
        protected IfcAbstractFixedSupportEntity(StartAnchorEntity anchor, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(anchor, nodeEntity, segmentEntities)
        {
            _anchor = anchor;
            
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = AbstractSegmentEntities[0].ObjectMatrix3D.Forward;
            XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
            if (forward == WorldUp || forward == -1 * WorldUp)
                WorldUp = new XbimVector3D(0, 1, 0);
            XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);
            
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchorModel(model, XbimVector3D.Zero);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, representationItems);
            
            _discreteAccessory = model.Instances.New<IfcDiscreteAccessory>(accessory =>
            {
                accessory.Name = _anchor.Name;
                accessory.Tag = Tag;
                accessory.PredefinedType = IfcDiscreteAccessoryTypeEnum.ANCHORPLATE;
                accessory.Representation = shape;
                accessory.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            AddProperties(model, _discreteAccessory);

            return _discreteAccessory;
        }

        private IEnumerable<IfcRepresentationItem> CreateAnchorModel(IModel model, XbimVector3D displacement)
        {
            IfcExtrudedAreaSolid rectangle = IfcGeometry.CreateRectangle(model, XDim, YDim, XDim / 10, XbimVector3D.Zero);
            return new[] { rectangle };
        }
    }

    #endif
}