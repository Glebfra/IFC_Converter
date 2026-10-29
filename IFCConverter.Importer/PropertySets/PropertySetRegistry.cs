using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Extensions;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.PropertySets
{
    internal sealed class PropertySetRegistry
    {
        private static readonly Lazy<PropertySetRegistry> _instance = new Lazy<PropertySetRegistry>(() => new PropertySetRegistry());

        private readonly List<Type> _propertySetsList = new List<Type>();

        private PropertySetRegistry()
        {
            RegisterAll();
        }

        public static PropertySetRegistry GetInstance()
        {
            return _instance.Value;
        }

        [Pure]
        public T Read<T>(IIfcPropertySet ifcPropertySet)
            where T : IPropertySet
        {
            if (!TryRead(ifcPropertySet, out T pSet))
                throw new Exception($"Property set with name {ifcPropertySet.Name} is not registered.");

            return pSet;
        }

        [Pure]
        public bool TryRead<T>(IIfcPropertySet ifcPropertySet, out T propertySet)
            where T : IPropertySet
        {
            propertySet = default;

            string propertySetName = ifcPropertySet.Name ?? string.Empty;
            Type propertySetType = _propertySetsList
                .FirstOrDefault(type => type.GetCustomAttribute<PropertySetAttribute>().IsMatch(propertySetName));
            if (propertySetType == null)
                return false;

            propertySet = ParameterlessConstructorRegistry<T>.Create(propertySetType);
            propertySet.SetDictionary(ifcPropertySet.ToDictionary());
            return true;
        }

        private void RegisterAll()
        {
            IEnumerable<Type> propertySetsTypes = AttributeFinder.GetClassesWithAttribute<PropertySetAttribute>();
            foreach (Type propertySetsType in propertySetsTypes)
            {
                PropertySetAttribute propertySetAttribute =
                    propertySetsType.GetCustomAttribute<PropertySetAttribute>();
                if (propertySetAttribute == null)
                    continue;

                _propertySetsList.Add(propertySetsType);
            }
        }
    }
}