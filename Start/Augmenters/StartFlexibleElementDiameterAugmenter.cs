using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using Start.Entities.Segments;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Augmenters
{
    internal sealed class StartFlexibleElementDiameterAugmenter : StartAbstractAugmenter<StartFlexibleElementEntity>
    {
        private const double DefaultDiameter = 0.05;
        
        public override void AugmentTyped(StartFlexibleElementEntity entity, IEnumerable<IStartEntity> otherEntities)
        {
            entity.Diameter = ResolveDiameter(entity);
        }
        
        private static IStartValueProperty<double> ResolveDiameter(StartFlexibleElementEntity entity)
        {
            if (entity.Diameter.HasValue)
                return entity.Diameter;
            
            if (entity.ConnectedEntities.Any(connected => connected is StartCylindricalShellEntity))
                return new LengthValueProperty<double>().CreateFromStart(DefaultDiameter);

            IEnumerable<double> diameters = entity.ConnectedEntities
                .OfType<IStartSegmentEntity>()
                .Where(segment => segment.Diameter.HasValue)
                .Select(segment => segment.Diameter.StartProperty);

            return new LengthValueProperty<double>().CreateFromStart(diameters.Min());
        }
    }
}