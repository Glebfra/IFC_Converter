using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.EntitiesExtensions;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Fittings;
using Start.StartProperties;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcWeldedTeeEntity : IfcAbstractTeeEntity
    {
        public override double Length { get; protected set; }
        public override double Height { get; protected set; }
        public override double Angle { get; protected set; }
        
        public IfcWeldedTeeEntity(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(teeEntity, nodeEntity, segmentEntities)
        {
            XbimVector3D right = IfcAxis.GetPipeDirectionFromNode(_HeadPipe, ObjectMatrix3D.Translation).Normalized();
            Angle = ObjectMatrix3D.Forward.Angle(right);
            Length = teeEntity.HeaderLength.SIProperty;
            if (Length == 0) 
                Length = _HeadPipe.Diameter;
            Height = teeEntity.CrotchHeight.SIProperty + _BranchPipes[0].Diameter / 2;
        }

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

        private static void UpdateFromPsetStart(IIfcPropertySet psetStart, ref StartTeeEntity pipeFitting)
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