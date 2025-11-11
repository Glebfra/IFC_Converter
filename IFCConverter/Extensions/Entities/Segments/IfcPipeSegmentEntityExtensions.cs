using System;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Segments;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.API;
using Start.Entities.Segments;
using Start.StartProperties;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Segments
{
    internal static class IfcPipeSegmentEntityExtensions
    {
        public static IfcPipeSegmentEntity CreateFromStart(StartPipeEntity pipeEntity, IfcNodeEntity[] nodeEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreatePipeObjectMatrix(pipeEntity, nodeEntities, out double length);
            
            string name = pipeEntity.Name;
            string type = pipeEntity.Type.ToString();
            
            IfcPipeSegmentEntity pipeSegment = new IfcPipeSegmentEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntities),
                type,
                objectMatrix3D,
                length,
                pipeEntity.Diameter.SIProperty,
                nodeEntities
            );

            pipeSegment.PropertySets.Add(Pset_Start.CreateFromStart(pipeEntity));
            pipeSegment.PropertySets.Add(Pset_PipeSegmentTypeCommon.CreateFromStart(pipeEntity));
            pipeSegment.PropertySets.Add(Qto_PipeSegmentBaseQuantities.CreateFromStart(pipeEntity));

            return pipeSegment;
        }
        
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
                startPipeEntity.UpdateFromStartPset(psetStart);

            return startPipeEntity;
        }
    }
}