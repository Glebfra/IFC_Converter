using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IFC.Entities.Segments;
using IFC.PropertySets;
using IFCtoSTART.Extensions.PropertySets;
using Start.API;
using Start.Entities.Segments;
using Start.StartProperties;
using Xbim.Common.Geometry;

namespace IFCtoSTART.Extensions.Entities
{
    internal static class IfcPipeSegmentEntityExtensions
    {
        public static StartPipeEntity ToStartPipeEntity(this IfcPipeSegmentEntity ifcPipeSegmentEntity)
        {
            StartPipeEntity startPipeEntity = new StartPipeEntity();
            startPipeEntity.Name = ifcPipeSegmentEntity.Name.Value;

            bool hasStartType = Enum.TryParse(ifcPipeSegmentEntity.Tag.Value, out StartElementType elementType);
            startPipeEntity.Type = hasStartType ? elementType : StartElementType.PIPE_ELEMENT;
            
            startPipeEntity.Diameter = LengthProperty.CreateFromSi(ifcPipeSegmentEntity.Diameter.Value);

            XbimVector3D projection = ifcPipeSegmentEntity.ObjectMatrix3D.Value.Forward * ifcPipeSegmentEntity.Length;
            startPipeEntity.ProjectionAlongOXAxis = LengthProperty.CreateFromSi(projection.X);
            startPipeEntity.ProjectionAlongOYAxis = LengthProperty.CreateFromSi(projection.Y);
            startPipeEntity.ProjectionAlongOZAxis = LengthProperty.CreateFromSi(projection.Z);

            Pset_Start? psetStart = ifcPipeSegmentEntity.PropertySets.OfType<Pset_Start>().FirstOrDefault();
            if (psetStart != null)
            {
                UpdateStartEntityFromStartPset(ref startPipeEntity, psetStart);
            }

            return startPipeEntity;
        }

        private static void UpdateStartEntityFromStartPset(ref StartPipeEntity startPipeEntity, Pset_Start psetStart)
        {
            Dictionary<string, string> data = psetStart.Data;
            
            if (data.TryGetValue(nameof(startPipeEntity.MaterialName), out string materialName))
                startPipeEntity.MaterialName = materialName;
            if (data.TryGetValue(nameof(startPipeEntity.MillTolerance), out string millTolerance))
                startPipeEntity.MillTolerance = LengthProperty.CreateFromSi(GetPropertyValue(millTolerance));
            if (data.TryGetValue(nameof(startPipeEntity.CorrosionAllowance), out string corrosionAllowance))
                startPipeEntity.CorrosionAllowance = LengthProperty.CreateFromSi(GetPropertyValue(corrosionAllowance));
            if (data.TryGetValue(nameof(startPipeEntity.PipeUnitWeight), out string pipeUnitWeight))
                startPipeEntity.PipeUnitWeight = MassUnitProperty.CreateFromSi(GetPropertyValue(pipeUnitWeight));
            if (data.TryGetValue(nameof(startPipeEntity.InsulationUnitWeight), out string insulationWeight))
                startPipeEntity.InsulationUnitWeight = MassUnitProperty.CreateFromSi(GetPropertyValue(insulationWeight));
            if (data.TryGetValue(nameof(startPipeEntity.ProductUnitWeight), out string productUnitWeight))
                startPipeEntity.ProductUnitWeight = MassUnitProperty.CreateFromSi(GetPropertyValue(productUnitWeight));
            if (data.TryGetValue(nameof(startPipeEntity.LongitudinalWeldJointFactor), out string longitudinalWeldJointFactor))
                startPipeEntity.LongitudinalWeldJointFactor = FactorProperty.CreateFromSi(GetPropertyValue(longitudinalWeldJointFactor));
            if (data.TryGetValue(nameof(startPipeEntity.StrengthFactorOfTheTraverseWeld), out string strengthFactorOfTraverseWeld))
                startPipeEntity.StrengthFactorOfTheTraverseWeld = FactorProperty.CreateFromSi(GetPropertyValue(strengthFactorOfTraverseWeld));
            if (data.TryGetValue(nameof(startPipeEntity.AdditionalWeightLoad), out string additionalWeightLoad))
                startPipeEntity.AdditionalWeightLoad = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoad));
            if (data.TryGetValue(nameof(startPipeEntity.AdditionalWeightLoadAlongTheXAxis), out string additionalWeightLoadAlongTheXAxis))
                startPipeEntity.AdditionalWeightLoadAlongTheXAxis = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoadAlongTheXAxis));
            if (data.TryGetValue(nameof(startPipeEntity.AdditionalWeightLoadAlongTheYAxis), out string additionalWeightLoadAlongTheYAxis))
                startPipeEntity.AdditionalWeightLoadAlongTheYAxis = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoadAlongTheYAxis));
            if (data.TryGetValue(nameof(startPipeEntity.AdditionalWeightLoadAlongTheZAxis), out string additionalWeightLoadAlongTheZAxis))
                startPipeEntity.AdditionalWeightLoadAlongTheZAxis = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoadAlongTheZAxis));
        }
        
        private static double GetPropertyValue(string rawValue) => Pset_StartExtensions.GetDoublePropertyValue(rawValue);
    }
}