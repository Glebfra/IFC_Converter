using System.Collections.Generic;
using IFC.Entities.Abstract;
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

namespace IFC.Entities.Anchors.CAD
{
    public class IfcAnchorEntity : IfcAbstractAnchorEntity
    {
        private double _xDim;
        private double _yDim;

        private StartAnchorEntity _anchorEntity;
        private IfcDiscreteAccessory _discreteAccessory;

        public IfcAnchorEntity(StartAnchorEntity anchorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities)
            : base(anchorEntity, nodeEntity, abstractSegmentEntities)
        {
            _anchorEntity = anchorEntity;

            _xDim = abstractSegmentEntities[0].OuterDiameter * 2;
            _yDim = _xDim;
            
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = abstractSegmentEntities[0].ObjectMatrix3D.Forward;
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
                accessory.Name = _anchorEntity.Name;
                accessory.Tag = Tag;
                accessory.PredefinedType = IfcDiscreteAccessoryTypeEnum.ANCHORPLATE;
                accessory.Representation = shape;
                accessory.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            AddProperties(model, _discreteAccessory);

            return _discreteAccessory;
        }

        protected override IEnumerable<IfcRepresentationItem> CreateAnchorModel(IModel model, XbimVector3D displacement)
        {
            IfcExtrudedAreaSolid rectangle = IfcGeometry.CreateRectangle(model, _xDim, _yDim, _xDim / 10, XbimVector3D.Zero);
            return new[] { rectangle };
        }
    }
}