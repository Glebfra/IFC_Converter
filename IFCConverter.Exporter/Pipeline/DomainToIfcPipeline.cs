using System;
using IFCConverter.Domain;
using IFCConverter.Exporter.DomainToIfc;
using IFCConverter.Exporter.DomainToIfc.Phases;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Exporter.Pipeline
{
    internal sealed class DomainToIfcPipeline
    {
        private readonly DomainToIfcPhaseRegistry _phaseRegistry = new DomainToIfcPhaseRegistry();

        public void Execute(EngineeringModel domain, IModel ifcModel, ILoggerFactory factory, Action<IIfcProduct> addProduct)
        {
            if (domain == null)
                throw new ArgumentNullException(nameof(domain));
            if (ifcModel == null)
                throw new ArgumentNullException(nameof(ifcModel));
            if (addProduct == null)
                throw new ArgumentNullException(nameof(addProduct));

            ILogger logger = factory.CreateLogger<DomainToIfcPipeline>();

            ExportContext context = new ExportContext();
            foreach (IDomainToIfcPhase domainToIfcPhase in _phaseRegistry.GetPhases())
            {
                logger.LogInformation("Executing phase {phase}", domainToIfcPhase.GetType().Name);
                domainToIfcPhase.Execute(domain, ifcModel, context, factory);
            }

            foreach (IIfcProduct product in context.Products)
            {
                addProduct(product);
            }
        }
    }
}