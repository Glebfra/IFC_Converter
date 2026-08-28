using System.Collections.Generic;

namespace IFCConverter.Start.Interfaces.Augmenters
{
    public interface IStartEntityAugmenter
    {
        void Augment(IStartEntity entity, IEnumerable<IStartEntity> otherEntities);
    }
}