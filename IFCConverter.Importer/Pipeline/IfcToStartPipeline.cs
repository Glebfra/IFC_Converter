using IFCConverter.Domain;
using IFCConverter.Start.Interfaces;
using Xbim.Common;

namespace IFCConverter.Importer.Pipeline
{
    internal sealed class IfcToStartPipeline
    {
        private readonly IfcToDomainPipeline _ifcToDomainPipeline = new IfcToDomainPipeline();
        private readonly DomainToStartPipeline _domainToStartPipeline = new DomainToStartPipeline();

        public void Execute(IModel model, IStartProject startProject)
        {
            EngineeringModel domain = _ifcToDomainPipeline.Execute(model);
            _domainToStartPipeline.Execute(domain, startProject);
        }
    }
}