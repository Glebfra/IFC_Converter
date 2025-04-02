using IFC.Entities.Interfaces;
using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractAnchorEntity : IfcAbstractEntity, IIfcOneNodeEntity
    {
        public IfcNodeEntity NodeEntity { get; }
        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        
        protected IfcAbstractSegmentEntity[] _IfcAbstractSegmentEntities;

        private StartAbstractEntity _abstractEntity;
        
        public IfcAbstractAnchorEntity(StartAbstractEntity abstractEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(abstractEntity)
        {
            _abstractEntity = abstractEntity;
            NodeEntity = nodeEntity;
            _IfcAbstractSegmentEntities = abstractSegmentEntities;
            
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = abstractSegmentEntities[0].ObjectMatrix3D.Forward;
            XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
            if (forward == WorldUp || forward == -1 * WorldUp)
                WorldUp = new XbimVector3D(0, 1, 0);
            XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);
            
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }

        protected override void AddProperties(IModel model, IfcProduct product)
        {
            base.AddProperties(model, product);
            
            #region Pset_PipeAnchorTypeStart

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "Pset_PipeAnchorTypeStart";
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