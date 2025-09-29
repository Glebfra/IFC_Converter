using System;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using IFC.Extensions;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.API;
using Start.Entities.Fittings;
using Start.StartProperties;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcVertexReducerEccentricEntityExtensions
    {
        public static IfcVertexReducerEccentricEntity CreateFromStart(StartReducerEntity reducer, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            segmentEntities = segmentEntities
                .OrderBy(segment => segment.Diameter.Value)
                .ToArray();
            
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateReducerEccentricObjectMatrix(nodeEntity, segmentEntities, out double displacementLength);

            if ((segmentEntities[1].StartNode.GetDistanceToNode(nodeEntity) < segmentEntities[1].EndNode.GetDistanceToNode(nodeEntity)))
            {
                segmentEntities[1].MovePipe(objectMatrix3D.Up * displacementLength);
            }

            double length = reducer.LengthOfConicalPart.SIProperty;
            double[] diameters = segmentEntities.Select(segment => segment.Diameter.Value).ToArray();

            IfcVertexReducerEccentricEntity reducerEccentricEntity = new IfcVertexReducerEccentricEntity(
                reducer.Name,
                reducer.Type.ToString(),
                objectMatrix3D,
                length,
                displacementLength,
                diameters,
                numSegments
            );

            reducerEccentricEntity.ConnectedEntities.AddRange(segmentEntities);
            reducerEccentricEntity.PropertySets.Add(Pset_Start.CreateFromStart(reducer));
            reducerEccentricEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(reducer));

            return reducerEccentricEntity;
        }

        public static StartReducerEntity ToStartReducerEntity(this IfcVertexReducerEccentricEntity reducerEccentricEntity)
        {
            StartReducerEntity startReducerEntity = new StartReducerEntity();
            startReducerEntity.Name = reducerEccentricEntity.Name.Value;
            bool hasStartType = Enum.TryParse(reducerEccentricEntity.Tag.Value, out StartElementType elementType);
            startReducerEntity.Type = hasStartType ? elementType : StartElementType.REDUCER_ECCENTRIC;
            
            startReducerEntity.LengthOfConicalPart = LengthProperty.CreateFromSi(reducerEccentricEntity.Length);
            
            double[] diameters = reducerEccentricEntity.Diameters
                .Select(diameter => diameter.Value)
                .ToArray();
            startReducerEntity.MinDiameter = LengthProperty.CreateFromSi(diameters.Min());
            startReducerEntity.MaxDiameter = LengthProperty.CreateFromSi(diameters.Max());

            Pset_Start? psetStart = reducerEccentricEntity.PropertySets.OfType<Pset_Start>().FirstOrDefault();
            if (psetStart != null)
                startReducerEntity.UpdateFromStartPset(psetStart);

            return startReducerEntity;
        }
    }
}