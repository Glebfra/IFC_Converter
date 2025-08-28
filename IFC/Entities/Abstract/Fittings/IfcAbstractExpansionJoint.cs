using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Interfaces;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Fittings
{
    /// <summary>
    /// Abstract base class representing an expansion joint entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractExpansionJointEntity : IfcAbstractFittingEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractExpansionJointEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
        /// <summary>
        /// Clips the connected pipes to fit the expansion joint.
        /// </summary>
        protected void ClipPipes()
        {
            IEnumerable<IIfcClippable> clippables = ConnectedEntities.OfType<IIfcClippable>();
            foreach (IIfcClippable ifcClippable in clippables)
            {
                ifcClippable.Clip(NodeEntity, Length / 2);
            }
        }
    }
}