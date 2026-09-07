using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Augmenters;

namespace IFCConverter.Start.Entities.Fittings
{
    [StartElement(StartElementTypeEnum.REDUCER_ECCENTRIC, typeof(StartReducerSegmentMoverAugmenter))]
    public sealed class StartReducerEccentricEntity : StartAbstractReducerEntity
    {
    }
}