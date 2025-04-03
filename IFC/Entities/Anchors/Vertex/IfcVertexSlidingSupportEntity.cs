using System;
using System.Collections.Generic;
using IFC.Entities.Abstract;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Anchors;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedComponentElements;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Entities.Anchors.Vertex
{
    public class IfcVertexSlidingSupportEntity : IfcAbstractAnchorEntity
    {
        private readonly bool _isVertical;
        
        private readonly double _height;

        private readonly double _xDim;
        private readonly double _yDim;
        private readonly double _rectangleHeight;

        private readonly double _stickRadius;
        private readonly double _stickBotCoordinates;
        private readonly double _stickHeight;
        
        private readonly double _coneRadius;
        private readonly double _coneBotCoordinates;
        private readonly double _coneTopCoordinates;
        
        private int _numSegments;
        private double _angleStep;
        
        private StartSlidingSupportEntity _slidingSupportEntity;
        private IfcDiscreteAccessory _discreteAccessory;
        
        public IfcVertexSlidingSupportEntity(StartSlidingSupportEntity slidingSupportEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(slidingSupportEntity, nodeEntity)
        {
            _slidingSupportEntity = slidingSupportEntity;
            
            _isVertical = abstractSegmentEntities[0].ObjectMatrix3D.Forward == VectorExtensions.Z;
            
            _numSegments = numSegments;
            _angleStep = 2 * Math.PI / _numSegments;
            
            _coneRadius = abstractSegmentEntities[0].Diameter / 4;
            _xDim = _coneRadius * 3;
            _yDim = _xDim * 2;
            _rectangleHeight = _xDim / 10;
            _height = _xDim * 1.5;

            _stickRadius = _coneRadius / 3;
            _stickBotCoordinates = -_height + _rectangleHeight;
            _stickHeight = _height / 3;
            
            _coneBotCoordinates = _stickBotCoordinates + _stickHeight;
            _coneTopCoordinates = _isVertical ? 0 : -abstractSegmentEntities[0].Diameter / 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IEnumerable<IfcRepresentationItem> representationItems = _isVertical
                ? CreateVerticalAnchor(model)
                : CreateHorizontalAnchor(model);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            _discreteAccessory = model.Instances.New<IfcDiscreteAccessory>(accessory =>
            {
                accessory.Name = _slidingSupportEntity.Name;
                accessory.Tag = Tag;
                accessory.PredefinedType = IfcDiscreteAccessoryTypeEnum.ANCHORPLATE;
                accessory.Representation = shape;
                accessory.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            AddProperties(model, _discreteAccessory);

            return _discreteAccessory;
        }
        
        private IEnumerable<IfcRepresentationItem> CreateHorizontalAnchor(IModel model)
        {
            return CreateAnchor(model, XbimVector3D.Zero);
        }
        
        private IEnumerable<IfcRepresentationItem> CreateVerticalAnchor(IModel model)
        {
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            representationItems.AddRange(CreateAnchor(model, VectorExtensions.Right.Negated() * _xDim));
            representationItems.AddRange(CreateAnchor(model, VectorExtensions.Right * _xDim));

            return representationItems;
        }

        private IEnumerable<IfcRepresentationItem> CreateAnchor(IModel model, XbimVector3D displacement)
        {
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[3];
            representationItems[0] = CreateRectangle(model, displacement);
            representationItems[1] = CreateStick(model, displacement);

            IfcCartesianPoint[] bottomCircle = CreateCircle(model, _coneRadius, _coneBotCoordinates, displacement);
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, VectorExtensions.Forward * _coneTopCoordinates + displacement);
            representationItems[2] = CreateCone(model, bottomCircle, topPoint);

            return representationItems;
        }

        private IfcExtrudedAreaSolid CreateRectangle(IModel model, XbimVector3D displacement)
        {
            IfcProfileDef profileDef = model.Instances.New<IfcRectangleProfileDef>(def =>
            {
                def.ProfileType = IfcProfileTypeEnum.AREA;
                def.XDim = _xDim;
                def.YDim = _yDim;
            });
            
            IfcCartesianPoint point = IfcAxis.CreatePoint(model, -_height * VectorExtensions.Forward + displacement);
            IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point);
            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.Depth = _rectangleHeight;
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = IfcAxis.CreateDirection(model, VectorExtensions.Forward);
                solid.Position = axis2Placement3D;
            });
        }

        private IfcExtrudedAreaSolid CreateStick(IModel model, XbimVector3D displacement)
        {
            IfcProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, _stickRadius, XbimVector3D.Zero);
            
            IfcCartesianPoint point = IfcAxis.CreatePoint(model, _stickBotCoordinates * VectorExtensions.Forward + displacement);
            IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point);
            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.Depth = _stickHeight;
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = IfcAxis.CreateDirection(model, VectorExtensions.Forward);
                solid.Position = axis2Placement3D;
            });
        }
        
        private IfcFacetedBrep CreateCone(IModel model, IfcCartesianPoint[] points, IfcCartesianPoint topPoint)
        {
            IfcFace[] faces = new IfcFace[_numSegments + 1];
            int facesIndex = 0;
            for (int i = 0; i < _numSegments; i++)
            {
                IfcCartesianPoint p1 = points[i];
                IfcCartesianPoint p2 = points[(i + 1) % _numSegments];
                faces[facesIndex++] = IfcVertexGeometry.CreateTriangleFace(model, p1, p2, topPoint);
            }
            faces[facesIndex++] = IfcVertexGeometry.CreatePolygonFace(model, points);
            
            return model.Instances.New<IfcFacetedBrep>(brep =>
            {
                brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
            });
        }
        
        private IfcCartesianPoint[] CreateCircle(IModel model, double radius, double height, XbimVector3D displacement)
        {
            IfcCartesianPoint[] points = new IfcCartesianPoint[_numSegments];
            for (int i = 0; i < _numSegments; i++)
            {
                XbimVector3D point = new XbimVector3D(radius * Math.Cos(_angleStep * i), radius * Math.Sin(_angleStep * i), height);
                points[i] = IfcAxis.CreatePoint(model, point + displacement);
            }

            return points;
        }
    }
}