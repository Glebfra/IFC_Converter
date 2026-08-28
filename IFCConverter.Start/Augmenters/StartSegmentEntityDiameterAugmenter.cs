using System.Collections.Generic;
using System.Linq;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;

namespace IFCConverter.Start.Augmenters
{
    internal sealed class StartSegmentEntityDiameterAugmenter : StartAbstractAugmenter<IStartSegmentEntity>
    {
        private const double DefaultDiameter = 0.05;

        public override void AugmentTyped(IStartSegmentEntity entity, IEnumerable<IStartEntity> otherEntities)
        {
            entity.Diameter = ResolveDiameter(entity);
        }

        private static IStartValueProperty<double> ResolveDiameter(IStartSegmentEntity entity)
        {
            HashSet<IStartSegmentEntity> visited = new HashSet<IStartSegmentEntity>();
            if (TryGetDiameterRecursive(entity, visited, out double diameter))
                return entity.Diameter.CreateFromStart(diameter);

            return new LengthValueProperty<double>().CreateFromStart(DefaultDiameter);
        }

        private static bool TryGetDiameterRecursive(IStartSegmentEntity segment, HashSet<IStartSegmentEntity> visited, out double diameter)
        {
            diameter = DefaultDiameter;
            if (!visited.Add(segment))
                return false;

            if (segment.Diameter.HasValue)
            {
                diameter = segment.Diameter.StartProperty;
                return true;
            }

            if (segment.ConnectedEntities.Any(x => x is StartCylindricalShellEntity))
                return true;

            foreach (IStartSegmentEntity neighbour in segment.ConnectedEntities.OfType<IStartSegmentEntity>())
            {
                if (TryGetDiameterRecursive(neighbour, visited, out diameter))
                    return true;
            }

            return false;
        }
    }
}