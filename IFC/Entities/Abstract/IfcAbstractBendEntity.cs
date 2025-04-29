using System;
using System.Linq;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.QuantityResource;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractBendEntity : IfcAbstractFittingEntity
    {
        public sealed override double Length { get; protected set; }
        
        protected double _BendRadius;
        protected double _PipeRadius;
        
        private readonly StartBendEntity _bendEntity;
        private readonly XbimVector3D[] _directionToPipes;

        public IfcAbstractBendEntity(StartBendEntity bendEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(bendEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            _bendEntity = bendEntity;
            _directionToPipes = CalculateDirectionToPipes();
            
            _BendRadius = _bendEntity.Radius;
            _PipeRadius = Math.Min(AbstractSegmentEntities[0].OuterDiameter / 2, AbstractSegmentEntities[1].OuterDiameter / 2);

            ObjectMatrix3D = ObjectMatrix3D.Translate(CalculateCircleCenter());
            Length = Angle * _BendRadius;
        }

        protected XbimVector3D[] CalculateDirectionToPipes()
        {
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            return AbstractSegmentEntities.Select(pipe => IfcAxis.GetDirectionToPipe(pipe, coordinates)).ToArray();
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
            foreach (IfcAbstractSegmentEntity ifcPipeEntity in AbstractSegmentEntities)
            {
                ifcPipeEntity.Clip(NodeEntity, clipLength);
            }
        }

        protected override void AddProperties(IModel model, IfcProduct product)
        {
            base.AddProperties(model, product);
            
            #region Pset_PipeFittingTypeBend

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "Pset_PipeFittingTypeBend";
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "BendAngle";
                        value.NominalValue = new IfcPositivePlaneAngleMeasure(Angle);
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "BendRadius";
                        value.NominalValue = new IfcPositiveLengthMeasure(_BendRadius);
                    }));
                });
            });

            #endregion
        }
    }
}