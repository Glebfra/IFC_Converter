using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.Converters.Importers
{
    internal abstract class AbstractEntityImporter<TSource, TResult> : IEntityImporter<TSource, TResult>
        where TSource : IIfcElement
        where TResult : class
    {
        public abstract TResult ReadTyped(TSource source);
        public object Read(IInstantiableEntity entity) => ReadTyped((TSource)entity);

        private double? _lengthPowerCache = null;

        [Pure]
        protected double GetLengthPower(TSource source)
        {
            if (_lengthPowerCache != null)
                return (double)_lengthPowerCache;

            IfcSIUnit? siUnit = source.Model.Instances
                .OfType<IfcSIUnit>()
                .FirstOrDefault(unit => unit.UnitType == IfcUnitEnum.LENGTHUNIT);
            _lengthPowerCache = siUnit?.Power ?? 1.0;
            
            return (double)_lengthPowerCache;
        }

        [Pure]
        protected IEnumerable<IIfcRepresentationItem> GetRepresentationItems(TSource source)
        {
            IIfcProductRepresentation representation = source.Representation;
            IEnumerable<IIfcRepresentation> representations = representation.Representations;
            return representations.SelectMany(ifcRepresentation => ifcRepresentation.Items);
        }
    }
}