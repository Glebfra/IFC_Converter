using IFCConverter.Domain;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal interface IStartEntityImporter
    {
        bool CanImport(IStartEntity source);
        void Import(IStartEntity source, EngineeringModel model, StartMappingContext context);
    }
}