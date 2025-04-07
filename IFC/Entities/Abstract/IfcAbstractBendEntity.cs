using System;
using System.Linq;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.QuantityResource;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractBendEntity : IfcAbstractFittingEntity
    {
        public double Length { get; }
        
        protected double _BendRadius;
        protected double _PipeRadius;
        
        private readonly StartBendEntity _bendEntity;
        private readonly XbimVector3D[] _directionToPipes;

        public IfcAbstractBendEntity(StartBendEntity bendEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities) 
            : base(bendEntity, ifcNodeEntity, ifcAbstractSegmentEntities)
        {
            _bendEntity = bendEntity;
            _directionToPipes = CalculateDirectionToPipes();
            
            _BendRadius = _bendEntity.Radius;
            _PipeRadius = Math.Min(_IfcAbstractSegmentEntities[0].Diameter / 2, _IfcAbstractSegmentEntities[1].Diameter / 2);

            ObjectMatrix3D = ObjectMatrix3D.Translate(CalculateCircleCenter());
            Length = Angle * _BendRadius;
        }
        
        protected XbimVector3D[] CalculateDirectionToPipes()
        {
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            return _IfcAbstractSegmentEntities.Select(pipe => IfcAxis.GetDirectionToPipe(pipe, coordinates)).ToArray();
        }

        private XbimVector3D CalculateCircleCenter()
        {
            XbimVector3D dirToCenter = (_directionToPipes[0].Normalized() + _directionToPipes[1].Normalized()).Normalized();
            double lengthToCenter = _bendEntity.Radius / Math.Cos(Angle / 2);
            return dirToCenter * lengthToCenter;
        }
        
        protected void ClipConnectedPipes()
        {
            double clipLength = _bendEntity.Radius * Math.Tan(Angle / 2);
            foreach (IfcAbstractSegmentEntity ifcPipeEntity in _IfcAbstractSegmentEntities)
            {
                ifcPipeEntity.Clip(NodeEntity, clipLength);
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