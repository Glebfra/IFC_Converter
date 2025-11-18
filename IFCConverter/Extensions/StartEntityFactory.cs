using System;
using IFC.Entities.Abstract;
using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Fittings.Vertex;
using IFC.Entities.Segments;
using IFCConverter.Extensions.Entities;
using Start.Entities.Abstract;

namespace IFCConverter.Extensions
{
    public static class StartEntityFactory
    {
        public static StartAbstractEntity CreateEntity(IfcAbstractEntity ifcAbstractEntity)
        {
            return ifcAbstractEntity switch
            {
                IfcPipeSegmentEntity pipeSegmentEntity => pipeSegmentEntity.ToStartPipeEntity(),
                IfcCadBendEntity cadBendEntity => cadBendEntity.ToStartBendEntity(),
                IfcWeldedTeeEntity weldedTeeEntity => weldedTeeEntity.ToStartTeeEntity(),
                IfcAbstractReducerEntity abstractReducerEntity => abstractReducerEntity.ToStartReducerEntity(),
                IfcAbstractAnchorEntity abstractAnchorEntity => abstractAnchorEntity.ToStartAnchorEntity(),
                IfcVertexValveEntity vertexValveEntity => vertexValveEntity.ToStartArmatureEntity(),
                _ => throw new Exception($"Cannot find start entity for {nameof(ifcAbstractEntity)}")
            };
        }
    }
}