using IFCConverter.Exporter.Attributes;
using IFCConverter.Utils.Registries;

namespace IFCConverter.Exporter.DomainToIfc.Phases
{
    internal sealed class DomainToIfcPhaseRegistry : AbstractPhaseRegistry<IDomainToIfcPhase, DomainToIfcPhaseAttribute>
    {
    }
}