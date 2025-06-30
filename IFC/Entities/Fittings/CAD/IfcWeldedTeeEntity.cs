using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.EntitiesExtensions;
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

            XbimMatrix3D matrix3D = pipeFitting.ObjectPlacement.ToMatrix3D();
            IfcNodeEntity nodeEntity = IfcNodeEntity.CreateFromIfc(matrix3D.Translation, 1);

            IfcAbstractSegmentEntity[] nearestSegments = GetNearestSegments(nodeEntity, segmentEntities);
            foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in nearestSegments)
            {
                ifcAbstractSegmentEntity.GetNearestNode(nodeEntity);
            }
            
            IIfcPropertySet? psetStart = pipeFitting.PropertySets.FirstOrDefault(set => set.Name == nameof(Pset_Start));
            if (psetStart != null)
                UpdateFromPsetStart(psetStart, ref teeEntity);

            IIfcPropertySet? psetTypeCommon = pipeFitting.PropertySets.FirstOrDefault(set => set.Name == nameof(Pset_PipeFittingTypeJunction));
            if (psetTypeCommon != null)
                UpdateFromPsetTypeCommon(psetTypeCommon, ref teeEntity);

            return new IfcWeldedTeeEntity(teeEntity, nodeEntity, segmentEntities);
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

            pipeFitting.HeaderThickness = new LengthProperty(GetPropertyValue(data[nameof(pipeFitting.HeaderThickness)]));
            pipeFitting.MillTolerance = new LengthProperty(GetPropertyValue(data[nameof(pipeFitting.MillTolerance)]));
            pipeFitting.HeaderLength = new LengthProperty(GetPropertyValue(data[nameof(pipeFitting.HeaderLength)]));
            pipeFitting.BranchWallThickness = new LengthProperty(GetPropertyValue(data[nameof(pipeFitting.BranchWallThickness)]));
            pipeFitting.MillToleranceForBranch = new LengthProperty(GetPropertyValue(data[nameof(pipeFitting.MillToleranceForBranch)]));
            pipeFitting.BranchHeight = new LengthProperty(GetPropertyValue(data[nameof(pipeFitting.BranchHeight)]));
            pipeFitting.PadThickness = new LengthProperty(GetPropertyValue(data[nameof(pipeFitting.PadThickness)]));
            pipeFitting.PadWidth = new LengthProperty(GetPropertyValue(data[nameof(pipeFitting.PadWidth)]));
            pipeFitting.CrotchHeight = new LengthProperty(GetPropertyValue(data[nameof(pipeFitting.CrotchHeight)]));
            pipeFitting.CrotchThickness = new LengthProperty(GetPropertyValue(data[nameof(pipeFitting.CrotchThickness)]));
            pipeFitting.StrengthFactorOfLongitudinalWeldSeamOnPressure = new FactorProperty(GetPropertyValue(data[nameof(pipeFitting.StrengthFactorOfLongitudinalWeldSeamOnPressure)]));
            pipeFitting.CrotchRadius = new LengthProperty(GetPropertyValue(data[nameof(pipeFitting.CrotchRadius)]));
        }

        private static void UpdateFromPsetTypeCommon(IIfcPropertySet psetCommon, ref StartTeeEntity pipeFitting)
        {
            Pset_PipeFittingTypeJunction pset = Pset_PipeFittingTypeJunction.CreateFromPropertySet(psetCommon);
        }

        private static void UpdateFromQtoPipeFitting(IIfcPropertySet psetQto, ref StartTeeEntity pipeFitting)
        {
            Qto_PipeFittingBaseQuantities qto = Qto_PipeFittingBaseQuantities.CreateFromPropertySet(psetQto);
        }

        private static IfcAbstractSegmentEntity[] GetNearestSegments(IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            return segmentEntities.OrderBy(item =>
                (item.GetNearestNode(nodeEntity).NodeEntity.ObjectMatrix3D.Translation - nodeEntity.ObjectMatrix3D.Translation).Modulus)
                .Take(3)
                .ToArray();
        }
    }
}