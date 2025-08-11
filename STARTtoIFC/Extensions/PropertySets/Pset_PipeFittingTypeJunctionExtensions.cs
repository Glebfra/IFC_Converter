using System;
using IFC.PropertySets;
using Start.Entities.Fittings;
using Xbim.Ifc4.MeasureResource;

namespace STARTtoIFC.Extensions.PropertySets
{
    #if NEW
    
    internal static class Pset_PipeFittingTypeJunctionExtensions
    {
        public static Pset_PipeFittingTypeJunction CreateFromStart(StartTeeEntity teeEntity)
        {
            Pset_PipeFittingTypeJunction pset = new Pset_PipeFittingTypeJunction()
            {
                JunctionType = new IfcLabel("TEE"),
            };

            return pset;
        }
    }
    
    #endif
}