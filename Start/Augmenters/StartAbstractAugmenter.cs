using System;
using System.Collections.Generic;
using Start.Interfaces;
using Start.Interfaces.Augmenters;

namespace Start.Augmenters
{
    abstract class StartAbstractAugmenter<T> : IStartEntityAugmenter
        where T : IStartEntity
    {
        public void Augment(IStartEntity entity, IEnumerable<IStartEntity> otherEntities)
        {
            if (entity is not T t)
                throw new Exception($"Entity is not type {typeof(T)}");
            AugmentTyped(t, otherEntities);
        }

        public abstract void AugmentTyped(T entity, IEnumerable<IStartEntity> otherEntities);
    }
}