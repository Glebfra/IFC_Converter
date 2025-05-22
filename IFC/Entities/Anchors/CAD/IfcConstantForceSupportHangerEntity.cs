using System.Collections.Generic;
using IFC.Entities.Abstract;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Anchors;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedComponentElements;

namespace IFC.Entities.Anchors.CAD
{
    public class IfcConstantForceSupportHangerEntity : IfcAbstractAnchorEntity
    {
        private int _numSegments;
        private double _height;
        
        private StartConstantForceSupportHangerEntity _constantForceSupport;
        private IfcDiscreteAccessory _discreteAccessory;
        
        public IfcConstantForceSupportHangerEntity(StartConstantForceSupportHangerEntity constantForceSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(constantForceSupport, nodeEntity, segmentEntities)
        {
            _constantForceSupport = constantForceSupport;
            
            _numSegments = 8;
            _height = _PipeDiameter * 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchor(model, -_PipeDiameter / 2 * VectorExtensions.Forward);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            _discreteAccessory = model.Instances.New<IfcDiscreteAccessory>(accessory =>
            {
                accessory.Name = _constantForceSupport.Name;
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
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[4];

            double rectangleXDim = _PipeDiameter;
            double rectangleYDim = _PipeDiameter;
            double rectangleHeight = _height / 20;
            XbimVector3D rectangleCoordinates = displacement + (_height - rectangleHeight) * VectorExtensions.Forward;
            representationItems[0] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates);
            
            double coneHeight = _height / 4;
            XbimVector3D botConeTopCoordinates = rectangleCoordinates;
            XbimVector3D botConeCoordinates = botConeTopCoordinates - coneHeight * VectorExtensions.Forward;
            double coneRadius = _PipeDiameter / 4;
            IfcCartesianPoint[] botConeCircle = IfcVertexGeometry.CreateCircle(model, coneRadius, botConeCoordinates, _numSegments);
            IfcCartesianPoint botConeTopPoint = IfcAxis.CreatePoint(model, botConeTopCoordinates);
            representationItems[1] = IfcVertexGeometry.CreateCone(model, botConeCircle, botConeTopPoint);

            double stickRadius = _PipeDiameter / 10;
            double stickHeight = _height - rectangleHeight - 2 * coneHeight - _PipeDiameter / 2;
            XbimVector3D stickCoordinates = botConeCoordinates - stickHeight * VectorExtensions.Forward;
            representationItems[2] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates);

            XbimVector3D topConeCoordinates = stickCoordinates;
            XbimVector3D topConeTopCoordinates = topConeCoordinates - coneHeight * VectorExtensions.Forward;
            double topConeRadius = _PipeDiameter / 4;
            IfcCartesianPoint[] topConeCircle = IfcVertexGeometry.CreateCircle(model, topConeRadius, topConeCoordinates, _numSegments);
            IfcCartesianPoint topConeTopPoint = IfcAxis.CreatePoint(model, topConeTopCoordinates);
            representationItems[3] = IfcVertexGeometry.CreateCone(model, topConeCircle, topConeTopPoint);

            return representationItems;
        }
    }
}