using IFCConverter.Exporter.StartToDomain.PortAugmenters;
using IFCConverter.Utils;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IPortAugmenterRegistry : IRegistry<IStartEntity, IPortAugmenter>
    {
    }
}