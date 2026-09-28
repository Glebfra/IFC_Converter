using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPostPortResolvers
{
    internal interface IEntityPostPortResolversRegistry
    {
        IEntityPostPortResolver Resolve(IIfcProduct product);
        bool TryResolve(IIfcProduct product, out IEntityPostPortResolver resolver);
    }
}