using System;
using IFCConverter.Domain;
using IFCConverter.Exporter.DomainToIfc;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Exporter.Pipeline
{
    internal sealed class DomainToIfcPipeline
    {
        private readonly DomainToIfcPhaseRegistry _phaseRegistry = new DomainToIfcPhaseRegistry();
        
        public void Execute(EngineeringModel domain, IModel ifcModel, Action<IIfcProduct> addProduct)
        {
            if (domain == null)
                throw new ArgumentNullException(nameof(domain));
            if (ifcModel == null)
                throw new ArgumentNullException(nameof(ifcModel));
            if (addProduct == null)
                throw new ArgumentNullException(nameof(addProduct));

            ExportContext context = new ExportContext();
            foreach (IDomainToIfcPhase domainToIfcPhase in _phaseRegistry.GetPhases())
            {
                domainToIfcPhase.Execute(domain, ifcModel, context);
            }
            foreach (IIfcProduct product in context.Products)
            {
                addProduct(product);
            }
        }
    }
}