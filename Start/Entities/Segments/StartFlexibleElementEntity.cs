using Start.API;
using Start.Attributes;
using Start.Augmenters;
using Start.Interfaces;

namespace Start.Entities.Segments
{
    /// <summary>
    ///     Represents a flexible element entity in the Start framework.
    ///     Inherits from <see cref="StartAbstractSegmentEntity" /> and implements the
    /// </summary>
    [StartElement(StartElementTypeEnum.FLEXIBLE_ELEMENT, typeof(StartFlexibleElementDiameterAugmenter))]
    public sealed class StartFlexibleElementEntity : StartAbstractSegmentEntity
    {
    }
}