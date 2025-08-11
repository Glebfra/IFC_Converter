using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Interfaces;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractExpansionJointEntity : IfcAbstractFittingEntity
    {
        protected IfcAbstractExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
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