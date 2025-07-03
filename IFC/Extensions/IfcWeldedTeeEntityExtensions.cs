using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.EntitiesExtensions;
using Start.Entities.Fittings;
using Start.StartProperties;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;

namespace IFC.Extensions
{
    public static class IfcWeldedTeeEntityExtensions
    {
        public static IfcWeldedTeeEntity CreateFromIfc(IfcPipeFitting pipeFitting, IfcAbstractSegmentEntity[] segmentEntities)
        {
            StartTeeEntity teeEntity = new StartTeeEntity();
            teeEntity.Name = pipeFitting.Name ?? string.Empty;
            
            IIfcPropertySet? psetStart = pipeFitting.PropertySets.FirstOrDefault(set => set.Name == nameof(Pset_Start));
            if (psetStart != null)
                UpdateFromPsetStart(psetStart, ref teeEntity);

            XbimMatrix3D matrix3D = pipeFitting.ObjectPlacement.ToMatrix3D();
            IfcNodeEntity nodeEntity = IfcNodeEntity.CreateFromIfc(matrix3D.Translation, 1);
            
            IfcAbstractSegmentEntity[] nearestSegments = segmentEntities.GetNearestSegments(nodeEntity, 3);
            return new IfcWeldedTeeEntity(teeEntity, nodeEntity, nearestSegments);
        }

        public static void UpdateFromPsetStart(IIfcPropertySet psetStart, ref StartTeeEntity pipeFitting)
        {
            double GetPropertyValue(string rawValue)
            {
                Regex regex = new Regex(@"(\d+.\d+)|\d+");
                Match match = regex.Match(rawValue);
                return Convert.ToDouble(match.Value);
            }
            
            Pset_Start pset = Pset_Start.CreateFromPropertySet(psetStart);
            Dictionary<string, string> data = pset.Data;

            pipeFitting.HeaderThickness = LengthProperty.CreateFromSi(GetPropertyValue(data[nameof(pipeFitting.HeaderThickness)]));
            pipeFitting.MillTolerance = LengthProperty.CreateFromSi(GetPropertyValue(data[nameof(pipeFitting.MillTolerance)]));
            pipeFitting.HeaderLength = LengthProperty.CreateFromSi(GetPropertyValue(data[nameof(pipeFitting.HeaderLength)]));
            pipeFitting.BranchWallThickness = LengthProperty.CreateFromSi(GetPropertyValue(data[nameof(pipeFitting.BranchWallThickness)]));
            pipeFitting.MillToleranceForBranch = LengthProperty.CreateFromSi(GetPropertyValue(data[nameof(pipeFitting.MillToleranceForBranch)]));
            pipeFitting.BranchHeight = LengthProperty.CreateFromSi(GetPropertyValue(data[nameof(pipeFitting.BranchHeight)]));
            pipeFitting.PadThickness = LengthProperty.CreateFromSi(GetPropertyValue(data[nameof(pipeFitting.PadThickness)]));
            pipeFitting.PadWidth = LengthProperty.CreateFromSi(GetPropertyValue(data[nameof(pipeFitting.PadWidth)]));
            pipeFitting.CrotchHeight = LengthProperty.CreateFromSi(GetPropertyValue(data[nameof(pipeFitting.CrotchHeight)]));
            pipeFitting.CrotchThickness = LengthProperty.CreateFromSi(GetPropertyValue(data[nameof(pipeFitting.CrotchThickness)]));
            pipeFitting.StrengthFactorOfLongitudinalWeldSeamOnPressure = FactorProperty.CreateFromSi(GetPropertyValue(data[nameof(pipeFitting.StrengthFactorOfLongitudinalWeldSeamOnPressure)]));
            pipeFitting.CrotchRadius = LengthProperty.CreateFromSi(GetPropertyValue(data[nameof(pipeFitting.CrotchRadius)]));
        }
    }
}