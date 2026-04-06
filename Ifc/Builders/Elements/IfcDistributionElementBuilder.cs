using System.Collections.Generic;
using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;

namespace Ifc.Builders.Elements
{
    public class IfcDistributionElementBuilder<T> : IfcElementBuilder<T>, IIfcDistributionElementBuilder<T>
        where T : IfcDistributionElement
    {
        public IfcDistributionElementBuilder(IfcLabel name, IfcIdentifier tag) : base(name, tag)
        {
        }

        public List<IIfcPort> Ports { get; } = new();

        public override T CreateInstance(IModel model)
        {
            T instance = base.CreateInstance(model);

            const string transactionName = $"{nameof(IfcDistributionElementBuilder<T>)}: {nameof(CreateInstance)}";
            foreach (IIfcPort ifcPort in Ports)
                model.Instances.New<IfcRelConnectsPortToElement>(element =>
                {
                    element.RelatingPort = (IfcPort)ifcPort;
                    element.RelatedElement = instance;
                });

            return instance;
        }
    }
}