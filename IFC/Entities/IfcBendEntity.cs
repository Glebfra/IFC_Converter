using System;
using System.Linq;
using IFC.Entities.Abstract;
using IFC.Tools;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
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
    public class IfcBendEntity : IfcAbstractEntity
    {
        public override IfcIdentifier Tag { get; protected set; } = "Bend";
    
        private readonly StartBendEntity _bendEntity;
        private readonly IfcNodeEntity _ifcNodeEntity;
        private readonly IfcAbstractSegmentEntity[] _ifcPipeEntities;
        private readonly XbimVector3D[] _directionToPipes;
        private readonly double _bendAngle;
        
        private IfcPipeFitting _pipeFitting;

        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public double Length => _bendAngle * _bendEntity.Radius;
        
        public IfcBendEntity(StartBendEntity bendEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcPipeEntities)
        {
            _bendEntity = bendEntity;
            _ifcPipeEntities = ifcPipeEntities;
            _ifcNodeEntity = ifcNodeEntity;

            _directionToPipes = CalculateDirectionToPipes();
            _bendAngle = CalculateBendAngle();
            
            ObjectMatrix3D = CreateObjectMatrix(_directionToPipes);
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointObjectPlacement(model, ObjectMatrix3D);

            IfcSurfaceCurveSweptAreaSolid sweptAreaSolid = CreateBendShape(model, ObjectMatrix3D, _bendAngle, _ifcPipeEntities[0].Diameter / 2);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, sweptAreaSolid);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);

            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _bendEntity.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });

            AddProperties(model, _pipeFitting);
            ClipConnectedPipes();

            return _pipeFitting;
        }

        private double CalculateBendAngle()
        {
            XbimVector3D[] pipesDirection = _ifcPipeEntities.Select(pipe => pipe.ObjectMatrix3D.Forward).ToArray();
            return pipesDirection[0].Angle(pipesDirection[1]);
        }

        private XbimVector3D[] CalculateDirectionToPipes()
        {
            XbimVector3D coordinates = _ifcNodeEntity.ObjectMatrix3D.Translation;
            return _ifcPipeEntities.Select(pipe => IfcAxis.GetDirectionToPipe(pipe, coordinates)).ToArray();
        }

        private XbimMatrix3D CreateObjectMatrix(XbimVector3D[] directionToPipes)
        {
            XbimVector3D coordinates = _ifcNodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D up = XbimVector3D.CrossProduct(directionToPipes[0] * -1, directionToPipes[1]).Normalized();
            XbimVector3D forward = directionToPipes[0] * -1;
            
            return XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }

        private IfcSurfaceCurveSweptAreaSolid CreateBendShape(IModel model, XbimMatrix3D ObjectPlacement, double angle, double radius)
        {
            XbimVector3D circleCenter = CalculateCircleCenter();
            
            IfcCircle circle = IfcGeometry.CreateCircle(model, _bendEntity.Radius, circleCenter, ObjectPlacement.Up, ObjectPlacement.Right);
            IfcTrimmedCurve trimmedCurve = IfcGeometry.CreateTrimmedCurve(model, circle, 0, angle);
            IfcPlane plane = IfcGeometry.CreatePlane(model, ObjectPlacement.Translation, ObjectPlacement.Up);
            IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, radius, XbimVector3D.Zero, new XbimVector3D(1, 0, 0));

            return model.Instances.New<IfcSurfaceCurveSweptAreaSolid>(solid =>
            {
                solid.SweptArea = profileDef;
                solid.Directrix = trimmedCurve;
                solid.ReferenceSurface = plane;
            });
        }

        private XbimVector3D CalculateCircleCenter()
        {
            XbimVector3D dirToCenter = (_directionToPipes[0].Normalized() + _directionToPipes[1].Normalized()).Normalized();
            double lengthToCenter = _bendEntity.Radius / Math.Cos(_bendAngle / 2);
            return dirToCenter * lengthToCenter;
        }

        private void ClipConnectedPipes()
        {
            double clipLength = _bendEntity.Radius * Math.Tan(_bendAngle / 2);
            foreach (var ifcPipeEntity in _ifcPipeEntities)
            {
                ifcPipeEntity.Clip(_ifcNodeEntity, clipLength);
            }
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