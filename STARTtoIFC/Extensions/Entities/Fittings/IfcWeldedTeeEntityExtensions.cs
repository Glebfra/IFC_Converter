using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
using Start.API;
using Start.Entities.Fittings;
using Start.StartProperties;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    internal static class IfcWeldedTeeEntityExtensions
    {
        public static IfcWeldedTeeEntity CreateFromStart(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);
            
            double length = teeEntity.HeaderLength.SIProperty;
            if (length == 0) 
                length = headPipe.Diameter;
            double height = teeEntity.CrotchHeight.SIProperty + branchPipes[0].Diameter / 2;
            
            IfcWeldedTeeEntity weldedTeeEntity = new IfcWeldedTeeEntity(
                teeEntity.Name,
                teeEntity.Type.ToString(),
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            weldedTeeEntity.ConnectedEntities.AddRange(segmentEntities);
            weldedTeeEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(teeEntity));
            weldedTeeEntity.PropertySets.Add(Pset_PipeFittingTypeJunctionExtensions.CreateFromStart(teeEntity));
            weldedTeeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(teeEntity));

            return weldedTeeEntity;
        }
        
        public static StartTeeEntity ToStartTeeEntity(this IfcWeldedTeeEntity weldedTeeEntity)
        {
            StartTeeEntity startTeeEntity = new StartTeeEntity();
            startTeeEntity.Name = weldedTeeEntity.Name.Value;

            bool hasStartType = Enum.TryParse(weldedTeeEntity.Tag.Value, out StartElementType elementType);
            startTeeEntity.Type = hasStartType ? elementType : StartElementType.WELDED_TEE;
            startTeeEntity.HeaderLength = LengthProperty.CreateFromSi(weldedTeeEntity.Length);
            startTeeEntity.CrotchHeight = LengthProperty.CreateFromSi(weldedTeeEntity.Height);
            
            Pset_Start? psetStart = weldedTeeEntity.PropertySets.OfType<Pset_Start>().FirstOrDefault();
            if (psetStart != null)
                UpdateStartEntityFromStartPset(ref startTeeEntity, psetStart);
            
            return startTeeEntity;
        }
        
        private static void UpdateStartEntityFromStartPset(ref StartTeeEntity startTeeEntity, Pset_Start psetStart)
        {
            Dictionary<string, string>? data = psetStart.Data;
            
            if (data.TryGetValue(nameof(startTeeEntity.ManufacturingTechnologyEnum), out string manufacturingTechnology))
                startTeeEntity.ManufacturingTechnologyEnum = ManufacturingTechnologyExtensions.GetManufacturingTechnology(manufacturingTechnology);
            if (data.TryGetValue(nameof(startTeeEntity.MaterialName), out string materialName))
                startTeeEntity.MaterialName = materialName;
            if (data.TryGetValue(nameof(startTeeEntity.HeaderLength), out string headerLength))
                startTeeEntity.HeaderLength = LengthProperty.CreateFromSi(GetPropertyValue(headerLength));
            if (data.TryGetValue(nameof(startTeeEntity.BranchHeight), out string branchHeight))
                startTeeEntity.BranchHeight = LengthProperty.CreateFromSi(GetPropertyValue(branchHeight));
            if (data.TryGetValue(nameof(startTeeEntity.CrotchRadius), out string crotchRadius))
                startTeeEntity.CrotchRadius = LengthProperty.CreateFromSi(GetPropertyValue(crotchRadius));
            if (data.TryGetValue(nameof(startTeeEntity.CrotchThickness), out string crotchThickness))
                startTeeEntity.CrotchThickness = LengthProperty.CreateFromSi(GetPropertyValue(crotchThickness));
            if (data.TryGetValue(nameof(startTeeEntity.HeaderThickness), out string headerThickness))
                startTeeEntity.HeaderThickness = LengthProperty.CreateFromSi(GetPropertyValue(headerThickness));
            if (data.TryGetValue(nameof(startTeeEntity.MillTolerance), out string millTolerance))
                startTeeEntity.MillTolerance = LengthProperty.CreateFromSi(GetPropertyValue(millTolerance));
            if (data.TryGetValue(nameof(startTeeEntity.PadThickness), out string padThickness))
                startTeeEntity.PadThickness = LengthProperty.CreateFromSi(GetPropertyValue(padThickness));
            if (data.TryGetValue(nameof(startTeeEntity.PadWidth), out string padWidth))
                startTeeEntity.PadWidth = LengthProperty.CreateFromSi(GetPropertyValue(padWidth));
            if (data.TryGetValue(nameof(startTeeEntity.BranchWallThickness), out string branchWallThickness))
                startTeeEntity.BranchWallThickness = LengthProperty.CreateFromSi(GetPropertyValue(branchWallThickness));
            if (data.TryGetValue(nameof(startTeeEntity.MillToleranceForBranch), out string millToleranceForBranch))
                startTeeEntity.MillToleranceForBranch = LengthProperty.CreateFromSi(GetPropertyValue(millToleranceForBranch));
            if (data.TryGetValue(nameof(startTeeEntity.StrengthFactorOfLongitudinalWeldSeamOnPressure), out string strengthFactorOfLongitudinalWeldSeamOnPressure))
                startTeeEntity.StrengthFactorOfLongitudinalWeldSeamOnPressure = FactorProperty.CreateFromSi(GetPropertyValue(strengthFactorOfLongitudinalWeldSeamOnPressure));
        }
        
        private static double GetPropertyValue(string rawValue) => Pset_StartExtensions.GetDoublePropertyValue(rawValue);
    }
}