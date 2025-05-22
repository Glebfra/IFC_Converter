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
    public class IfcGuideSingleDirectionSupportEntity : IfcAbstractAnchorEntity
    {
        private int _numSegments;
        private double _height;
        
        private StartGuideSingleDirectionSupportEntity _guideSingleDirectionSupportEntity;
        private IfcDiscreteAccessory _discreteAccessory;
        
        public IfcGuideSingleDirectionSupportEntity(StartGuideSingleDirectionSupportEntity guideSingleDirectionSupportEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(guideSingleDirectionSupportEntity, nodeEntity, segmentEntities)
        {
            _guideSingleDirectionSupportEntity = guideSingleDirectionSupportEntity;

            _numSegments = 8;
            _height = _PipeDiameter * 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchorModel(model, XbimVector3D.Zero);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);

            _discreteAccessory = model.Instances.New<IfcDiscreteAccessory>(accessory =>
            {
                accessory.Name = _guideSingleDirectionSupportEntity.Name;
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
            XbimVector3D[] xDirections = new[] { VectorExtensions.Up, VectorExtensions.Forward, VectorExtensions.Forward.Negated() };
            XbimVector3D[] yDirections = new[] { VectorExtensions.Right, VectorExtensions.Right.Negated(), VectorExtensions.Right.Negated() };
            XbimVector3D[] zDirections = new[] { VectorExtensions.Forward, VectorExtensions.Up, VectorExtensions.Up.Negated() };
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[zDirections.Length * 3];

            for (int i = 0; i < zDirections.Length; i++)
            {
                XbimVector3D rectangleCoordinates = displacement - _height * zDirections[i];
                double rectangleXDim = _PipeDiameter;
                double rectangleYDim = _PipeDiameter;
                double rectangleHeight = _height / 20;
                representationItems[i*3] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates, zDirections[i], xDirections[i]);
            
                XbimVector3D stickCoordinates = rectangleCoordinates + rectangleHeight * zDirections[i];
                double stickRadius = _PipeDiameter / 10;
                double stickHeight = _height / 3;
                representationItems[i*3 + 1] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates, zDirections[i], xDirections[i]);
            
                XbimVector3D coneCoordinates = stickCoordinates + stickHeight * zDirections[i];
                XbimVector3D coneTopCoordinates = displacement - _PipeDiameter / 2 * zDirections[i];
                double coneRadius = _PipeDiameter / 4;
                IfcCartesianPoint[] circle = IfcVertexGeometry.CreateCircle(model, coneRadius, coneCoordinates, _numSegments, xDirections[i], yDirections[i]);
                IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, coneTopCoordinates);
                representationItems[i*3 + 2] = IfcVertexGeometry.CreateCone(model, circle, topPoint);
            }
            
            return representationItems;
        }
    }
}