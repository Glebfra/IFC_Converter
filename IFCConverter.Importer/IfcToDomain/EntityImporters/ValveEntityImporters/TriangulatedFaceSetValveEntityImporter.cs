using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ValveEntityImporters
{
    internal sealed class TriangulatedFaceSetValveEntityImporter : IValveEntityImporter
    {
        public bool CanImport(IIfcProduct product)
        {
            return false;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
        }
    }
}