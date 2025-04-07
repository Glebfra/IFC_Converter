using IFC.Entities.Interfaces;
using Start.API;
using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractEntity : IIfcEntity
    {
        public IfcIdentifier Tag { get; }
        public StartElementType Type { get; }
        public abstract XbimMatrix3D ObjectMatrix3D { get; protected set; }

        public IfcAbstractEntity(StartAbstractEntity abstractEntity)
        {
            Tag = abstractEntity.Type.ToString();
            Type = abstractEntity.Type;
        }
        
        public abstract IfcProduct CreateAndAdd(IModel model);

        protected virtual void AddProperties(IModel model, IfcProduct product)
        {
            #region DEBUG
            #if DEBUG
            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "DEBUG";
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "Coordinates";
                        value.NominalValue = new IfcText(ObjectMatrix3D.Translation.ToString());
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "Forward direction";
                        value.NominalValue = new IfcText(ObjectMatrix3D.Forward.ToString());
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "Right direction";
                        value.NominalValue = new IfcText(ObjectMatrix3D.Right.ToString());
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "Up direction";
                        value.NominalValue = new IfcText(ObjectMatrix3D.Up.ToString());
                    }));
                });
            });
            #endif
            #endregion
        }
    }
}

