using IFC.Entities;
using IFC.Entities.Segments;
using IFC.Extensions;
using Start.Entities.Segments;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Segments
{
    #if NEW
    
    internal static class IfcPipeSegmentEntityExtensions
    {
        public static IfcPipeSegmentEntity CreateFromStart(StartPipeEntity pipeEntity, IfcNodeEntity[] nodeEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreatePipeObjectMatrix(pipeEntity, nodeEntities, out double length);
            
            IfcPipeSegmentEntity pipeSegment = new IfcPipeSegmentEntity(
                pipeEntity.Name,
                pipeEntity.Type.ToString(),
                objectMatrix3D,
                length,
                pipeEntity.Diameter.SIProperty
            );
            
            pipeSegment.PropertySets.Add(Pset_StartExtensions.CreateFromStart(pipeEntity));
            pipeSegment.PropertySets.Add(Pset_PipeSegmentTypeCommonExtensions.CreateFromStart(pipeEntity));
            pipeSegment.PropertySets.Add(Qto_PipeSegmentBaseQuantitiesExtensions.CreateFromStart(pipeEntity));

            return pipeSegment;
        }
    }
    
    #endif
}