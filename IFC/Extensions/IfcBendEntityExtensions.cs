using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
using Start.API;
using Start.Entities.Fittings;
using Start.StartProperties;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;

namespace IFC.Extensions
{
    public static class IfcBendEntityExtensions
    {
        public static IfcCadBendEntity CreateFromIfc(IfcPipeFitting pipeFitting, IfcAbstractSegmentEntity[] segmentEntities)
        {
            StartBendEntity startBendEntity = new StartBendEntity();
            startBendEntity.Name = pipeFitting.Name ?? string.Empty;

            XbimMatrix3D matrix3D = pipeFitting.ObjectPlacement.ToMatrix3D();
            IfcNodeEntity nodeEntity = IfcNodeEntity.CreateFromIfc(matrix3D.Translation);
            IfcAbstractSegmentEntity[] nearestSegments = segmentEntities.GetNearestSegments(nodeEntity, 2);

            IIfcPropertySet? psetStart = pipeFitting.PropertySets.FirstOrDefault(set => set.Name == nameof(Pset_Start));
            if (psetStart != null)
            {
                UpdateFromPsetStart(psetStart, ref startBendEntity);
                return new IfcCadBendEntity(startBendEntity, nodeEntity, nearestSegments);
            }

            IIfcPropertySet? psetTypeBend = pipeFitting.PropertySets.FirstOrDefault(set => set.Name == nameof(Pset_PipeFittingTypeBend));
            if (psetTypeBend != null)
                UpdateFromPsetTypeBend(psetTypeBend, ref startBendEntity);

            return new IfcCadBendEntity(startBendEntity, nodeEntity, nearestSegments);
        }

        public static void UpdateFromPsetStart(IIfcPropertySet psetStart, ref StartBendEntity bendEntity)
        {
            double GetPropertyValue(string rawValue)
            {
                Regex regex = new Regex(@"-(\d+,\d+)|-(\d+.\d+)|-\d+|(\d+,\d+)|(\d+.\d+)|\d+");
                Match match = regex.Match(rawValue);
                return Convert.ToDouble(match.Value);
            }
            
            Pset_Start pset = Pset_Start.CreateFromPropertySet(psetStart);
            Dictionary<string, string> data = pset.Data;
            
            if (data.TryGetValue(nameof(bendEntity.ManufacturingTechnologyEnum), out string manufacturingTechnology))
            {
                bool isValidTechnology = Enum.TryParse(manufacturingTechnology, out StartManufacturingTechnologyEnum manufacturingTechnologyEnum);
                bendEntity.ManufacturingTechnologyEnum = isValidTechnology ? manufacturingTechnologyEnum : StartManufacturingTechnologyEnum.SEAMLESS;
            }

            if (data.TryGetValue(nameof(bendEntity.Radius), out string radius))
                bendEntity.Radius = LengthProperty.CreateFromSi(GetPropertyValue(radius));
            if (data.TryGetValue(nameof(bendEntity.MillTolerance), out string millTolerance))
                bendEntity.MillTolerance = LengthProperty.CreateFromSi(GetPropertyValue(millTolerance));
            if (data.TryGetValue(nameof(bendEntity.OvalizationCoefficient), out string ovalizationCoefficient))
                bendEntity.OvalizationCoefficient = FactorProperty.CreateFromSi(GetPropertyValue(ovalizationCoefficient));
            if (data.TryGetValue(nameof(bendEntity.WallThickness), out string wallThickness))
                bendEntity.WallThickness = LengthProperty.CreateFromSi(GetPropertyValue(wallThickness));
            if (data.TryGetValue(nameof(bendEntity.MillToleranceOutside), out string millToleranceOutside))
                bendEntity.MillToleranceOutside = LengthProperty.CreateFromSi(GetPropertyValue(millToleranceOutside));
            if (data.TryGetValue(nameof(bendEntity.NumberOfMilters), out string numberOfMilters))
                bendEntity.NumberOfMilters = NumberProperty.CreateFromSi((int)GetPropertyValue(numberOfMilters));
            if (data.TryGetValue(nameof(bendEntity.Weight), out string weight))
                bendEntity.Weight = MassProperty.CreateFromSi(GetPropertyValue(weight));
        }

        public static void UpdateFromPsetTypeBend(IIfcPropertySet psetTypeBend, ref StartBendEntity bendEntity)
        {
            Pset_PipeFittingTypeBend pset = Pset_PipeFittingTypeBend.CreateFromPropertySet(psetTypeBend);
            bendEntity.Radius = LengthProperty.CreateFromSi(pset.BendRadius);
        }
    }
}