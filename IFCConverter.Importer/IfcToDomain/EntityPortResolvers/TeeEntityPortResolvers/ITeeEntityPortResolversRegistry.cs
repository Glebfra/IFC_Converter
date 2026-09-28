using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.TeeEntityPortResolvers
{
    internal interface ITeeEntityPortResolversRegistry
    {
        ITeeEntityPortResolver Resolve(IIfcProduct product, EngineeringModel model, ImportContext context);
        bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out ITeeEntityPortResolver resolver);
    }
}