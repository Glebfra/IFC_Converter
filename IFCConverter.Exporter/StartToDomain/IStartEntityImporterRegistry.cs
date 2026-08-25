using IFCConverter.Exporter.StartToDomain.StartEntityImporters;
using IFCConverter.Utils;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IStartEntityImporterRegistry : IRegistry<IStartEntity, IStartEntityImporter>
    {
    }
}