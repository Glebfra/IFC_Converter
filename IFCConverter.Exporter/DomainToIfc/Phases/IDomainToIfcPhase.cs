using IFCConverter.Domain;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.Phases
{
    public interface IDomainToIfcPhase
    {
        void Execute(EngineeringModel domain, IModel model, ExportContext context);
    }
}