using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal interface IStartEntityImporter
    {
        bool CanImport(IStartEntity source);
        Entity Import(IStartEntity source, EngineeringModel model, StartMappingContext context);
    }
}