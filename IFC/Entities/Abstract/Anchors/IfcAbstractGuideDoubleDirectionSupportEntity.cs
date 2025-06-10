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
    public abstract class IfcAbstractGuideDoubleDirectionSupportEntity : IfcAbstractNonFixedSupportEntity
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double Height { get; protected set; }

        private readonly StartGuideDoubleDirectionSupportEntity _doubleDirectionSupport;
        private IfcDiscreteAccessory? _discreteAccessory;
        
        protected IfcAbstractGuideDoubleDirectionSupportEntity(StartGuideDoubleDirectionSupportEntity doubleDirectionSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(doubleDirectionSupport, nodeEntity, segmentEntities)
        {
            _doubleDirectionSupport = doubleDirectionSupport;

            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = IfcAxis.GetPipeDirectionFromNode(AbstractSegmentEntities[1], coordinates);
            ObjectMatrix3D = MatrixExtensions.CreateWorld(coordinates, forward);
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchorModel(model, XbimVector3D.Zero);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, representationItems, IfcRepresentationType.Brep, IfcRepresentationIdentifier.Body);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, representationItems);

            _discreteAccessory = model.Instances.New<IfcDiscreteAccessory>(accessory =>
            {
                accessory.Name = _doubleDirectionSupport.Name;
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
            XbimVector3D[] zDirections = new[] { VectorExtensions.Up, VectorExtensions.Up.Negated(), VectorExtensions.Right, VectorExtensions.Right.Negated() };
            XbimVector3D[] xDirections = new[] { VectorExtensions.Right, VectorExtensions.Right, VectorExtensions.Up, VectorExtensions.Up };
            XbimVector3D[] yDirections = new XbimVector3D[4];

            for (int i = 0; i < 4; i++)
            {
                yDirections[i] = XbimVector3D.CrossProduct(zDirections[i], xDirections[i]);
            }
            
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[zDirections.Length * 3];

            for (int i = 0; i < zDirections.Length; i++)
            {
                XbimVector3D rectangleCoordinates = displacement - Height * zDirections[i];
                double rectangleXDim = Diameter;
                double rectangleYDim = Diameter;
                double rectangleHeight = Height / 20;
                representationItems[i*3] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates, zDirections[i], xDirections[i]);
            
                XbimVector3D stickCoordinates = rectangleCoordinates + rectangleHeight * zDirections[i];
                double stickRadius = Diameter / 10;
                double stickHeight = Height / 3;
                representationItems[i*3 + 1] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates, zDirections[i], xDirections[i]);
            
                XbimVector3D coneCoordinates = stickCoordinates + stickHeight * zDirections[i];
                XbimVector3D coneTopCoordinates = displacement - Diameter / 2 * zDirections[i];
                double coneRadius = Diameter / 4;
                IfcCartesianPoint[] circle = IfcVertexGeometry.CreateCircle(model, coneRadius, coneCoordinates, NumSegments, xDirections[i], yDirections[i]);
                IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, coneTopCoordinates);
                representationItems[i*3 + 2] = IfcVertexGeometry.CreateCone(model, circle, topPoint);
            }
            
            return representationItems;
        }
    }
}