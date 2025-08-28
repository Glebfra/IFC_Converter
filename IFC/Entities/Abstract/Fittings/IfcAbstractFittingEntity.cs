using IFC.Entities.Interfaces;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;

namespace IFC.Entities.Abstract.Fittings
{
    /// <summary>
    /// Abstract base class representing a fitting entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractFittingEntity : IfcAbstractEntity, IIfcOneNodeEntity
    {
        /// <summary>
        /// Gets the length of the fitting.
        /// </summary>
        public abstract ActionProperty<double> Length { get; }
        
        /// <summary>
        /// Gets the color of the fitting.
        /// </summary>
        public override ActionProperty<Colour> Colour { get; } = Tools.Colour.FromHEX("5f4e7c");
        
        /// <summary>
        /// Gets the node entity associated with the fitting.
        /// </summary>
        public IfcNodeEntity NodeEntity { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractFittingEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractFittingEntity(XbimMatrix3D objectMatrix3D)
            : base(objectMatrix3D)
        {
            NodeEntity = new IfcNodeEntity(objectMatrix3D);
        }

        /// <summary>
        /// Creates an IFC entity of type <typeparamref name="T"/> and adds it to the specified model.
        /// </summary>
        /// <typeparam name="T">The type of the IFC entity to create. Must be a subclass of <see cref="IfcPipeFitting"/>.</typeparam>
        /// <param name="model">The IFC model to which the entity will be added.</param>
        /// <returns>The created IFC entity of type <typeparamref name="T"/>.</returns>
        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            return pipeFitting;
        }
    }
}