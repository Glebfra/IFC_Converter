using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IFC.Entities.Segments;
using IFC.PropertySets;
using Start.API;
using Start.Entities.Segments;
using Start.StartProperties;
using Xbim.Common.Geometry;

namespace IfcConverter.Extensions
{
    public static partial class IfcPipeSegmentExtensions
    {
        public static StartPipeEntity ConvertToStartFromStartEntity(this IfcPipeSegmentEntity pipeSegmentEntity)
        {
            StartPipeEntity startPipeEntity = new StartPipeEntity();

            foreach (IPropertySet propertySet in pipeSegmentEntity.PropertySets)
            {
                if (propertySet is Pset_Start psetStart)
                {
                    UpdateFromPsetStart(psetStart, ref startPipeEntity);
                }

                if (propertySet is Pset_PipeSegmentTypeCommon psetPipeSegmentTypeCommon)
                {
                    UpdateFromPsetPipeSegmentTypeCommon(psetPipeSegmentTypeCommon, ref startPipeEntity);
                }
            }
            
            UpdateFromIfcEntity(pipeSegmentEntity, ref startPipeEntity);

            return startPipeEntity;
        }
        
        public static void UpdateFromIfcEntity(IfcPipeSegmentEntity pipeSegmentEntity, ref StartPipeEntity startPipeEntity)
        {
            startPipeEntity.Name = pipeSegmentEntity.Name.Value;
            startPipeEntity.Diameter = LengthProperty.CreateFromSi(pipeSegmentEntity.Diameter.Value);

            XbimMatrix3D objectMatrix3D = pipeSegmentEntity.ObjectMatrix3D;
            startPipeEntity.XCoord = LengthProperty.CreateFromSi(objectMatrix3D.Translation.X);
            startPipeEntity.YCoord = LengthProperty.CreateFromSi(objectMatrix3D.Translation.Y);
            startPipeEntity.ZCoord = LengthProperty.CreateFromSi(objectMatrix3D.Translation.Z);

            XbimVector3D projection = objectMatrix3D.Forward * pipeSegmentEntity.Length;
            startPipeEntity.ProjectionAlongOXAxis = LengthProperty.CreateFromSi(projection.X);
            startPipeEntity.ProjectionAlongOYAxis = LengthProperty.CreateFromSi(projection.Y);
            startPipeEntity.ProjectionAlongOZAxis = LengthProperty.CreateFromSi(projection.Z);
        }
        
        public static void UpdateFromPsetStart(Pset_Start pset, ref StartPipeEntity pipeEntity)
        {
            double GetPropertyValue(string rawValue)
            {
                Regex regex = new Regex(@"-(\d+,\d+)|-(\d+.\d+)|-\d+|(\d+,\d+)|(\d+.\d+)|\d+");
                Match match = regex.Match(rawValue);
                return Convert.ToDouble(match.Value);
            }

            Dictionary<string, string> data = pset.Data;
            
            if (data.TryGetValue(nameof(pipeEntity.ManufacturingTechnologyEnum), out string manufacturingTechnology))
            {
                bool isValidTechnology = Enum.TryParse(data["ManufacturingTechnologyEnum"], out StartManufacturingTechnologyEnum manufacturingTechnologyEnum);
                pipeEntity.ManufacturingTechnologyEnum = isValidTechnology ? manufacturingTechnologyEnum : StartManufacturingTechnologyEnum.SEAMLESS;
            }

            if (data.TryGetValue(nameof(pipeEntity.Diameter), out string diameter))
                pipeEntity.Diameter = LengthProperty.CreateFromSi(GetPropertyValue(diameter));
            if (data.TryGetValue(nameof(pipeEntity.MaterialName), out string materialName))
                pipeEntity.MaterialName = materialName;
            if (data.TryGetValue(nameof(pipeEntity.MillTolerance), out string millTolerance))
                pipeEntity.MillTolerance = LengthProperty.CreateFromSi(GetPropertyValue(millTolerance));
            if (data.TryGetValue(nameof(pipeEntity.CorrosionAllowance), out string corrosionAllowance))
                pipeEntity.CorrosionAllowance = LengthProperty.CreateFromSi(GetPropertyValue(corrosionAllowance));
            if (data.TryGetValue(nameof(pipeEntity.PipeUnitWeight), out string pipeUnitWeight))
                pipeEntity.PipeUnitWeight = MassUnitProperty.CreateFromSi(GetPropertyValue(pipeUnitWeight));
            if (data.TryGetValue(nameof(pipeEntity.InsulationUnitWeight), out string insulationWeight))
                pipeEntity.InsulationUnitWeight = MassUnitProperty.CreateFromSi(GetPropertyValue(insulationWeight));
            if (data.TryGetValue(nameof(pipeEntity.ProductUnitWeight), out string productUnitWeight))
                pipeEntity.ProductUnitWeight = MassUnitProperty.CreateFromSi(GetPropertyValue(productUnitWeight));
            if (data.TryGetValue(nameof(pipeEntity.LongitudinalWeldJointFactor), out string longitudinalWeldJointFactor))
                pipeEntity.LongitudinalWeldJointFactor = FactorProperty.CreateFromSi(GetPropertyValue(longitudinalWeldJointFactor));
            if (data.TryGetValue(nameof(pipeEntity.StrengthFactorOfTheTraverseWeld), out string strengthFactorOfTraverseWeld))
                pipeEntity.StrengthFactorOfTheTraverseWeld = FactorProperty.CreateFromSi(GetPropertyValue(strengthFactorOfTraverseWeld));
            if (data.TryGetValue(nameof(pipeEntity.AdditionalWeightLoad), out string additionalWeightLoad))
                pipeEntity.AdditionalWeightLoad = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoad));
            if (data.TryGetValue(nameof(pipeEntity.AdditionalWeightLoadAlongTheXAxis), out string additionalWeightLoadAlongTheXAxis))
                pipeEntity.AdditionalWeightLoadAlongTheXAxis = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoadAlongTheXAxis));
            if (data.TryGetValue(nameof(pipeEntity.AdditionalWeightLoadAlongTheYAxis), out string additionalWeightLoadAlongTheYAxis))
                pipeEntity.AdditionalWeightLoadAlongTheYAxis = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoadAlongTheYAxis));
            if (data.TryGetValue(nameof(pipeEntity.AdditionalWeightLoadAlongTheZAxis), out string additionalWeightLoadAlongTheZAxis))
                pipeEntity.AdditionalWeightLoadAlongTheZAxis = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoadAlongTheZAxis));
        }

        public static void UpdateFromPsetPipeSegmentTypeCommon(Pset_PipeSegmentTypeCommon pset, ref StartPipeEntity startPipeEntity)
        {
            startPipeEntity.Diameter = LengthProperty.CreateFromSi(pset.NominalDiameter);
            startPipeEntity.WallThickness = LengthProperty.CreateFromSi(pset.NominalDiameter - pset.InnerDiameter);
        }
    }
}