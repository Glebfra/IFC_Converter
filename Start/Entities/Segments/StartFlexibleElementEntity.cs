using Start.API;
using Start.Attributes;
using Start.Interfaces;

namespace Start.Entities.Segments
{
    /// <summary>
    ///     Represents a flexible element entity in the Start framework.
    ///     Inherits from <see cref="StartAbstractSegmentUndefinedEntity" /> and implements the
    ///     <see cref="IStartSegmentDiameterUndefinedEntity" /> interface.
    /// </summary>
    [StartElement(StartElementTypeEnum.FLEXIBLE_ELEMENT)]
    public sealed class StartFlexibleElementEntity : StartAbstractSegmentUndefinedEntity,
        IStartSegmentDiameterUndefinedEntity
    {
    }
}