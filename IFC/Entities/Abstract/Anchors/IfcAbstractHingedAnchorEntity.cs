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
    
    public abstract class IfcAbstractHingedAnchorEntity : IfcAbstractNonFixedSupportEntity
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double Height { get; protected set; }
        
        private readonly StartHingedAnchorEntity _hingedAnchor;
        private IfcDiscreteAccessory? _discreteAccessory;
        
        protected IfcAbstractHingedAnchorEntity(StartHingedAnchorEntity hingedAnchor, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(hingedAnchor, nodeEntity, segmentEntities)
        {
            _hingedAnchor = hingedAnchor;
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
                accessory.Name = _hingedAnchor.Name;
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
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[2];

            XbimVector3D rectangleCoordinates = displacement - Height * VectorExtensions.Forward;
            double rectangleXDim = Diameter;
            double rectangleYDim = Diameter;
            double rectangleHeight = Height / 20;
            representationItems[0] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates);

            XbimVector3D coneCoordinates = rectangleCoordinates + rectangleHeight * VectorExtensions.Forward;
            XbimVector3D coneTopCoordinates = displacement - Diameter / 2 * VectorExtensions.Forward;
            double coneRadius = Diameter / 2;
            IfcCartesianPoint[] circle = IfcVertexGeometry.CreateCircle(model, coneRadius, coneCoordinates, NumSegments);
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, coneTopCoordinates);
            representationItems[1] = IfcVertexGeometry.CreateCone(model, circle, topPoint);
            
            return representationItems;
        }
    }

    #endif
}