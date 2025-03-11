using System;
using IFC.Entities.Fittings;
using IFC.Entities.Interfaces;
using IFC.Tools;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.QuantityResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractSegmentEntity : IfcAbstractEntity, IIfcClippable, IIfcTwoNodeEntity
    {
        public abstract XbimVector3D Direction { get; }
        public abstract double Diameter { get; }

        public IfcNodeEntity[] NodeEntities { get; }

        public XbimVector3D Coordinates
        {
            get => _coordinates;
            set
            {
                _coordinates = value;
                _CoordinatesChanged?.Invoke();
            }
        }

        public double Length
        {
            get => _length;
            set
            {
                _length = value;
                _LengthChanged?.Invoke();
            }
        }
        
        protected event Action? _CoordinatesChanged;
        protected event Action? _LengthChanged;

        private double _length;
        private XbimVector3D _coordinates;
        
        private IfcPipeSegment _pipeSegment;

        public IfcAbstractSegmentEntity(StartAbstractEntity entity, IfcNodeEntity[] ifcNodeEntities)
            : base(entity)
        {
            NodeEntities = ifcNodeEntities;
        }
        
        public void Clip(IfcNodeEntity nodeEntity, double clipLength)
        {
            if (IsStartNode(nodeEntity))
                Coordinates += ObjectMatrix3D.Forward * clipLength;
            Length -= clipLength;
        }

        protected IfcPipeSegment CreatePipeSegment(IModel model, string name, IfcPipeSegmentTypeEnum type)
        {
            IfcCartesianPoint startPoint = CreateStartPoint(model);
            IfcDirection forwardDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Forward);
            IfcDirection rightDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Right);

            IfcAxis2Placement3D startAxis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, startPoint, forwardDirection, rightDirection);
            IfcLocalPlacement localPlacement = IfcAxis.CreateLocalPlacement(model, startAxis2Placement3D);

            IfcDirection extrudedDirection = IfcAxis.CreateDirection(model, new XbimVector3D(0, 0, 1));
            IfcProductDefinitionShape productDefShape = CreatePipeShape(model, extrudedDirection);
            _pipeSegment = CreatePipe(model, productDefShape, localPlacement, name, type);

            return _pipeSegment;
        }

        protected IfcPipeSegment CreatePipe(IModel model, IfcProductDefinitionShape productDefShape, IfcLocalPlacement localPlacement, string name, IfcPipeSegmentTypeEnum type)
        {
            return model.Instances.New<IfcPipeSegment>(segment =>
            {
                segment.Tag = Tag;
                segment.Name = name;
                segment.PredefinedType = type;
                segment.ObjectPlacement = localPlacement;
                segment.Representation = productDefShape;
            });
        }
        
        protected IfcProductDefinitionShape CreatePipeShape(IModel model, IfcDirection extrudedDirection)
        {
            IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, Diameter / 2, XbimVector3D.Zero);
            IfcExtrudedAreaSolid extrudedArea = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = extrudedDirection;
                solid.Depth = Length;

                _LengthChanged += () => solid.Depth = Length;
            });
            IfcShapeRepresentation shapeRep = IfcGeometry.CreateShapeRepresentation(model, extrudedArea);
        
            return IfcGeometry.CreateProductDefinitionShape(model, shapeRep);
        }

        protected IfcCartesianPoint CreateStartPoint(IModel model)
        {
            return model.Instances.New<IfcCartesianPoint>(point =>
            {
                point.SetXYZ(Coordinates.X, Coordinates.Y, Coordinates.Z);
                _CoordinatesChanged += () => point.SetXYZ(Coordinates.X, Coordinates.Y, Coordinates.Z);
            });
        }

        protected bool IsStartNode(IfcNodeEntity nodeEntity)
        {
            XbimVector3D nodeCoordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D startPipeCoordinates = ObjectMatrix3D.Translation;
            XbimVector3D endPipeCoordinates = ObjectMatrix3D.Translation + ObjectMatrix3D.Forward * Length;

            return (nodeCoordinates - startPipeCoordinates).Length < (nodeCoordinates - endPipeCoordinates).Length;
        }

        protected override void AddProperties(IModel model, IfcProduct product)
        {
            base.AddProperties(model, product);

            #region Qto_PipeSegmentBaseQuantities

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcElementQuantity>(quantity =>
                {
                    quantity.Name = "Qto_PipeSegmentBaseQuantities";
                    quantity.Quantities.Add(model.Instances.New<IfcQuantityLength>(length =>
                    {
                        length.Name = "Length";
                        length.LengthValue = new IfcLengthMeasure(Length);

                        _LengthChanged += () => length.LengthValue = new IfcLengthMeasure(Length);
                    }));
                    quantity.Quantities.Add(model.Instances.New<IfcQuantityArea>(area =>
                    {
                        double circumference = Math.PI * Diameter;
                        area.Name = "OuterSurfaceArea";
                        area.AreaValue = new IfcAreaMeasure(circumference * Length);

                        _LengthChanged += () => area.AreaValue = new IfcAreaMeasure(circumference * Length);
                    }));
                });
            });

            #endregion
        }
    }
}