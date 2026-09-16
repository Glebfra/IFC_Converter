using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.AvevaEntityPortResolvers
{
    internal interface IAvevaEntityPortResolversRegistry
    {
        IAvevaEntityPortResolver Resolve(IIfcProduct product, EngineeringModel model, ImportContext context);
        bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out IAvevaEntityPortResolver resolver);
    }
}