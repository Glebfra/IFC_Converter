using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.TeeEntityPortResolvers
{
    internal interface ITeeEntityPortResolver
    {
        bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context);
        void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}