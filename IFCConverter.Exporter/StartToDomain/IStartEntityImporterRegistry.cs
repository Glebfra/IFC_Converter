using IFCConverter.Exporter.StartToDomain.StartEntityImporters;
using IFCConverter.Utils.Registries;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IStartEntityImporterRegistry : IRegistry<IStartEntity, IStartEntityImporter>
    {
    }
}