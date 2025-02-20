using System;
using System.Linq;
using IFC.Entities.Abstract;
using IFC.Tools;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
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

namespace IFC.Entities
{
    public class IfcBendEntity : IfcAbstractEntity
    {
        protected override IfcIdentifier Tag { get; set; } = "Bend";
    
        private readonly StartBendEntity _bendEntity;
        private readonly IfcNodeEntity _ifcNodeEntity;
        private readonly IfcPipeEntity[] _ifcPipeEntities;

        private IfcPipeFitting _pipeFitting;
    
        private readonly double _pipeAngle;

        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public XbimVector3D[] PipesDirection { get; }
        public XbimVector3D[] DirectionToPipes { get; }

        public double Length => _pipeAngle * _bendEntity.Radius;

        public IfcBendEntity(StartBendEntity bendEntity, IfcNodeEntity ifcNodeEntity, IfcPipeEntity[] ifcPipeEntities)
        {
            _bendEntity = bendEntity;
            _ifcPipeEntities = ifcPipeEntities;
            _ifcNodeEntity = ifcNodeEntity;
            _ifcNodeEntity.ConnEntities.Add(this);

            XbimVector3D coordinates = _ifcNodeEntity.ObjectMatrix3D.Translation;
            PipesDirection = ifcPipeEntities.Select(pipe => pipe.ObjectMatrix3D.Forward).ToArray();
            DirectionToPipes = ifcPipeEntities.Select(pipe => IfcAxis.GetDirectionToPipe(pipe, coordinates)).ToArray();

            XbimVector3D upDirection = XbimVector3D.CrossProduct(DirectionToPipes[0] * -1, DirectionToPipes[1]).Normalized();
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, DirectionToPipes[0] * -1, upDirection);
        
            _pipeAngle = PipesDirection[0].Angle(PipesDirection[1]);
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcSurfaceCurveSweptAreaSolid sweptAreaSolid = CreateBendShape(model);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, sweptAreaSolid);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            _pipeFitting = CreateBend(model, shape);
            AddProperties(model, _pipeFitting);
            ClipConnectedPipes();
            ConnectPorts(model);

            return _pipeFitting;
        }

        private IfcRelConnectsPorts ConnectPorts(IModel model)
        {
            var closestPorts = (
                from port in _ifcPipeEntities.SelectMany(pipe => pipe.Ports)
                let distance = (port.ObjectPlacement.ToMatrix3D().Translation - ObjectMatrix3D.Translation).Length
                orderby distance
                select port
            ).Take(2).ToArray();

            return model.Instances.New<IfcRelConnectsPorts>(ports =>
            {
                ports.Name = $"{closestPorts[0].GlobalId}|{closestPorts[1].GlobalId}";
                ports.Description = "Flow";
                ports.RelatingPort = closestPorts[0];
                ports.RelatedPort = closestPorts[1];
                ports.RealizingElement = _pipeFitting;
            });
        }

        private IfcPipeFitting CreateBend(IModel model, IfcProductDefinitionShape shape)
        {
            IfcCartesianPoint point = IfcAxis.CreatePoint(model, ObjectMatrix3D.Translation);
            IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point);
            IfcLocalPlacement localPlacement = IfcAxis.CreateLocalPlacement(model, axis2Placement3D);
        
            return model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _bendEntity.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;
                fitting.Representation = shape;
                fitting.ObjectPlacement = localPlacement;
            });
        }

        private IfcSurfaceCurveSweptAreaSolid CreateBendShape(IModel model)
        {
            XbimVector3D circleCenter = CalculateCircleCenter();
        
            IfcCircle circle = IfcGeometry.CreateCircle(model, _bendEntity.Radius, circleCenter, ObjectMatrix3D.Up, ObjectMatrix3D.Right);
            IfcTrimmedCurve trimmedCurve = IfcGeometry.CreateTrimmedCurve(model, circle, 0, _pipeAngle);
            IfcPlane plane = IfcGeometry.CreatePlane(model, ObjectMatrix3D.Translation, ObjectMatrix3D.Up);
            IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, _ifcPipeEntities[0].Diameter / 2, XbimVector3D.Zero, new XbimVector3D(1, 0, 0));

            return model.Instances.New<IfcSurfaceCurveSweptAreaSolid>(solid =>
            {
                solid.SweptArea = profileDef;
                solid.Directrix = trimmedCurve;
                solid.ReferenceSurface = plane;
            });
        }

        private XbimVector3D CalculateCircleCenter()
        {
            XbimVector3D dirToCenter = (DirectionToPipes[0].Normalized() + DirectionToPipes[1].Normalized()).Normalized();
            double lengthToCenter = _bendEntity.Radius / Math.Cos(_pipeAngle / 2);
            return dirToCenter * lengthToCenter;
        }

        private void ClipConnectedPipes()
        {
            double clipLength = _bendEntity.Radius * Math.Tan(_pipeAngle / 2);
            foreach (var ifcPipeEntity in _ifcPipeEntities)
            {
                ifcPipeEntity.Clip(_ifcNodeEntity, clipLength);
            }
        }

        private XbimVector3D CalculateAlternateCircleCenter()
        {
            double lengthToCenter = _bendEntity.Radius * Math.Tan(_pipeAngle / 2);
            XbimVector3D dirToCenter = new XbimVector3D(-1, 0, 0);
        
            return dirToCenter * lengthToCenter;
        }

        private IfcRevolvedAreaSolid CreateAlternateBendShape(IModel model)
        {
            XbimVector3D circleCenter = CalculateAlternateCircleCenter();

            IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(
                model,
                _ifcPipeEntities[0].Diameter / 2,
                XbimVector3D.Zero,
                new XbimVector3D(1, 0, 0)
            );
        
            double lengthToCenter = _bendEntity.Radius* Math.Tan(_pipeAngle / 2);

            IfcRevolvedAreaSolid sweptAreaSolid = model.Instances.New<IfcRevolvedAreaSolid>(solid =>
            {
                solid.SweptArea = profileDef;
                solid.Axis = IfcAxis.CreateAxis1Placement(model, circleCenter, new XbimVector3D(0, -1, 0));
                solid.Angle = new IfcPlaneAngleMeasure(_pipeAngle);
                solid.Position = IfcAxis.CreateAxis2Placement3D(model, DirectionToPipes[0] * lengthToCenter, ObjectMatrix3D.Forward, ObjectMatrix3D.Right);
            });

            return sweptAreaSolid;
        }

        protected override void AddProperties(IModel model, IfcProduct product)
        {
            base.AddProperties(model, product);
        
            #region Pset_PipeFittingTypeStart

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "Pset_PipeFittingTypeStart";
                    foreach (var kvp in _bendEntity.GetData())
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

            #region Qto_PipeFittingBaseQuantities

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcElementQuantity>(quantity =>
                {
                    quantity.Name = "Qto_PipeFittingBaseQuantities";
                    quantity.Quantities.Add(model.Instances.New<IfcQuantityLength>(length =>
                    {
                        length.Name = "Length";
                        length.LengthValue = Length;
                        length.Formula = "radius*angle; [angle]=rad, [radius]=metre";
                    }));
                    quantity.Quantities.Add(model.Instances.New<IfcQuantityWeight>(weight =>
                    {
                        weight.Name = "NetWeight";
                        weight.WeightValue = ValueConverter.ValueConverter.TfToKg(_bendEntity.Weight * Length);
                    }));
                });
            });

            #endregion
        }
    }
}