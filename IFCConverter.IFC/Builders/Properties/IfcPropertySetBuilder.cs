using System.Collections.Generic;
using System.Linq;
using IFCConverter.IFC.Interfaces.Properties;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.PropertyResource;

namespace IFCConverter.IFC.Builders.Properties
{
    public class IfcPropertySetBuilder : IIfcPropertySetBuilder
    {
        private readonly string _name;
        private readonly IEnumerable<IIfcPropertyBuilder<IIfcProperty>> _propertyBuilders;

        public IfcPropertySetBuilder(string name, IEnumerable<IIfcPropertyBuilder<IIfcProperty>> propertyBuilders)
        {
            _name = name;
            _propertyBuilders = propertyBuilders;
        }

        public IIfcPropertySet CreatePropertySet(IModel model)
        {
            IEnumerable<IIfcProperty> properties =
                _propertyBuilders.Select(propertyBuilder => propertyBuilder.CreateInstance(model));

            return model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = _name;
                set.HasProperties.AddRange(properties.Cast<IfcProperty>());
            });
        }
    }
}