using System.Collections.Generic;
using IFC.Entities.Interfaces;
using IFC.PropertySets;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractEntity : IIfcEntity
    {
        /// <summary>
        /// Gets the name of the IFC entity.
        /// </summary>
        public abstract ActionProperty<IfcLabel> Name { get; }
        
        /// <summary>
        /// Gets the tag of the IFC entity.
        /// </summary>
        public abstract ActionProperty<IfcIdentifier> Tag { get; } 
        
        /// <summary>
        /// Gets the color of the IFC entity.
        /// </summary>
        public virtual ActionProperty<Colour> Colour { get; } = new ActionProperty<Colour>(Tools.Colour.FromHEX("bebebe"));

        /// <summary>
        /// Gets the 3D object matrix of the IFC entity.
        /// </summary>
        public ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        
        /// <summary>
        /// Gets the list of property sets associated with the IFC entity.
        /// </summary>
        public List<IPropertySet> PropertySets { get; } = new List<IPropertySet>();
        
        /// <summary>
        /// Gets the list of connected entities for the IFC entity.
        /// </summary>
        public List<IfcAbstractEntity> ConnectedEntities { get; } = new List<IfcAbstractEntity>();

        /// <summary>
        /// Initializes a new instance of the IfcAbstractEntity class with the specified object matrix.
        /// </summary>
        /// <param name="objectMatrix3D">The 3D object matrix of the IFC entity.</param>
        protected IfcAbstractEntity(XbimMatrix3D objectMatrix3D)
        {
            ObjectMatrix3D = new ActionProperty<XbimMatrix3D>(objectMatrix3D);
        }

        /// <summary>
        /// Creates and adds an IFC product to the model.
        /// </summary>
        /// <param name="model">The model to which the product will be added.</param>
        /// <returns>The created IFC product.</returns>
        public abstract IfcProduct CreateAndAdd(IModel model);

        /// <summary>
        /// Performs operations before creating the IFC entity.
        /// </summary>
        protected virtual void PreCreate() { }
        
        /// <summary>
        /// Performs operations after creating the IFC entity.
        /// </summary>
        protected virtual void PostCreate() { }

        /// <summary>
        /// Creates an IFC entity of the specified type and adds it to the model.
        /// </summary>
        /// <typeparam name="T">The type of the IFC entity to create.</typeparam>
        /// <param name="model">The model to which the entity will be added.</param>
        /// <returns>The created IFC entity.</returns>
        protected T CreateIfcEntity<T>(IModel model)
            where T : IfcElement, IInstantiableEntity
        {
            PreCreate();
            
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
            
            PostCreate();
            return ifcElement;
        }

        /// <summary>
        /// Adds a shape representation to the specified IFC product using a single representation item.
        /// </summary>
        /// <param name="model">The model containing the product.</param>
        /// <param name="product">The IFC product to which the shape representation will be added.</param>
        /// <param name="representationItem">The representation item to use for the shape representation.</param>
        protected void AddShapeRepresentation(IModel model, IfcProduct product, IfcRepresentationItem representationItem)
        {
            AddShapeRepresentation(model, product, new[] { representationItem });
        }
        
        /// <summary>
        /// Adds a shape representation to the specified IFC product using multiple representation items.
        /// </summary>
        /// <param name="model">The model containing the product.</param>
        /// <param name="product">The IFC product to which the shape representation will be added.</param>
        /// <param name="representationItems">The representation items to use for the shape representation.</param>
        protected void AddShapeRepresentation(IModel model, IfcProduct product, IEnumerable<IfcRepresentationItem> representationItems)
        {
            ColourEntity(model, representationItems);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, representationItems);
            product.Representation = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        }

        /// <summary>
        /// Adds properties to the specified IFC product.
        /// </summary>
        /// <param name="model">The model containing the product.</param>
        /// <param name="product">The IFC product to which the properties will be added.</param>
        private void AddProperties(IModel model, IfcProduct product)
        {
            #if DEBUG
            PropertySets.Add(new Pset_Debug(ObjectMatrix3D));
            #endif
            
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
        
        /// <summary>
        /// Applies a color style to the specified representation items.
        /// </summary>
        /// <param name="model">The model containing the representation items.</param>
        /// <param name="representationItems">The representation items to style.</param>
        protected void ColourEntity(IModel model, IEnumerable<IfcRepresentationItem> representationItems)
        {
            IfcColours.StyleItems(model, Colour, representationItems);
        }

        /// <summary>
        /// Applies a color style to a single representation item.
        /// </summary>
        /// <param name="model">The model containing the representation item.</param>
        /// <param name="representationItems">The representation item to style.</param>
        protected void ColourEntity(IModel model, IfcRepresentationItem representationItems)
        {
            ColourEntity(model, new[] { representationItems });
        }
    }
}
