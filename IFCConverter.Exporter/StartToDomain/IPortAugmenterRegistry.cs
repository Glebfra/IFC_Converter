using IFCConverter.Exporter.StartToDomain.PortAugmenters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Registries;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IPortAugmenterRegistry : IRegistry<IStartEntity, IPortAugmenter>
    {
    }
}