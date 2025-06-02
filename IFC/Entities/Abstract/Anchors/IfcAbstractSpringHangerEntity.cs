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
    public abstract class IfcAbstractSpringHangerEntity : IfcAbstractNonFixedSupportEntity
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double Height { get; protected set; }

        private readonly StartSpringSupportEntity _springSupport;
        private IfcDiscreteAccessory? _discreteAccessory;
        
        protected IfcAbstractSpringHangerEntity(StartSpringSupportEntity springSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(springSupport, nodeEntity, segmentEntities)
        {
            _springSupport = springSupport;
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
                accessory.Name = _springSupport.Name;
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

            double stickRadius = Diameter / 10;
            double stickHeight = Height / 3;
            XbimVector3D stickCoordinates = rectangleCoordinates - stickHeight * VectorExtensions.Forward;
            representationItems[1] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates);

            XbimVector3D springCoordinates = stickCoordinates;
            double springRadius = stickRadius * 2;
            double springHeight = stickHeight;
            double springWireRadius = stickRadius / 2;
            const int springNumTurns = 5;
            IfcCartesianPoint[] spiralPoints = IfcVertexGeometry.CreateSpiral(model, springRadius, springHeight, NumSegments, springNumTurns, springCoordinates);
            IfcPolyline spiralPolyline = IfcVertexGeometry.CreatePolyline(model, spiralPoints);
            representationItems[2] = IfcGeometry.CreateSweptDiskSolid(model, spiralPolyline, springWireRadius);

            XbimVector3D coneCoordinates = stickCoordinates;
            XbimVector3D coneTopCoordinates = displacement + Diameter / 2 * VectorExtensions.Forward;
            double coneRadius = Diameter / 4;
            IfcCartesianPoint[] circle = IfcVertexGeometry.CreateCircle(model, coneRadius, coneCoordinates, NumSegments);
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, coneTopCoordinates);
            representationItems[3] = IfcVertexGeometry.CreateCone(model, circle, topPoint);

            return representationItems;
        }
    }
}