using Start.API;
using Start.Attributes;
using Start.Augmenters;

namespace Start.Entities.Fittings
{
    [StartElement(StartElementTypeEnum.REDUCER_ECCENTRIC, typeof(StartReducerSegmentMoverAugmenter))]
    public sealed class StartReducerEccentricEntity : StartAbstractReducerEntity
    {
    }
}