using IFCConverter.Domain;
using Xbim.Common;

namespace IFCConverter.Importer.IfcToDomain.Phases
{
    public interface IIfcToDomainPhase
    {
        void Execute(IModel model, EngineeringModel domain, ImportContext context);
    }
}