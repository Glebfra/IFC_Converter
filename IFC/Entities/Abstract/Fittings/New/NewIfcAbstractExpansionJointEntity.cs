using System;
using IFC.Entities.Interfaces;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class NewIfcAbstractExpansionJointEntity : NewIfcAbstractFittingEntity
    {
        protected void ClipPipes()
        {
            foreach (NewIfcAbstractEntity abstractEntity in ConnectedEntities)
            {
                if (abstractEntity is IIfcClippable ifcClippable)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}