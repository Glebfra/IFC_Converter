using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using IFCConverter.Attributes;
using IFCConverter.Extensions;
using Utils;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.PropertySets
{
    internal class PropertySetRegistry
    {
        public static PropertySetRegistry GetInstance() => _instance.Value;
        private static readonly Lazy<PropertySetRegistry> _instance =
            new Lazy<PropertySetRegistry>(() => new PropertySetRegistry());

        private readonly List<Type> _propertySetsList = new List<Type>();

        private PropertySetRegistry()
        {
            RegisterAll();
        }

        [Pure]
        public T Read<T>(IIfcPropertySet ifcPropertySet)
            where T : AbstractPropertySet
        {
            string propertySetName = ifcPropertySet.Name ?? string.Empty;
            Type? propertySetType = _propertySetsList
                .FirstOrDefault(type => type.GetCustomAttribute<PropertySetAttribute>().IsMatch(propertySetName));
            if (propertySetType == null)
                throw new Exception($"Property set with name {propertySetName} is not registered");

            T propertySet = (T)Activator.CreateInstance(propertySetType);
            propertySet.SetDictionary(ifcPropertySet.ToDictionary());
            return propertySet;
        }

        private void RegisterAll()
        {
            IEnumerable<Type> propertySetsTypes = AttributeFinder.GetClassesWithAttribute<PropertySetAttribute>();
            foreach (Type propertySetsType in propertySetsTypes)
            {
                PropertySetAttribute? propertySetAttribute =
                    propertySetsType.GetCustomAttribute<PropertySetAttribute>();
                if (propertySetAttribute == null)
                    continue;

                _propertySetsList.Add(propertySetsType);
            }
        }
    }
}