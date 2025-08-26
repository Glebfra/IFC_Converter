using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
using IFCtoSTART.Extensions.PropertySets;
using Start.API;
using Start.Entities.Fittings;
using Start.StartProperties;

namespace IFCtoSTART.Extensions.Entities
{
    internal static class IfcCadBendEntityExtensions
    {
        public static StartBendEntity ToStartBendEntity(this IfcCadBendEntity ifcCadBendEntity)
        {
            StartBendEntity startBendEntity = new StartBendEntity();
            startBendEntity.Name = ifcCadBendEntity.Name.Value;

            bool hasStartType = Enum.TryParse(ifcCadBendEntity.Tag.Value, out StartElementType elementType);
            startBendEntity.Type = hasStartType ? elementType : StartElementType.ELBOW;
            startBendEntity.Radius = LengthProperty.CreateFromSi(ifcCadBendEntity.BendRadius);

            Pset_Start? psetStart = ifcCadBendEntity.PropertySets.OfType<Pset_Start>().FirstOrDefault();
            if (psetStart != null)
            {
                UpdateStartEntityFromStartPset(ref startBendEntity, psetStart);
            }
            
            return startBendEntity;
        }
        
        private static void UpdateStartEntityFromStartPset(ref StartBendEntity startBendEntity, Pset_Start psetStart)
        {
            Dictionary<string, string>? data = psetStart.Data;
        }
        
        private static double GetPropertyValue(string rawValue) => Pset_StartExtensions.GetDoublePropertyValue(rawValue);
    }
}