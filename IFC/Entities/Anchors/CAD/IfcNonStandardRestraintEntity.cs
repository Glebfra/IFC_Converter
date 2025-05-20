using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract;
using IFC.Extensions;
using IFC.Tools;
using Start.API;
using Start.Entities;
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
    public class IfcNonStandardRestraintEntity : IfcAbstractAnchorEntity
    {
        private readonly double _height;
        private readonly int _numSegments;
        
        private StartNonStandardRestraint _nonStandardRestraint;
        private IfcDiscreteAccessory _discreteAccessory;
        
        public IfcNonStandardRestraintEntity(StartNonStandardRestraint nonStandardRestraint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(nonStandardRestraint, nodeEntity, segmentEntities)
        {
            _nonStandardRestraint = nonStandardRestraint;
            
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
                accessory.Name = _nonStandardRestraint.Name;
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
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();

            foreach (StartNonStandardRestraintModule restraintModule in _nonStandardRestraint.Restraints)
            {
                bool hasSpring = restraintModule.Type == StartRestraintTypeEnum.ELASTIC;
                bool isDoubleSided = restraintModule.Type != StartRestraintTypeEnum.RIGID_ONE_SIDED;

                XbimVector3D direction = CreateAnchorDirection(restraintModule);

                representationItems.AddRange(CreateSingleAnchorShape(model, direction, displacement - _height * direction, hasSpring));
                if (isDoubleSided)
                {
                    representationItems.AddRange(CreateSingleAnchorShape(model, direction.Negated(), displacement + _height * direction, hasSpring));
                }
            }

            return representationItems;
        }

        private XbimVector3D CreateAnchorDirection(StartNonStandardRestraintModule restraintModule)
        {
            const double PI_2 = Math.PI / 2;
            bool useLocalAxes = restraintModule.Local == StartRestraintAxesTypeEnum.LOCAL;

            int xAxisFactor = (int)((restraintModule.AngleX.SIProperty - PI_2) / PI_2);
            int yAxisFactor = (int)((restraintModule.AngleY.SIProperty - PI_2) / PI_2);
            int zAxisFactor = (int)((restraintModule.AngleZ.SIProperty - PI_2) / PI_2);

            if (!useLocalAxes)
            {
                return xAxisFactor * VectorExtensions.X +
                       yAxisFactor * VectorExtensions.Y +
                       zAxisFactor * VectorExtensions.Z.Negated();
            }
            
            foreach (IfcAbstractSegmentEntity segmentEntity in AbstractSegmentEntities)
            {
                if (segmentEntity.NodeEntities.All(item => item.ID != _nonStandardRestraint.SectionStartNode) ||
                    segmentEntity.NodeEntities.All(item => item.ID != _nonStandardRestraint.SectionEndNode))
                {
                    continue;
                }
                
                IfcNodeEntity startNode = segmentEntity.NodeEntities.First(item => item.ID == _nonStandardRestraint.SectionStartNode);
                IfcNodeEntity endNode = segmentEntity.NodeEntities.First(item => item.ID == _nonStandardRestraint.SectionEndNode);
                
                XbimVector3D forward = endNode.ObjectMatrix3D.Translation - startNode.ObjectMatrix3D.Translation;
                XbimMatrix3D fictiveObjectMatrix = MatrixExtensions.CreateWorld(XbimVector3D.Zero, forward);
                
                return xAxisFactor * fictiveObjectMatrix.Forward + 
                       yAxisFactor * fictiveObjectMatrix.Right + 
                       zAxisFactor * fictiveObjectMatrix.Up;
            }
            
            throw new NullReferenceException(nameof(IfcNonStandardRestraintEntity) + "Cannot find local axes");
        }

        private IEnumerable<IfcRepresentationItem> CreateSingleAnchorShape(IModel model, XbimVector3D direction, XbimVector3D displacement, bool hasSpring)
        {
            XbimMatrix3D shapePlacementMatrix = MatrixExtensions.CreateWorld(displacement, direction.Negated());
            XbimVector3D refDirection = shapePlacementMatrix.Right;
            XbimVector3D upDirection = shapePlacementMatrix.Up;
            
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();

            XbimVector3D rectangleCoordinates = displacement;
            double rectangleXDim = _PipeDiameter;
            double rectangleYDim = _PipeDiameter;
            double rectangleHeight = _height / 20;
            representationItems.Add(IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates, direction, refDirection));
            
            XbimVector3D stickCoordinates = rectangleCoordinates + rectangleHeight * direction;
            double stickRadius = _PipeDiameter / 10;
            double stickHeight = _height / 3;
            representationItems.Add(IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates, direction, refDirection));

            if (hasSpring)
            {
                double springRadius = stickRadius * 2;
                double springWireRadius = stickRadius / 2;
                const int springNumTurns = 5;
                IfcCartesianPoint[] spiralPoints = IfcVertexGeometry.CreateSpiral(model, springRadius, stickHeight, _numSegments, springNumTurns, stickCoordinates, refDirection, upDirection);
                IfcPolyline spiralPolyline = IfcVertexGeometry.CreatePolyline(model, spiralPoints);
                representationItems.Add(IfcGeometry.CreateSweptDiskSolid(model, spiralPolyline, springWireRadius));
            }
            
            XbimVector3D coneCoordinates = stickCoordinates + stickHeight * direction;
            XbimVector3D coneTopCoordinates = -_PipeDiameter / 2 * direction;
            double coneRadius = _PipeDiameter / 4;
            double coneHeight = Math.Sqrt((coneTopCoordinates - coneCoordinates).Modulus);
            representationItems.Add(IfcVertexGeometry.CreateCone(model, coneRadius, coneHeight, coneCoordinates, _numSegments, refDirection, upDirection));

            return representationItems;
        }
    }
}