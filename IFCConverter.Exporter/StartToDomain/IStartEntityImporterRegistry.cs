using IFCConverter.Exporter.StartToDomain.StartEntityImporters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Registries;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IStartEntityImporterRegistry : IRegistry<IStartEntity, IStartEntityImporter>
    {
    }
}