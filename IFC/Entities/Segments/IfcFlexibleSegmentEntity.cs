using System;
using IFC.Entities.Abstract;
using IFC.Entities.Fittings;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.Entities.Segments
{
    public sealed class IfcFlexibleSegmentEntity : IfcAbstractSegmentEntity
    {
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public override XbimVector3D Direction { get; }
        public override double Diameter { get; }

        private StartFlexibleElementEntity _startFlexibleElementEntity;
        private IfcPipeSegment _pipeSegment;
        
        public IfcFlexibleSegmentEntity(StartFlexibleElementEntity startFlexibleElementEntity, IfcNodeEntity[] ifcNodeEntities, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(startFlexibleElementEntity, ifcNodeEntities)
        {
            _startFlexibleElementEntity = startFlexibleElementEntity;
            Coordinates = ifcNodeEntities[0].ObjectMatrix3D.Translation;
            Direction = ifcNodeEntities[1].ObjectMatrix3D.Translation - Coordinates;
            Length = Direction.Length;
            
            XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
            XbimVector3D forward = Direction.Normalized();
            if (forward == WorldUp || forward == -1 * WorldUp) 
                WorldUp = new XbimVector3D(0, 1, 0);
            XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);
            
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(Coordinates, forward, up);
            Diameter = abstractSegmentEntities.Length switch
            {
                1 => abstractSegmentEntities[0].Diameter,
                2 => Math.Min(abstractSegmentEntities[0].Diameter, abstractSegmentEntities[1].Diameter),
                _ => 0.05
            };
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeSegment = CreatePipeSegment(model, _startFlexibleElementEntity.Name, IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT);
            AddProperties(model, _pipeSegment);
            return _pipeSegment;
        }

        protected override void AddProperties(IModel model, IfcProduct product)
        {
            base.AddProperties(model, product);
            
            #region Pset_FlexibleSegmentTypeStart

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "Pset_FlexibleSegmentTypeStart";
                    foreach (var kvp in _startFlexibleElementEntity.GetData())
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