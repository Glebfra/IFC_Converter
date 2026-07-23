using System.Collections.Generic;

namespace Start.Interfaces.Augmenters
{
    public interface IStartEntityAugmenter
    {
        public void Augment(IStartEntity entity, IEnumerable<IStartEntity> otherEntities);
    }
}