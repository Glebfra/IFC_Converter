using IFCConverter.Exporter.StartToDomain.PortAugmenters;
using IFCConverter.Utils.Registries;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IPortAugmenterRegistry : IRegistry<IStartEntity, IPortAugmenter>
    {
    }
}