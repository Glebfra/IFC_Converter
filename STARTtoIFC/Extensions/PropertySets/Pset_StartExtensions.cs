using IFC.PropertySets;
using Start.Entities.Abstract;

namespace STARTtoIFC.Extensions.PropertySets
{
    #if NEW
    
    internal static class Pset_StartExtensions
    {
        public static Pset_Start CreateFromStart(StartAbstractEntity startAbstractEntity)
        {
            return new Pset_Start()
            {
                Data = startAbstractEntity.GetData()
            };
        }
    }
    
    #endif
}