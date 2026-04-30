using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace IFCConverter.Interfaces
{
    internal interface IPropertySet
    {
        [Pure]
        public Dictionary<string, object> GetDictionary();
        public void SetDictionary(Dictionary<string, object> dictionary);
    }
}