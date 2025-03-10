using System.Collections.Generic;
using System.Linq;
using Start.API;

namespace Start.Extensions
{
    public static class StartDataArrayItemExtensions
    {
        public static IEnumerable<StartDataArrayItem> GetElementsByType(this IEnumerable<StartDataArrayItem> arrayItems, StartElementType type)
        {
            return arrayItems.Where(item => item.Type == type);
        }
        
        public static IEnumerable<StartDataArrayItem> GetElementsByType(this IEnumerable<StartDataArrayItem> arrayItems, IEnumerable<StartElementType> types)
        {
            return arrayItems.Where(item => types.Contains(item.Type));
        }

        public static IEnumerable<StartDataArrayItem> GetConnElements(this StartDataArrayItem[] arrayItems, int ID)
        {
            StartDataArrayItem baseElement = arrayItems.Single(item => item.DataArrayIndex == ID);
            int[] baseElementNodeIds = baseElement.NodeIds;
            return baseElementNodeIds.Length == 1 
                ? arrayItems.Where(item => item.NodeIds.Contains(baseElementNodeIds[0])) 
                : arrayItems.Where(item => item.NodeIds.Contains(baseElementNodeIds[0]) || item.NodeIds.Contains(baseElementNodeIds[1]));
        }
    }
}