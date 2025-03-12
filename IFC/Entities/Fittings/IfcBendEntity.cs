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
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.QuantityResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Fittings
{
    public sealed class IfcBendEntity : IfcAbstractFittingEntity
    {
        public double Length => Angle * _bendEntity.Radius;
        
        private readonly StartBendEntity _bendEntity;
        private readonly XbimVector3D[] _directionToPipes;
        private IfcPipeFitting _pipeFitting;

        public IfcBendEntity(StartBendEntity bendEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities)
            : base(bendEntity, ifcNodeEntity, ifcAbstractSegmentEntities)
        {
            _bendEntity = bendEntity;
            _directionToPipes = CalculateDirectionToPipes();
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointObjectPlacement(model, ObjectMatrix3D);

            IfcSurfaceCurveSweptAreaSolid sweptAreaSolid = CreateBendShape(model, ObjectMatrix3D, Angle, Diameter / 2);
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
            XbimVector3D[] pipesDirection = _IfcAbstractSegmentEntities.Select(pipe => pipe.ObjectMatrix3D.Forward).ToArray();
            return pipesDirection[0].Angle(pipesDirection[1]);
        }

        private XbimVector3D[] CalculateDirectionToPipes()
        {
            XbimVector3D coordinates = IfcNodeEntity.ObjectMatrix3D.Translation;
            return _IfcAbstractSegmentEntities.Select(pipe => IfcAxis.GetDirectionToPipe(pipe, coordinates)).ToArray();
        }

        private XbimMatrix3D CreateObjectMatrix(XbimVector3D[] directionToPipes)
        {
            XbimVector3D coordinates = IfcNodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = directionToPipes[0].Negated();
            XbimVector3D up = XbimVector3D.CrossProduct(forward, directionToPipes[1]).Normalized();

            return XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }

        private IfcSurfaceCurveSweptAreaSolid CreateBendShape(IModel model, XbimMatrix3D objectPlacement, double angle, double radius)
        {
            XbimVector3D circleCenter = CalculateCircleCenter();
            
            IfcCircle circle = IfcGeometry.CreateCircle(model, _bendEntity.Radius, circleCenter, objectPlacement.Up, objectPlacement.Right);
            IfcTrimmedCurve trimmedCurve = IfcGeometry.CreateTrimmedCurve(model, circle, 0, angle);
            IfcPlane plane = IfcGeometry.CreatePlane(model, objectPlacement.Translation, objectPlacement.Up);
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
            double lengthToCenter = _bendEntity.Radius / Math.Cos(Angle / 2);
            return dirToCenter * lengthToCenter;
        }

        private void ClipConnectedPipes()
        {
            double clipLength = _bendEntity.Radius * Math.Tan(Angle / 2);
            foreach (var ifcPipeEntity in _IfcAbstractSegmentEntities)
            {
                ifcPipeEntity.Clip(IfcNodeEntity, clipLength);
            }
        }

        protected override void AddProperties(IModel model, IfcProduct product)
        {
            base.AddProperties(model, product);

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