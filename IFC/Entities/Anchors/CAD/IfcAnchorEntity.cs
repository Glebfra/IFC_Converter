using IFC.Entities.Abstract;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Anchors;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
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
            : base(anchorEntity, nodeEntity)
        {
            _anchorEntity = anchorEntity;

            _xDim = abstractSegmentEntities[0].Diameter * 2;
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
            
            IfcRectangleProfileDef profileDef = CreateRectangleProfileDef(model);
            IfcExtrudedAreaSolid extrudedAreaSolid = CreateExtrudedAreaSolid(model, profileDef);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, extrudedAreaSolid);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
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

        private IfcRectangleProfileDef CreateRectangleProfileDef(IModel model)
        {
            return model.Instances.New<IfcRectangleProfileDef>(def =>
            {
                def.ProfileType = IfcProfileTypeEnum.AREA;
                def.XDim = _xDim;
                def.YDim = _yDim;
            });
        }
        
        private IfcExtrudedAreaSolid CreateExtrudedAreaSolid(IModel model, IfcProfileDef profileDef)
        {
            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.Depth = _xDim / 10;
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = IfcAxis.CreateDirection(model, VectorExtensions.Forward);
            });
        }
    }
}