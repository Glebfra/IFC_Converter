using IFCConverter.Domain;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc
{
    public interface IDomainToIfcPhase
    {
        public void Execute(EngineeringModel domain, IModel model, ExportContext context);
    }
}