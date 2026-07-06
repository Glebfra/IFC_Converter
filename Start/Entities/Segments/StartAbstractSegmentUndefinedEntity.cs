using System.Linq;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities.Segments
{
    /// <summary>
    ///     Represents an abstract segment entity with an undefined diameter in the Start framework.
    ///     Implements the <see cref="IStartSegmentDiameterUndefinedEntity" /> interface.
    /// </summary>
    public abstract class StartAbstractSegmentUndefinedEntity : StartAbstractSegmentEntity,
        IStartSegmentDiameterUndefinedEntity
    {
        /// <summary>
        ///     Backing field for the <see cref="Diameter" /> property.
        /// </summary>
        private IStartValueProperty<double>? _diameter;

        /// <summary>
        ///     Gets or sets the diameter of the segment.
        ///     If the diameter is not explicitly set, it is calculated using the <see cref="GetDiameter" /> method.
        /// </summary>
        public override IStartValueProperty<double> Diameter
        {
            get => _diameter ??= GetDiameter(this);
            set => _diameter = value;
        }

        /// <summary>
        ///     Calculates the diameter of the segment by considering connected entities.
        ///     Skips the specified entity during the calculation.
        /// </summary>
        /// <param name="skipEntity">The entity to skip during the calculation. Defaults to <c>null</c>.</param>
        /// <returns>The calculated diameter as an <see cref="IStartValueProperty{T}" />.</returns>
        public IStartValueProperty<double> GetDiameter(object? skipEntity = null)
        {
            IStartSegmentEntity[] skippedEntities = ConnectedEntities
                .OfType<IStartSegmentEntity>()
                .Where(entity => !entity.Equals(skipEntity))
                .ToArray();

            IStartSegmentEntity[] segmentEntities = skippedEntities
                .Where(segment => segment is not IStartSegmentDiameterUndefinedEntity)
                .ToArray();
            if (segmentEntities.Any())
                return segmentEntities.Min(entity => entity.Diameter);

            IStartSegmentDiameterUndefinedEntity[] diameterUndefinedEntities = skippedEntities
                .OfType<IStartSegmentDiameterUndefinedEntity>()
                .ToArray();
            if (diameterUndefinedEntities.Any())
                return diameterUndefinedEntities.Min(entity => entity.Diameter);

            return new LengthValueProperty<double>().CreateFromStart(0.05);
        }
    }
}