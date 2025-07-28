using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract.Segments;
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

namespace IFC.Entities.Abstract.Anchors
{
    #if NEW

    public abstract class IfcAbstractNonStandardRestraintEntity : IfcAbstractNonFixedSupportEntity
    {
        public abstract StartNonStandardRestraint NonStandardRestraint { get; }
        public abstract int NumSegments { get; }
        public abstract double Height { get; }

        private IfcAbstractSegmentEntity[] _abstractSegmentEntities;
        
        protected IfcAbstractNonStandardRestraintEntity(XbimMatrix3D objectMatrix) : base(objectMatrix) { }
        
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

            _abstractSegmentEntities = ConnectedEntities.OfType<IfcAbstractSegmentEntity>().ToArray();

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchorModel(model, Diameter / 2 * VectorExtensions.Forward);
            AddShapeRepresentation(model, discreteAccessory, representationItems);
            
            return discreteAccessory;
        }
        
        protected override IEnumerable<IfcRepresentationItem> CreateAnchorModel(IModel model, XbimVector3D displacement)
        {
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();

            foreach (StartNonStandardRestraintModule restraintModule in NonStandardRestraint.Restraints)
            {
                bool hasSpring = restraintModule.Type == StartRestraintTypeEnum.ELASTIC;
                bool isDoubleSided = restraintModule.Type != StartRestraintTypeEnum.RIGID_ONE_SIDED;

                XbimVector3D direction = CreateAnchorDirection(restraintModule);
                IEnumerable<XbimVector3D> segmentDirections = _abstractSegmentEntities.Select(entity => entity.ObjectMatrix3D.Value.Forward);
                bool isParallel = segmentDirections.Any(segmentDirection => segmentDirection.IsParallel(direction));

                XbimVector3D[] displacements;
                if (isParallel)
                {
                    displacements = new XbimVector3D[]
                    {
                        displacement - Height * direction + Diameter * VectorExtensions.Z,
                        displacement + Height * direction + Diameter * VectorExtensions.Z
                    };
                }
                else
                {
                    displacements = new XbimVector3D[]
                    {
                        displacement - Height * direction,
                        displacement + Height * direction
                    };
                }
                representationItems.AddRange(CreateSingleAnchorShape(model, direction, displacements[0], hasSpring));
                if (isDoubleSided)
                {
                    representationItems.AddRange(CreateSingleAnchorShape(model, direction.Negated(), displacements[1], hasSpring));
                }
            }

            return representationItems;
        }

        private XbimVector3D CreateAnchorDirection(StartNonStandardRestraintModule restraintModule)
        {
            double restraintX = Math.Cos(restraintModule.AngleX.SIProperty);
            double restraintY = Math.Cos(restraintModule.AngleY.SIProperty);
            double restraintZ = Math.Cos(restraintModule.AngleZ.SIProperty);
            
            bool useLocalAxes = restraintModule.Local == StartRestraintAxesTypeEnum.LOCAL;
            if (!useLocalAxes)
            {
                XbimVector3D restraintVector = new XbimVector3D(restraintX, restraintY, restraintZ);
                return restraintVector;
            }
            
            foreach (IfcAbstractSegmentEntity segmentEntity in _abstractSegmentEntities)
            {
                if (segmentEntity.NodeEntities.All(item => item.ID != NonStandardRestraint.SectionStartNode) ||
                    segmentEntity.NodeEntities.All(item => item.ID != NonStandardRestraint.SectionEndNode))
                {
                    continue;
                }
                
                IfcNodeEntity startNode = segmentEntity.NodeEntities.First(item => item.ID == NonStandardRestraint.SectionStartNode);
                IfcNodeEntity endNode = segmentEntity.NodeEntities.First(item => item.ID == NonStandardRestraint.SectionEndNode);
                
                XbimVector3D forward = endNode.ObjectMatrix3D.Translation - startNode.ObjectMatrix3D.Translation;
                XbimMatrix3D fictiveObjectMatrix = MatrixExtensions.CreateWorld(XbimVector3D.Zero, forward);
                XbimVector3D restraintVector = fictiveObjectMatrix.Forward * restraintX +
                                               fictiveObjectMatrix.Right * restraintY +
                                               fictiveObjectMatrix.Up * restraintZ;
                return restraintVector.Negated();
            }
            
            throw new NullReferenceException(nameof(IfcAbstractNonStandardRestraintEntity) + "Cannot find local axes");
        }

        private IEnumerable<IfcRepresentationItem> CreateSingleAnchorShape(IModel model, XbimVector3D direction, XbimVector3D displacement, bool hasSpring)
        {
            XbimMatrix3D shapePlacementMatrix = MatrixExtensions.CreateWorld(displacement, direction.Negated());
            XbimVector3D refDirection = shapePlacementMatrix.Right;
            XbimVector3D upDirection = shapePlacementMatrix.Up;
            
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();

            XbimVector3D rectangleCoordinates = displacement;
            double rectangleXDim = Diameter;
            double rectangleYDim = Diameter;
            double rectangleHeight = Height / 20;
            representationItems.Add(IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates, direction, refDirection));
            
