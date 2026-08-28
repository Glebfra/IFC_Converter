using System.Collections.Generic;
using System.Linq;
using IFCConverter.Start.API;

namespace IFCConverter.Start.Extensions
{
    internal static class StartDataArrayItemExtensions
    {
        public static IEnumerable<StartDataArrayItem> GetElementsByType(this IEnumerable<StartDataArrayItem> arrayItems,
            StartElementTypeEnum type)
        {
            return arrayItems.GetElementsByType(new[]
            {
                type
            });
        }

        public static IEnumerable<StartDataArrayItem> GetElementsByType(this IEnumerable<StartDataArrayItem> arrayItems,
            IEnumerable<StartElementTypeEnum> types)
        {
            return arrayItems.Where(item => types.Contains(item.Type));
        }

        public static IEnumerable<StartDataArrayItem> GetConnElements(this StartDataArrayItem[] arrayItems, int ID)
        {
            StartDataArrayItem baseElement = arrayItems.Single(item => item.DataArrayIndex == ID);
            int[] baseElementNodeIds = baseElement.NodeIds;

            List<StartDataArrayItem> dataArrayItems = new List<StartDataArrayItem>();
            foreach (int baseElementNodeId in baseElementNodeIds)
                dataArrayItems.AddRange(arrayItems.Where(item => item.NodeIds.Contains(baseElementNodeId)
                                                                 && item.DataArrayIndex != ID));
            return dataArrayItems;
        }

        public static IEnumerable<StartDataArrayItem> GetConnElements(this StartDataArrayItem[] arrayItems,
            StartDataArrayItem baseElement)
        {
            return arrayItems.GetConnElements(baseElement.DataArrayIndex);
        }
    }
}