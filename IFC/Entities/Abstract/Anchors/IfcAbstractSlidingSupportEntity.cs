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

    public abstract class IfcAbstractSlidingSupportEntity : IfcAbstractNonFixedSupportEntity
    {
        public abstract ActionProperty<int> NumSegments { get; }
        public abstract ActionProperty<double> Height { get; }
        
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

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchor(model, Diameter / 2 * VectorExtensions.Forward);
            AddShapeRepresentation(model, discreteAccessory, representationItems);
            
            return discreteAccessory;
        }
        
        protected override IEnumerable<IfcRepresentationItem> CreateAnchorModel(IModel model, XbimVector3D displacement)
        {
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[3];
            
            XbimVector3D rectangleCoordinates = displacement - Height * VectorExtensions.Forward;
            double rectangleXDim = Diameter;
            double rectangleYDim = Diameter;
            double rectangleHeight = Height / 20;
            representationItems[0] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates);
            
            XbimVector3D stickCoordinates = rectangleCoordinates + rectangleHeight * VectorExtensions.Forward;
            double stickRadius = Diameter / 10;
            double stickHeight = Height / 3;
            representationItems[1] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates);
            
            XbimVector3D coneCoordinates = stickCoordinates + stickHeight * VectorExtensions.Forward;
            XbimVector3D coneTopCoordinates = displacement - Diameter / 2 * VectorExtensions.Forward;
            double coneRadius = Diameter / 4;
            IfcCartesianPoint[] circle = IfcVertexGeometry.CreateCircle(model, coneRadius, coneCoordinates, NumSegments);
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, coneTopCoordinates);
            representationItems[2] = IfcVertexGeometry.CreateCone(model, circle, topPoint);

            return representationItems;
        }
    }
    
    #else
    
    public abstract class IfcAbstractSlidingSupportEntity : IfcAbstractNonFixedSupportEntity
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double Height { get; protected set; }

        private readonly StartSlidingSupportEntity _slidingSupport;
        private IfcDiscreteAccessory? _discreteAccessory;
        
        protected IfcAbstractSlidingSupportEntity(StartSlidingSupportEntity slidingSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(slidingSupport, nodeEntity, segmentEntities)
        {
            _slidingSupport = slidingSupport;
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
                accessory.Name = _slidingSupport.Name;
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
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[3];
            
            XbimVector3D rectangleCoordinates = displacement - Height * VectorExtensions.Forward;
            double rectangleXDim = Diameter;
            double rectangleYDim = Diameter;
            double rectangleHeight = Height / 20;
            representationItems[0] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates);
            
            XbimVector3D stickCoordinates = rectangleCoordinates + rectangleHeight * VectorExtensions.Forward;
            double stickRadius = Diameter / 10;
            double stickHeight = Height / 3;
            representationItems[1] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates);
            
            XbimVector3D coneCoordinates = stickCoordinates + stickHeight * VectorExtensions.Forward;
            XbimVector3D coneTopCoordinates = displacement - Diameter / 2 * VectorExtensions.Forward;
            double coneRadius = Diameter / 4;
            IfcCartesianPoint[] circle = IfcVertexGeometry.CreateCircle(model, coneRadius, coneCoordinates, NumSegments);
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, coneTopCoordinates);
            representationItems[2] = IfcVertexGeometry.CreateCone(model, circle, topPoint);

            return representationItems;
        }
    }

    #endif
}