using System;
using System.Collections.Generic;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.Interfaces.Augmenters;

namespace IFCConverter.Start.Augmenters
{
    internal abstract class StartAbstractAugmenter<T> : IStartEntityAugmenter
        where T : IStartEntity
    {
        public void Augment(IStartEntity entity, IEnumerable<IStartEntity> otherEntities)
        {
            if (!(entity is T t))
                throw new Exception($"Entity is not type {typeof(T)}");
            AugmentTyped(t, otherEntities);
        }

        public abstract void AugmentTyped(T entity, IEnumerable<IStartEntity> otherEntities);
    }
}