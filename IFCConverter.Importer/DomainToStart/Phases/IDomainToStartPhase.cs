using IFCConverter.Domain;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Importer.DomainToStart.Phases
{
    public interface IDomainToStartPhase
    {
        void Execute(EngineeringModel model, ExportContext context, ILoggerFactory factory);
    }
}