using IFC.Entities;
using IFC.Entities.Segments;
using IFC.Extensions;
using IFC.PropertySets;
using Start.Entities.Segments;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions
{
    internal static class IfcPipeSegmentEntityExtensions
    {
        public static NewIfcPipeSegmentEntity CreateFromStart(StartPipeEntity pipeEntity, IfcNodeEntity[] nodeEntities)
        {
            XbimVector3D coordinates = nodeEntities[0].ObjectMatrix3D.Translation;
            XbimVector3D nodesDirection = nodeEntities[1].ObjectMatrix3D.Translation - coordinates;
            XbimVector3D pipeProjection = new XbimVector3D(
                pipeEntity.ProjectionAlongOXAxis.SIProperty,
                pipeEntity.ProjectionAlongOYAxis.SIProperty,
                pipeEntity.ProjectionAlongOZAxis.SIProperty
            );
            XbimVector3D direction = (pipeProjection * XbimVector3D.DotProduct(nodesDirection, pipeProjection)).Normalized() * pipeProjection.Length;
            XbimVector3D forward = direction.Normalized();
            XbimMatrix3D objectMatrix3D = MatrixExtensions.CreateWorld(coordinates, forward);
            
            NewIfcPipeSegmentEntity pipeSegment = new NewIfcPipeSegmentEntity(
                pipeEntity.Name,
                pipeEntity.Type.ToString(),
                objectMatrix3D,
                direction.Length,
                pipeEntity.Diameter.SIProperty
            );
            
            pipeSegment.PropertySets.Add(Pset_Start.CreateFromStart(pipeEntity));
            pipeSegment.PropertySets.Add(Pset_PipeSegmentTypeCommon.CreateFromStart(pipeEntity));
            pipeSegment.PropertySets.Add(Qto_PipeSegmentBaseQuantities.CreateFromStart(pipeEntity));

            return pipeSegment;
        }
    }
}