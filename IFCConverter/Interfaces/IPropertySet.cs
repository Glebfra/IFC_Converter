using System.Collections.Generic;

namespace IFCConverter.Interfaces
{
    internal interface IPropertySet
    {
        public Dictionary<string, object> GetDictionary();
        public void SetDictionary(Dictionary<string, object> dictionary);
    }
}