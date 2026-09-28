using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.SegmentEntityPortResolvers
{
    internal interface ISegmentEntityPortResolversRegistry
    {
        ISegmentEntityPortResolver Resolve(IIfcProduct product, EngineeringModel model, ImportContext context);
        bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out ISegmentEntityPortResolver resolver);
    }
}