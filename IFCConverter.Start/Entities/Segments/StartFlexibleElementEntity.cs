using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;

namespace IFCConverter.Start.Entities.Segments
{
    /// <summary>
    ///     Represents a flexible element entity in the IFCConverter.Start framework.
    ///     Inherits from <see cref="StartAbstractSegmentEntity" /> and implements the
    /// </summary>
    [StartElement(StartElementTypeEnum.FLEXIBLE_ELEMENT)]
    public sealed class StartFlexibleElementEntity : StartAbstractSegmentEntity
    {
    }
}