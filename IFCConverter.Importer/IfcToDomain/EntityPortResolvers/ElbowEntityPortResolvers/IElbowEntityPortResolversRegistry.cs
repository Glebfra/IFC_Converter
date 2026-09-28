using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.ElbowEntityPortResolvers
{
    internal interface IElbowEntityPortResolversRegistry
    {
        IElbowEntityPortResolver Resolve(IIfcProduct product, EngineeringModel model, ImportContext context);
        bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out IElbowEntityPortResolver resolver);
    }
}