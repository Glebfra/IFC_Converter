using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract;
using IFC.Entities.Abstract.Equipments;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Equipments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Equipments.Vertex
{
    public sealed class IfcVertexCompressorEntity : IfcAbstractVertexCompressorEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        
        public IfcVertexCompressorEntity(StartCompressorEntity compressorEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(compressorEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            NumSegments = numSegments;
            Length = AbstractSegmentEntities[0].Diameter / 2;
        }
    }
}