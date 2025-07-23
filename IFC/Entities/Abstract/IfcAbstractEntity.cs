using System.Collections.Generic;
using IFC.Entities.Interfaces;
using IFC.PropertySets;
using IFC.Tools;
using Start.API;
using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract
{
    #if NEW 
    
    public abstract class IfcAbstractEntity : IIfcNewEntity
    {
        public abstract ActionProperty<IfcLabel> Name { get; }
        public abstract ActionProperty<IfcIdentifier> Tag { get; } 
        public abstract ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        public virtual ActionProperty<Colour> Colour { get; } = new ActionProperty<Colour>(IFC.Tools.Colour.FromHEX("bebebe"));
        
        public List<IPropertySet> PropertySets { get; } = new List<IPropertySet>();
        public List<IfcAbstractEntity> ConnectedEntities { get; } = new List<IfcAbstractEntity>();

        public abstract IfcProduct CreateAndAdd(IModel model);

        protected T CreateIfcEntity<T>(IModel model)
            where T : IfcElement, IInstantiableEntity
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            T ifcElement = model.Instances.New<T>(product =>
            {
                product.Name = Name;
                product.Tag = Tag;
                product.ObjectPlacement = objectPlacement.LocalPlacement;

                Name.OnValueChange += () => product.Name = Name;
                Tag.OnValueChange += () => product.Tag = Tag;
            });
            
            AddProperties(model, ifcElement);
            return ifcElement;
        }

        protected void AddShapeRepresentation(IModel model, IfcProduct product, IfcRepresentationItem representationItem)
        {
            AddShapeRepresentation(model, product, new[] { representationItem });
        }
        
        protected void AddShapeRepresentation(IModel model, IfcProduct product, IEnumerable<IfcRepresentationItem> representationItems)
        {
            ColourEntity(model, representationItems);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, representationItems);
            product.Representation = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        }

        private void AddProperties(IModel model, IfcProduct product)
        {
            foreach (IPropertySet propertySet in PropertySets)
            {
                model.Instances.New<IfcRelDefinesByProperties>(properties =>
                {
                    properties.Name = propertySet.GetType().Name;
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
    
    #else
    
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

    #endif
}

