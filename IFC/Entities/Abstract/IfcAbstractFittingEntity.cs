using System.Linq;
using IFC.Entities.Fittings;
using IFC.Entities.Interfaces;
using IFC.Tools;
using Start.Entities;
using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractFittingEntity : IfcAbstractEntity, IIfcOneNodeEntity
    {
        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }

        public double Angle { get; protected set; }
        public double Diameter { get; protected set; }
        public IfcNodeEntity NodeEntity { get; }
        
        private StartAbstractEntity _abstractEntity;
        protected IfcAbstractSegmentEntity[] _IfcAbstractSegmentEntities;
        
        protected IfcAbstractFittingEntity(StartAbstractEntity abstractEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities) 
            : base(abstractEntity)
        {
            _abstractEntity = abstractEntity;
            NodeEntity = ifcNodeEntity;
            _IfcAbstractSegmentEntities = ifcAbstractSegmentEntities;
            
            XbimVector3D coordinates = ifcNodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D[] directionToPipes = _IfcAbstractSegmentEntities.Select(entity => IfcAxis.GetDirectionToPipe(entity, coordinates)).ToArray();
            XbimVector3D forward = directionToPipes[0].Negated();
            XbimVector3D up;

            if (_IfcAbstractSegmentEntities.Length == 2)
            {
                Angle = forward.Angle(directionToPipes[1]);
            }
            if (Angle == 0 && directionToPipes.Length == 3)
            {
                Angle = forward.Angle(directionToPipes[2]);
            }
            if (Angle != 0)
            {
                up = XbimVector3D.CrossProduct(forward, directionToPipes[1]).Normalized();
            }
            else
            {
                XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
                if (forward != WorldUp && forward != WorldUp.Negated())
                {
                    up = WorldUp;
                }
                else
                {
                    up = new XbimVector3D(0, 1, 0);
                }
            }

            Diameter = _IfcAbstractSegmentEntities[0].Diameter;

            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
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
                    foreach (var kvp in _abstractEntity.GetData())
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
        }
    }
}