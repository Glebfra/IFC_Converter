using System.Collections.Generic;
using System.Linq;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Diagnostics;

namespace IFCConverter.Start.Augmenters
{
    internal sealed class StartClippableEntityAugmenter : StartAbstractAugmenter<IStartClippableEntity>
    {
        private readonly Logger _logger = Logger.GetInstance();

        public override void AugmentTyped(IStartClippableEntity entity, IEnumerable<IStartEntity> otherEntities)
        {
            IEnumerable<IStartClippingEntity> clippingEntities = entity.ConnectedEntities.OfType<IStartClippingEntity>();
            foreach (IStartClippingEntity clippingEntity in clippingEntities)
            {
                clippingEntity.ClipEntity(entity);
                _logger.Info($"Clipping entity {entity.GetType().FullName} by {clippingEntity.GetType().FullName}");
            }
        }
    }
}