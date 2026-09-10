using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.AvevaEntityPortResolvers
{
    internal interface IAvevaEntityPortResolver
    {
        bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context);
        void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}