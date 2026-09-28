using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPostPortResolvers
{
    internal interface IEntityPostPortResolver
    {
        bool CanResolve(IIfcProduct product);
        void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}