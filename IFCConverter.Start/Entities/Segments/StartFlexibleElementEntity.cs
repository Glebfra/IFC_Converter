using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Augmenters;

namespace IFCConverter.Start.Entities.Segments
{
    /// <summary>
    ///     Represents a flexible element entity in the IFCConverter.Start framework.
    ///     Inherits from <see cref="StartAbstractSegmentEntity" /> and implements the
    /// </summary>
    [StartElement(StartElementTypeEnum.FLEXIBLE_ELEMENT, typeof(StartSegmentEntityDiameterAugmenter), typeof(StartClippableEntityAugmenter))]
    public sealed class StartFlexibleElementEntity : StartAbstractSegmentEntity
    {
    }
}