using System;
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
    public class IfcVertexHingedAnchorEntity : IfcAbstractAnchorEntity
    {
        private readonly bool _isVertical;
        
        private readonly double _height;
        
        private readonly double _xDim;
        private readonly double _yDim;
        private readonly double _rectangleHeight;
        
        private readonly int _numSegments;
        private readonly double _angleStep;
        
        private readonly double _coneRadius;
        private readonly double _coneBotCoordinates;
        private readonly double _coneTopCoordinates;

        private StartHingedAnchorEntity _hingedAnchorEntity;
        private IfcDiscreteAccessory _discreteAccessory;

        public IfcVertexHingedAnchorEntity(StartHingedAnchorEntity hingedAnchorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(hingedAnchorEntity, nodeEntity)
        {
            _hingedAnchorEntity = hingedAnchorEntity;

            _isVertical = abstractSegmentEntities[0].ObjectMatrix3D.Forward == VectorExtensions.Z;

            _numSegments = numSegments;
            _angleStep = 2 * Math.PI / _numSegments;
            
            _coneRadius = abstractSegmentEntities[0].Diameter / 2;
            _xDim = _coneRadius * 3;
            _yDim = _xDim;
            _rectangleHeight = _xDim / 10;
            _height = _xDim * 1.5;

            _coneBotCoordinates = -_height + _rectangleHeight;
            _coneTopCoordinates = _isVertical ? 0 : -_coneRadius;

            XbimVector3D forward = new XbimVector3D(0, 0, 1);
            XbimVector3D up = new XbimVector3D(0, 1, 0);
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(NodeEntity.ObjectMatrix3D.Translation, forward, up);
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IfcRepresentationItem[] representationItems = _isVertical 
                ? CreateVerticalAnchor(model) 
                : CreateHorizontalAnchor(model);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);

            _discreteAccessory = model.Instances.New<IfcDiscreteAccessory>(accessory =>
            {
                accessory.Name = _hingedAnchorEntity.Name;
                accessory.Tag = Tag;
                accessory.PredefinedType = IfcDiscreteAccessoryTypeEnum.ANCHORPLATE;
                accessory.Representation = shape;
                accessory.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            AddProperties(model, _discreteAccessory);

            return _discreteAccessory;
        }

        private IfcRepresentationItem[] CreateHorizontalAnchor(IModel model)
        {
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[2];
            representationItems[0] = CreateRectangle(model, XbimVector3D.Zero);

            IfcCartesianPoint[] bottomCircle = CreateCircle(model, _coneRadius, _coneBotCoordinates, XbimVector3D.Zero);
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, VectorExtensions.Forward * _coneTopCoordinates);
            representationItems[1] = CreateCone(model, bottomCircle, topPoint);

            return representationItems;
        }
        
        private IfcRepresentationItem[] CreateVerticalAnchor(IModel model)
        {
            XbimVector3D[] displacements = new []
            {
                VectorExtensions.Right.Negated() * _xDim,
                VectorExtensions.Right * _xDim
            };
            
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[4];
            representationItems[0] = CreateRectangle(model, displacements[0]);
            representationItems[1] = CreateRectangle(model, displacements[1]);

            IfcCartesianPoint[] bottomCircle1 = CreateCircle(model, _coneRadius, _coneBotCoordinates, displacements[0]);
            IfcCartesianPoint topPoint1 = IfcAxis.CreatePoint(model, VectorExtensions.Forward * _coneTopCoordinates + displacements[0]);
            representationItems[2] = CreateCone(model, bottomCircle1, topPoint1);
            
            IfcCartesianPoint[] bottomCircle2 = CreateCircle(model, _coneRadius, _coneBotCoordinates, displacements[1]);
            IfcCartesianPoint topPoint2 = IfcAxis.CreatePoint(model, VectorExtensions.Forward * _coneTopCoordinates + displacements[1]);
            representationItems[3] = CreateCone(model, bottomCircle2, topPoint2);

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