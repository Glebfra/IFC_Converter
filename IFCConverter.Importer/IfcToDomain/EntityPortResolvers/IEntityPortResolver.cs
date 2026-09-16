using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers
{
    internal interface IEntityPortResolver
    {
        bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context);
        void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}