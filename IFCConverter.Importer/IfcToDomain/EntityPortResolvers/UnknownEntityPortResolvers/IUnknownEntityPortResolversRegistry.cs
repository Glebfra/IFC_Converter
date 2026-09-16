using IFCConverter.Domain;
using IFCConverter.Importer.IfcToDomain.EntityPortResolvers.AvevaEntityPortResolvers;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.UnknownEntityPortResolvers
{
    internal interface IUnknownEntityPortResolversRegistry
    {
        IUnknownEntityPortResolver Resolve(IIfcProduct product, EngineeringModel model, ImportContext context);
        bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out IUnknownEntityPortResolver resolver);
    }
}