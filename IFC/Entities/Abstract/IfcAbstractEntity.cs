using System.Collections.Generic;
using IFC.Entities.Interfaces;
using IFC.EntitiesExtensions;
using IFC.Tools;
using Start.API;
using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractEntity : IIfcEntity
    {
        public IfcIdentifier Tag { get; }
        public StartElementType Type { get; }
        public StartAbstractEntity StartAbstractEntity { get; }
        
        public List<IPropertySet> PropertySets { get; } = new List<IPropertySet>();

        public abstract XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public abstract Colour Colour { get; protected set; }

        protected IfcAbstractEntity(StartAbstractEntity startAbstractEntity)
        {
            Tag = startAbstractEntity.Type.ToString();
            Type = startAbstractEntity.Type;
            
            StartAbstractEntity = startAbstractEntity;
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

            #region Pset_Start

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "Pset_Start";
                    foreach (var kvp in StartAbstractEntity.GetData())
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
            
            foreach (IPropertySet propertySet in PropertySets)
            {
                model.Instances.New<IfcRelDefinesByProperties>(properties =>
                {
                    properties.RelatedObjects.Add(product);
                    properties.RelatingPropertyDefinition = propertySet.CreatePropertySet(model);
                });
            }
        }
        
        protected void ColourEntity(IModel model, IEnumerable<IfcRepresentationItem> representationItems)
        {
            IfcColours.StyleItems(model, Colour, representationItems);
        }

        protected void ColourEntity(IModel model, IfcRepresentationItem representationItems)
        {
            ColourEntity(model, new[] { representationItems });
        }
    }
}