            XbimVector3D stickCoordinates = rectangleCoordinates + rectangleHeight * direction;
            double stickRadius = Diameter / 10;
            double stickHeight = Height / 3;
            representationItems.Add(IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates, direction, refDirection));

            if (hasSpring)
            {
                double springRadius = stickRadius * 2;
                double springWireRadius = stickRadius / 2;
                const int springNumTurns = 5;
                IfcCartesianPoint[] spiralPoints = IfcVertexGeometry.CreateSpiral(model, springRadius, stickHeight, NumSegments, springNumTurns, stickCoordinates, refDirection, upDirection);
                IfcPolyline spiralPolyline = IfcVertexGeometry.CreatePolyline(model, spiralPoints);
                representationItems.Add(IfcGeometry.CreateSweptDiskSolid(model, spiralPolyline, springWireRadius));
            }
            
            XbimVector3D coneCoordinates = stickCoordinates + stickHeight * direction;
            XbimVector3D coneTopCoordinates = -Diameter / 2 * direction;
            double coneRadius = Diameter / 4;
            double coneHeight = Math.Sqrt((coneTopCoordinates - coneCoordinates).Modulus);
            representationItems.Add(IfcVertexGeometry.CreateCone(model, coneRadius, coneHeight, coneCoordinates, NumSegments, refDirection, upDirection));

            return representationItems;
        }
    }
    
    #else
    
    public abstract class IfcAbstractNonStandardRestraintEntity : IfcAbstractNonFixedSupportEntity
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double Height { get; protected set; }

        private readonly StartNonStandardRestraint _nonStandardRestraint;
        private IfcDiscreteAccessory? _discreteAccessory;
        
        protected IfcAbstractNonStandardRestraintEntity(StartNonStandardRestraint nonStandardRestraint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(nonStandardRestraint, nodeEntity, segmentEntities)
        {
            _nonStandardRestraint = nonStandardRestraint;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchorModel(model, XbimVector3D.Zero);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, representationItems);

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
                IEnumerable<XbimVector3D> segmentDirections = AbstractSegmentEntities.Select(entity => entity.Direction);
                bool isParallel = segmentDirections.Any(segmentDirection => segmentDirection.IsParallel(direction));

                XbimVector3D[] displacements;
                if (isParallel)
                {
                    displacements = new XbimVector3D[]
                    {
                        displacement - Height * direction + Diameter * VectorExtensions.Z,
                        displacement + Height * direction + Diameter * VectorExtensions.Z
                    };
                }
                else
                {
                    displacements = new XbimVector3D[]
                    {
                        displacement - Height * direction,
                        displacement + Height * direction
                    };
                }
                representationItems.AddRange(CreateSingleAnchorShape(model, direction, displacements[0], hasSpring));
                if (isDoubleSided)
                {
                    representationItems.AddRange(CreateSingleAnchorShape(model, direction.Negated(), displacements[1], hasSpring));
                }
            }

            return representationItems;
        }

        private XbimVector3D CreateAnchorDirection(StartNonStandardRestraintModule restraintModule)
        {
            double restraintX = Math.Cos(restraintModule.AngleX.SIProperty);
            double restraintY = Math.Cos(restraintModule.AngleY.SIProperty);
            double restraintZ = Math.Cos(restraintModule.AngleZ.SIProperty);
            
            bool useLocalAxes = restraintModule.Local == StartRestraintAxesTypeEnum.LOCAL;
            if (!useLocalAxes)
            {
                XbimVector3D restraintVector = new XbimVector3D(restraintX, restraintY, restraintZ);
                return restraintVector;
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
                XbimVector3D restraintVector = fictiveObjectMatrix.Forward * restraintX +
                                               fictiveObjectMatrix.Right * restraintY +
                                               fictiveObjectMatrix.Up * restraintZ;
                return restraintVector.Negated();
            }
            
            throw new NullReferenceException(nameof(IfcAbstractNonStandardRestraintEntity) + "Cannot find local axes");
        }

        private IEnumerable<IfcRepresentationItem> CreateSingleAnchorShape(IModel model, XbimVector3D direction, XbimVector3D displacement, bool hasSpring)
        {
            XbimMatrix3D shapePlacementMatrix = MatrixExtensions.CreateWorld(displacement, direction.Negated());
            XbimVector3D refDirection = shapePlacementMatrix.Right;
            XbimVector3D upDirection = shapePlacementMatrix.Up;
            
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();

            XbimVector3D rectangleCoordinates = displacement;
            double rectangleXDim = Diameter;
            double rectangleYDim = Diameter;
            double rectangleHeight = Height / 20;
            representationItems.Add(IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates, direction, refDirection));
            
            XbimVector3D stickCoordinates = rectangleCoordinates + rectangleHeight * direction;
            double stickRadius = Diameter / 10;
            double stickHeight = Height / 3;
            representationItems.Add(IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates, direction, refDirection));

            if (hasSpring)
            {
                double springRadius = stickRadius * 2;
                double springWireRadius = stickRadius / 2;
                const int springNumTurns = 5;
                IfcCartesianPoint[] spiralPoints = IfcVertexGeometry.CreateSpiral(model, springRadius, stickHeight, NumSegments, springNumTurns, stickCoordinates, refDirection, upDirection);
                IfcPolyline spiralPolyline = IfcVertexGeometry.CreatePolyline(model, spiralPoints);
                representationItems.Add(IfcGeometry.CreateSweptDiskSolid(model, spiralPolyline, springWireRadius));
            }
            
            XbimVector3D coneCoordinates = stickCoordinates + stickHeight * direction;
            XbimVector3D coneTopCoordinates = -Diameter / 2 * direction;
            double coneRadius = Diameter / 4;
            double coneHeight = Math.Sqrt((coneTopCoordinates - coneCoordinates).Modulus);
            representationItems.Add(IfcVertexGeometry.CreateCone(model, coneRadius, coneHeight, coneCoordinates, NumSegments, refDirection, upDirection));

            return representationItems;
        }
    }

    #endif
}