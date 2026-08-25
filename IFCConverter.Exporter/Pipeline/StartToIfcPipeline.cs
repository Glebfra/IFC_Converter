using System;
using System.Collections.Generic;
using IFCConverter.Domain;
using Start.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Exporter.Pipeline
{
    internal sealed class StartToIfcPipeline
    {
        private readonly StartToDomainPipeline _startToDomainPipeline = new StartToDomainPipeline();
        private readonly DomainToIfcPipeline _domainToIfcPipeline = new DomainToIfcPipeline();
        
        public void Execute(IReadOnlyCollection<IStartEntity> source, IModel ifcModel, Action<IIfcProduct> addProduct)
        {
            EngineeringModel domain = _startToDomainPipeline.Execute(source);
            _domainToIfcPipeline.Execute(domain, ifcModel, addProduct);
        }
    }
}