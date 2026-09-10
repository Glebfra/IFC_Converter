using IFCConverter.Domain;

namespace IFCConverter.Importer.DomainToStart.Phases
{
    public interface IDomainToStartPhase
    {
        void Execute(EngineeringModel model, ExportContext context);
    }
}