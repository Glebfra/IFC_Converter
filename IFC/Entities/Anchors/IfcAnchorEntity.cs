using IFC.Entities.Abstract;
using IFC.Entities.Fittings;
using IFC.Extensions;
using IFC.Tools;
using Start.API;
using Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Anchors
{
    public class IfcAnchorEntity : IfcAbstractAnchorEntity
    {
        private double _xDim;
        private double _yDim;
        private StartAnchorEntity _anchorEntity;
        private IfcPipeFitting _pipeFitting;
        
        public IfcAnchorEntity(StartAnchorEntity anchorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities)
            : base(anchorEntity, nodeEntity, abstractSegmentEntities)
        {
            _anchorEntity = anchorEntity;
            _xDim = _IfcAbstractSegmentEntities[0].Diameter * 2;
            _yDim = _xDim;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IfcRectangleProfileDef profileDef = CreateRectangleProfileDef(model);
            IfcExtrudedAreaSolid extrudedAreaSolid = CreateExtrudedAreaSolid(model, profileDef);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, extrudedAreaSolid);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _anchorEntity.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.OBSTRUCTION;
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });

            return _pipeFitting;
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