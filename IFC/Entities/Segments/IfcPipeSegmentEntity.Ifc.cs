using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IFC.Entities.Abstract.Segments;
using IFC.EntitiesExtensions;
using Start.API;
using Start.Entities.Segments;
using Start.StartProperties;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;

namespace IFC.Entities.Segments
{
    public sealed partial class IfcPipeSegmentEntity : IfcAbstractPipeSegmentEntity
    {
        public static IfcPipeSegmentEntity? CreateFromIfc(IfcPipeSegment pipeSegment, IfcNodeEntity[] nodeEntities)
        {
            IfcPipeSegmentEntity? pipeSegmentEntity = null;
            
            IIfcPropertySet? psetStart = pipeSegment.PropertySets.First(set => set.Name == "Pset_Start");
            if (psetStart != null)
            {
                StartPipeEntity pipeEntity = CreatePipeFromPsetStart(psetStart);
                pipeSegmentEntity = new IfcPipeSegmentEntity(pipeEntity, nodeEntities);
            }
            
            IIfcPropertySet? psetTypeCommon = pipeSegment.PropertySets.First(set => set.Name == "Pset_PipeSegmentTypeCommon");
            if (psetTypeCommon != null && pipeSegmentEntity == null)
            {
                StartPipeEntity pipeEntity = CreatePipeFromPsetTypeCommon(psetTypeCommon);
                pipeSegmentEntity = new IfcPipeSegmentEntity(pipeEntity, nodeEntities);
            }

            return pipeSegmentEntity;
        }

        private static StartPipeEntity CreatePipeFromPsetStart(IIfcPropertySet psetStart)
        {
            double GetPropertyValue(string rawValue)
            {
                Regex regex = new Regex(@"(\d+.\d+)|\d+");
                Match match = regex.Match(rawValue);
                return Convert.ToDouble(match.Value);
            }

            Pset_Start pset = Pset_Start.CreateFromPropertySet(psetStart);
            Dictionary<string, string> data = pset.Data;
            bool isValidTechnology = Enum.TryParse(data["ManufacturingTechnologyEnum"], out StartManufacturingTechnologyEnum manufacturingTechnologyEnum);
            return new StartPipeEntity()
            {
                MaterialName = data["MaterialName"],
                MillTolerance = new LengthProperty(GetPropertyValue(data["MillTolerance"])),
                CorrosionAllowance = new LengthProperty(GetPropertyValue(data["CorrosionAllowance"])),
                PipeUnitWeight = new MassUnitProperty(GetPropertyValue(data["PipeUnitWeight"])),
                InsulationUnitWeight = new MassUnitProperty(GetPropertyValue(data["InsulationUnitWeight"])),
                ProductUnitWeight = new MassUnitProperty(GetPropertyValue(data["ProductUnitWeight"])),
                ManufacturingTechnologyEnum = isValidTechnology ? manufacturingTechnologyEnum : StartManufacturingTechnologyEnum.SEAMLESS,
                LongitudinalWeldJointFactor = new FactorProperty(GetPropertyValue(data["LongitudinalWeldJointFactor"])),
                StrengthFactorOfTheTraverseWeld = new FactorProperty(GetPropertyValue(data["StrengthFactorOfTheTraverseWeld"])),
                AdditionalWeightLoad = new MassUnitProperty(GetPropertyValue(data["AdditionalWeightLoad"])),
                AdditionalWeightLoadAlongTheXAxis = new MassUnitProperty(GetPropertyValue(data["AdditionalWeightLoadAlongTheXAxis"])),
                AdditionalWeightLoadAlongTheYAxis = new MassUnitProperty(GetPropertyValue(data["AdditionalWeightLoadAlongTheYAxis"])),
                AdditionalWeightLoadAlongTheZAxis = new MassUnitProperty(GetPropertyValue(data["AdditionalWeightLoadAlongTheZAxis"])),
                ProjectionAlongOXAxis = new LengthProperty(GetPropertyValue(data["ProjectionAlongOXAxis"])),
                ProjectionAlongOYAxis = new LengthProperty(GetPropertyValue(data["ProjectionAlongOYAxis"])),
                ProjectionAlongOZAxis = new LengthProperty(GetPropertyValue(data["ProjectionAlongOZAxis"])),
                XCoord = new LengthProperty(GetPropertyValue(data["XCoord"])),
                YCoord = new LengthProperty(GetPropertyValue(data["YCoord"])),
                ZCoord = new LengthProperty(GetPropertyValue(data["ZCoord"])),
            };
        }

        private static StartPipeEntity CreatePipeFromPsetTypeCommon(IIfcPropertySet psetTypeCommon)
        {
            Pset_PipeSegmentTypeCommon pset = Pset_PipeSegmentTypeCommon.CreateFromPropertySet(psetTypeCommon);
            return new StartPipeEntity()
            {
                Diameter = new LengthProperty(pset.NominalDiameter),
                WallThickness = new LengthProperty(pset.NominalDiameter - pset.InnerDiameter),
            };
        }
    }
}