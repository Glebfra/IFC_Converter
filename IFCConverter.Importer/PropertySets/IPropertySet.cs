using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace IFCConverter.Importer.PropertySets
{
    internal interface IPropertySet
    {
        [Pure]
        Dictionary<string, object> GetDictionary();

        void SetDictionary(Dictionary<string, object> dictionary);
    }
}