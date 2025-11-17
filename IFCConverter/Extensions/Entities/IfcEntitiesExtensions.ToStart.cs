using System;
using System.Linq;
using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Fittings.Vertex;
using IFC.Entities.Segments;
using IFC.PropertySets;
using Start.API;
using Start.Entities.Anchors;
using Start.Entities.Fittings;
using Start.Entities.Segments;
using Start.StartProperties;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities
{
    internal static partial class IfcEntitiesExtensions
    {
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

        public static StartBendEntity ToStartBendEntity(this IfcCadBendEntity ifcCadBendEntity)
        {
            StartBendEntity startBendEntity = new StartBendEntity();
            startBendEntity.Name = ifcCadBendEntity.Name.Value;

            bool hasStartType = Enum.TryParse(ifcCadBendEntity.Tag.Value, out StartElementType elementType);
            startBendEntity.Type = hasStartType ? elementType : StartElementType.ELBOW;
            startBendEntity.Radius = LengthProperty.CreateFromSi(ifcCadBendEntity.BendRadius);

            Pset_Start? psetStart = ifcCadBendEntity.PropertySets.OfType<Pset_Start>().FirstOrDefault();
            if (psetStart != null)
                startBendEntity.UpdateFromStartPset(psetStart);

            return startBendEntity;
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
        
        public static StartReducerEntity ToStartReducerEntity(this IfcAbstractReducerEntity reducerEntity)
        {
            StartReducerEntity startReducerEntity = new StartReducerEntity();
            startReducerEntity.Name = reducerEntity.Name.Value;

            StartElementType defaultType = reducerEntity switch 
            {
                IfcVertexReducerConcentricEntity => StartElementType.REDUCER_CONCENTRIC,
                IfcVertexReducerEccentricEntity => StartElementType.REDUCER_ECCENTRIC,
                _ => StartElementType.REDUCER_CONCENTRIC
            };

            bool hasStartType = Enum.TryParse(reducerEntity.Tag.Value, out StartElementType elementType);
            startReducerEntity.Type = hasStartType ? elementType : defaultType;
            
            startReducerEntity.LengthOfConicalPart = LengthProperty.CreateFromSi(reducerEntity.Length);
            
            double[] diameters = reducerEntity.Diameters
                .Select(diameter => diameter.Value)
                .ToArray();
            startReducerEntity.MinDiameter = LengthProperty.CreateFromSi(diameters.Min());
            startReducerEntity.MaxDiameter = LengthProperty.CreateFromSi(diameters.Max());

            Pset_Start? psetStart = reducerEntity.PropertySets.OfType<Pset_Start>().FirstOrDefault();
            if (psetStart != null)
                startReducerEntity.UpdateFromStartPset(psetStart);

            return startReducerEntity;
        }
        
        public static StartAnchorEntity ToStartAnchorEntity(this IfcAbstractAnchorEntity anchorEntity)
        {
            StartAnchorEntity startAnchorEntity = new StartAnchorEntity();
            startAnchorEntity.Name = anchorEntity.Name.Value;

            bool hasStartType = Enum.TryParse(anchorEntity.Tag.Value, out StartElementType type);
            startAnchorEntity.Type = hasStartType ? type : StartElementType.SLIDING_SUPPORT;
            
            Pset_Start? psetStart = anchorEntity.PropertySets.OfType<Pset_Start>().FirstOrDefault();
            if (psetStart != null)
                startAnchorEntity.UpdateFromStartPset(psetStart);

            return startAnchorEntity;
        }

        public static StartArmatureEntity ToStartArmatureEntity(this IfcVertexValveEntity valveEntity)
        {
            StartArmatureEntity startArmatureEntity = new StartArmatureEntity();
            startArmatureEntity.Name = valveEntity.Name.Value;

            bool hasStartType = Enum.TryParse(valveEntity.Tag.Value, out StartElementType type);
            startArmatureEntity.Type = hasStartType ? type : StartElementType.VALVE;
            
            startArmatureEntity.Length = LengthProperty.CreateFromSi(valveEntity.Length);
            startArmatureEntity.OutsideDiameter = LengthProperty.CreateFromSi(valveEntity.Diameter);

            Pset_Start? psetStart = valveEntity.PropertySets.OfType<Pset_Start>().FirstOrDefault();
            if (psetStart != null)
                startArmatureEntity.UpdateFromStartPset(psetStart);

            return startArmatureEntity;
        }
    }
}