using System.Collections.Generic;
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

namespace IFC.Entities.Abstract.Anchors
{
    #if NEW

    public abstract class IfcAbstractConstantForceSupportHangerEntity : IfcAbstractConstantForceSupportEntity
    {
        protected IfcAbstractConstantForceSupportHangerEntity(XbimMatrix3D objectMatrix) : base(objectMatrix) { }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcDiscreteAccessory discreteAccessory = CreateIfcEntity<IfcDiscreteAccessory>(model);
            return discreteAccessory;
        }
        
        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcDiscreteAccessory, IInstantiableEntity
        {
            T discreteAccessory = base.CreateIfcEntity<T>(model);
            discreteAccessory.PredefinedType = IfcDiscreteAccessoryTypeEnum.ANCHORPLATE;

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchorModel(model, XbimVector3D.Zero);
            AddShapeRepresentation(model, discreteAccessory, representationItems);
            
            return discreteAccessory;
        }
        
        protected IEnumerable<IfcRepresentationItem> CreateAnchorModel(IModel model, XbimVector3D displacement)
        {
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[4];

            double rectangleXDim = Diameter;
            double rectangleYDim = Diameter;
            double rectangleHeight = Height / 20;
            XbimVector3D rectangleCoordinates = displacement + (Height - rectangleHeight) * VectorExtensions.Forward;
            representationItems[0] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates);
            
            double coneHeight = Height / 4;
            XbimVector3D botConeTopCoordinates = rectangleCoordinates;
            XbimVector3D botConeCoordinates = botConeTopCoordinates - coneHeight * VectorExtensions.Forward;
            double coneRadius = Diameter / 4;
            IfcCartesianPoint[] botConeCircle = IfcVertexGeometry.CreateCircle(model, coneRadius, botConeCoordinates, NumSegments);
            IfcCartesianPoint botConeTopPoint = IfcAxis.CreatePoint(model, botConeTopCoordinates);
            representationItems[1] = IfcVertexGeometry.CreateCone(model, botConeCircle, botConeTopPoint);

            double stickRadius = Diameter / 10;
            double stickHeight = Height - rectangleHeight - 2 * coneHeight - Diameter / 2;
            XbimVector3D stickCoordinates = botConeCoordinates - stickHeight * VectorExtensions.Forward;
            representationItems[2] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates);

            XbimVector3D topConeCoordinates = stickCoordinates;
            XbimVector3D topConeTopCoordinates = topConeCoordinates - coneHeight * VectorExtensions.Forward;
            double topConeRadius = Diameter / 4;
            IfcCartesianPoint[] topConeCircle = IfcVertexGeometry.CreateCircle(model, topConeRadius, topConeCoordinates, NumSegments);
            IfcCartesianPoint topConeTopPoint = IfcAxis.CreatePoint(model, topConeTopCoordinates);
            representationItems[3] = IfcVertexGeometry.CreateCone(model, topConeCircle, topConeTopPoint);

            return representationItems;
        }
    }
    
    #else
    
    public abstract class IfcAbstractConstantForceSupportHangerEntity : IfcAbstractConstantForceSupportEntity
    {
        private readonly StartConstantForceSupportHangerEntity _constantForceSupport;
        private IfcDiscreteAccessory? _discreteAccessory;
        
        protected IfcAbstractConstantForceSupportHangerEntity(StartConstantForceSupportHangerEntity constantForceSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(constantForceSupport, nodeEntity, segmentEntities)
        {
            _constantForceSupport = constantForceSupport;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchor(model, -Diameter / 2 * VectorExtensions.Forward);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, representationItems);
            
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

            double rectangleXDim = Diameter;
            double rectangleYDim = Diameter;
            double rectangleHeight = Height / 20;
            XbimVector3D rectangleCoordinates = displacement + (Height - rectangleHeight) * VectorExtensions.Forward;
            representationItems[0] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates);
            
            double coneHeight = Height / 4;
            XbimVector3D botConeTopCoordinates = rectangleCoordinates;
            XbimVector3D botConeCoordinates = botConeTopCoordinates - coneHeight * VectorExtensions.Forward;
            double coneRadius = Diameter / 4;
            IfcCartesianPoint[] botConeCircle = IfcVertexGeometry.CreateCircle(model, coneRadius, botConeCoordinates, NumSegments);
            IfcCartesianPoint botConeTopPoint = IfcAxis.CreatePoint(model, botConeTopCoordinates);
            representationItems[1] = IfcVertexGeometry.CreateCone(model, botConeCircle, botConeTopPoint);

            double stickRadius = Diameter / 10;
            double stickHeight = Height - rectangleHeight - 2 * coneHeight - Diameter / 2;
            XbimVector3D stickCoordinates = botConeCoordinates - stickHeight * VectorExtensions.Forward;
            representationItems[2] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates);

            XbimVector3D topConeCoordinates = stickCoordinates;
            XbimVector3D topConeTopCoordinates = topConeCoordinates - coneHeight * VectorExtensions.Forward;
            double topConeRadius = Diameter / 4;
            IfcCartesianPoint[] topConeCircle = IfcVertexGeometry.CreateCircle(model, topConeRadius, topConeCoordinates, NumSegments);
            IfcCartesianPoint topConeTopPoint = IfcAxis.CreatePoint(model, topConeTopCoordinates);
            representationItems[3] = IfcVertexGeometry.CreateCone(model, topConeCircle, topConeTopPoint);

            return representationItems;
        }
    }

    #endif
}