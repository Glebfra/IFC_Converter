using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
using IFCConverter.Extensions.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.API;
using Start.Entities.Fittings;
using Start.StartProperties;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
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
                startTeeEntity.UpdateFromStartPset(psetStart);

            return startTeeEntity;
        }
    }
}