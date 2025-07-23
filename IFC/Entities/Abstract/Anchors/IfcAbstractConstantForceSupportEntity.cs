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
    
    
    
    #else
    
    public abstract class IfcAbstractConstantForceSupportEntity : IfcAbstractNonFixedSupportEntity
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double Height { get; protected set; }

        private readonly StartConstantForceSupportEntity _constantForceSupport;
        private IfcDiscreteAccessory? _discreteAccessory;
        
        protected IfcAbstractConstantForceSupportEntity(StartConstantForceSupportEntity constantForceSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(constantForceSupport, nodeEntity, segmentEntities)
        {
            _constantForceSupport = constantForceSupport;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchor(model, Diameter / 2 * VectorExtensions.Forward);
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

            XbimVector3D rectangleCoordinates = displacement - Height * VectorExtensions.Forward;
            double rectangleXDim = Diameter;
            double rectangleYDim = Diameter;
            double rectangleHeight = Height / 20;
            representationItems[0] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates);
            
            double coneHeight = Height / 4;
            XbimVector3D botConeTopCoordinates = rectangleCoordinates + rectangleHeight * VectorExtensions.Forward;
            XbimVector3D botConeCoordinates = botConeTopCoordinates + coneHeight * VectorExtensions.Forward;
            double coneRadius = Diameter / 4;
            IfcCartesianPoint[] botConeCircle = IfcVertexGeometry.CreateCircle(model, coneRadius, botConeCoordinates, NumSegments);
            IfcCartesianPoint botConeTopPoint = IfcAxis.CreatePoint(model, botConeTopCoordinates);
            representationItems[1] = IfcVertexGeometry.CreateCone(model, botConeCircle, botConeTopPoint);

            XbimVector3D stickCoordinates = botConeCoordinates;
            double stickRadius = Diameter / 10;
            double stickHeight = Height - rectangleHeight - 2 * coneHeight - Diameter / 2;
            representationItems[2] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates);

            XbimVector3D topConeCoordinates = stickCoordinates + stickHeight * VectorExtensions.Forward;
            XbimVector3D topConeTopCoordinates = topConeCoordinates + coneHeight * VectorExtensions.Forward;
            double topConeRadius = Diameter / 4;
            IfcCartesianPoint[] topConeCircle = IfcVertexGeometry.CreateCircle(model, topConeRadius, topConeCoordinates, NumSegments);
            IfcCartesianPoint topConeTopPoint = IfcAxis.CreatePoint(model, topConeTopCoordinates);
            representationItems[3] = IfcVertexGeometry.CreateCone(model, topConeCircle, topConeTopPoint);

            return representationItems;
        }
    }

    #endif
}