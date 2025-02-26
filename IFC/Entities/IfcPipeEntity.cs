using System;
using IFC.Entities.Abstract;
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
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.QuantityResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC.Entities
{
    public class IfcPipeEntity : IfcAbstractEntity
    {
        private event Action? _onDepthChanged;
        private event Action? _onCoordinatesChanged;

        private StartPipeEntity _pipeEntity;
        private IfcPipeSegment _pipeSegment;
        
        protected override IfcIdentifier Tag { get; set; } = "Pipe";
        
        public IfcNodeEntity[] NodeEntities { get; }
        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public double Diameter { get; }
        public IfcDistributionPort[] Ports { get; }

        public double Depth
        {
            get => _depth;
            set
            {
                _depth = value;
                _onDepthChanged?.Invoke();
            }
        }
        public XbimVector3D Coordinates
        {
            get => _coordinates;
            set
            {
                _coordinates = value;
                _onCoordinatesChanged?.Invoke();
            }
        }
    
        private double _depth;
        private XbimVector3D _coordinates;

        public IfcPipeEntity(StartPipeEntity pipeEntity, IfcNodeEntity[] nodeEntities)
        {
            _pipeEntity = pipeEntity;
            NodeEntities = nodeEntities;

            XbimVector3D coordinates = nodeEntities[0].ObjectMatrix3D.Translation;
            XbimVector3D pipeProjection = new XbimVector3D(
                _pipeEntity.ProjectionAlongOXAxis,
                _pipeEntity.ProjectionAlongOYAxis,
                _pipeEntity.ProjectionAlongOZAxis
            );

            XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
            XbimVector3D forward = pipeProjection.Normalized();
            if (forward == WorldUp || forward == -1 * WorldUp) 
                WorldUp = new XbimVector3D(0, 1, 0);
            XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);
        
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
            Coordinates = ObjectMatrix3D.Translation;
            
            Diameter = _pipeEntity.Diameter;
            Depth = pipeProjection.Length;

            Ports = new IfcDistributionPort[2];
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcCartesianPoint startPoint = CreateStartPoint(model);
            IfcCartesianPoint endPoint = CreateEndPoint(model);
        
            IfcDirection forwardDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Forward);
            IfcDirection rightDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Right);

            IfcAxis2Placement3D startAxis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, startPoint, forwardDirection, rightDirection);
            IfcAxis2Placement3D endAxis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, endPoint, forwardDirection, rightDirection);

            IfcLocalPlacement startLocalPlacement = IfcAxis.CreateLocalPlacement(model, startAxis2Placement3D);
            IfcLocalPlacement endLocalPlacement = IfcAxis.CreateLocalPlacement(model, endAxis2Placement3D);

            IfcDirection extrudedDirection = IfcAxis.CreateDirection(model, new XbimVector3D(0, 0, 1));
            IfcProductDefinitionShape productDefShape = CreatePipeShape(model, extrudedDirection);
            _pipeSegment = CreatePipe(model, productDefShape, startLocalPlacement);
            AddProperties(model, _pipeSegment);

            Ports[0] = CreatePort(model, startLocalPlacement);
            Ports[1] = CreatePort(model, endLocalPlacement);
            IfcPortConnection.ConnectPorts(model, Ports, _pipeSegment);

            return _pipeSegment;
        }

        public void Clip(IfcNodeEntity nodeEntity, double clipLength)
        {
            if (IsStartNode(nodeEntity))
                Coordinates += ObjectMatrix3D.Forward * clipLength;
            Depth -= clipLength;
        }

        private IfcDistributionPort CreatePort(IModel model, IfcLocalPlacement localPlacement)
        {
            return model.Instances.New<IfcDistributionPort>(port =>
            {
                port.Name = "Port";
                port.ObjectPlacement = localPlacement;
                port.PredefinedType = IfcDistributionPortTypeEnum.PIPE;
                port.FlowDirection = IfcFlowDirectionEnum.SOURCEANDSINK;
                port.SystemType = IfcDistributionSystemEnum.FUEL;
            });
        }

        private bool IsStartNode(IfcNodeEntity nodeEntity)
        {
            XbimVector3D nodeCoordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D startPipeCoordinates = ObjectMatrix3D.Translation;
            XbimVector3D endPipeCoordinates = ObjectMatrix3D.Translation + ObjectMatrix3D.Forward * Depth;

            return (nodeCoordinates - startPipeCoordinates).Length < (nodeCoordinates - endPipeCoordinates).Length;
        }

        private IfcCartesianPoint CreateStartPoint(IModel model)
        {
            return model.Instances.New<IfcCartesianPoint>(point =>
            {
                point.SetXYZ(Coordinates.X, Coordinates.Y, Coordinates.Z);
                _onCoordinatesChanged += () => point.SetXYZ(Coordinates.X, Coordinates.Y, Coordinates.Z);
            });
        }

        private IfcCartesianPoint CreateEndPoint(IModel model)
        {
            return model.Instances.New<IfcCartesianPoint>(point =>
            {
                XbimVector3D endCoordinates = Coordinates + ObjectMatrix3D.Forward * Depth;
                point.SetXYZ(endCoordinates.X, endCoordinates.Y, endCoordinates.Z);
            
                _onCoordinatesChanged += () =>
                {
                    endCoordinates = Coordinates + ObjectMatrix3D.Forward * Depth;
                    point.SetXYZ(endCoordinates.X, endCoordinates.Y, endCoordinates.Z);
                };
                _onDepthChanged += () =>
                {
                    endCoordinates = Coordinates + ObjectMatrix3D.Forward * Depth;
                    point.SetXYZ(endCoordinates.X, endCoordinates.Y, endCoordinates.Z);
                };
            });
        }

        private IfcProductDefinitionShape CreatePipeShape(IModel model, IfcDirection extrudedDirection)
        {
            IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, Diameter / 2, XbimVector3D.Zero);
            IfcExtrudedAreaSolid extrudedArea = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = extrudedDirection;
                solid.Depth = Depth;

                _onDepthChanged += () => solid.Depth = Depth;
            });
            IfcShapeRepresentation shapeRep = IfcGeometry.CreateShapeRepresentation(model, extrudedArea);
        
            return IfcGeometry.CreateProductDefinitionShape(model, shapeRep);
        }

        private IfcPipeSegment CreatePipe(IModel model, IfcProductDefinitionShape productDefShape, IfcLocalPlacement localPlacement)
        {
            return model.Instances.New<IfcPipeSegment>(segment =>
            {
                segment.Name = _pipeEntity.Name;
                segment.Tag = Tag;
                segment.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
                segment.ObjectPlacement = localPlacement;
                segment.Representation = productDefShape;
            });
        }

        protected override void AddProperties(IModel model, IfcProduct product)
        {
            base.AddProperties(model, product);
        
            #region Pset_PipeSegmentTypeStart

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "Pset_PipeSegmentTypeStart";
                    foreach (var kvp in _pipeEntity.GetData())
                    {
                        set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                        {
                            value.Name = kvp.Key;
                            value.NominalValue = new IfcText(kvp.Value);
                        }));
                    }
                });
            });

            #endregion
        
            #region Pset_PipeSegmentTypeCommon

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "Pset_PipeSegmentTypeCommon";
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "InnerDiameter";
                        value.NominalValue = new IfcPositiveLengthMeasure(_pipeEntity.Diameter - _pipeEntity.WallThickness);
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "OuterDiameter";
                        value.NominalValue = new IfcPositiveLengthMeasure(_pipeEntity.Diameter);
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "WorkingPressure";
                        value.NominalValue = new IfcPressureMeasure(ValueConverter.ValueConverter.T_m2ToPa(_pipeEntity.Pressure));
                    }));
                });
            });

            #endregion
        
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
                        length.LengthValue = new IfcLengthMeasure(Depth);
                    
                        _onDepthChanged += () => length.LengthValue = new IfcLengthMeasure(Depth);
                    }));
                    quantity.Quantities.Add(model.Instances.New<IfcQuantityWeight>(weight =>
                    {
                        weight.Name = "NetWeight";
                        weight.WeightValue = new IfcMassMeasure(ValueConverter.ValueConverter.TfToKg(_pipeEntity.PipeUnitWeight) * Depth);
                    
                        _onDepthChanged += () => weight.WeightValue = new IfcMassMeasure(_pipeEntity.PipeUnitWeight * Depth);
                    }));
                    quantity.Quantities.Add(model.Instances.New<IfcQuantityArea>(area =>
                    {
                        double circumference = Math.PI * _pipeEntity.Diameter;
                        area.Name = "OuterSurfaceArea";
                        area.AreaValue = new IfcAreaMeasure(circumference * Depth);

                        _onDepthChanged += () => area.AreaValue = new IfcAreaMeasure(circumference * Depth);
                    }));
                });
            });

            #endregion
        }
    }
}