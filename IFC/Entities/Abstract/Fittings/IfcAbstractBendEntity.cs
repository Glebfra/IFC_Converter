using System;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractBendEntity : IfcAbstractFittingEntity
    {
        public abstract double Angle { get; protected set; }
        public abstract double BendRadius { get; protected set; }
        public abstract double PipeRadius { get; protected set; }

        protected IfcAbstractBendEntity(StartAbstractFittingEntity fittingEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(fittingEntity, nodeEntity, segmentEntities)
        {
            
        }

        protected void ClipConnectedPipes()
        {
            double clipLength = BendRadius * Math.Tan(Angle / 2);
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
                        value.NominalValue = new IfcPositiveLengthMeasure(BendRadius);
                    }));
                });
            });

            #endregion
        }
    }
}