using System.Collections.Generic;
using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Segments;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importers
{
    internal interface IImporter
    {
        public IEnumerable<IfcProduct> Products { get; }

        public IEnumerable<IfcPipeSegmentEntity> CreateSegments();
        public IEnumerable<IfcAbstractFittingEntity> CreateFittings(List<IfcPipeSegmentEntity> pipeSegmentEntities);
        public IEnumerable<IfcAbstractAnchorEntity> CreateAnchors(List<IfcPipeSegmentEntity> pipeSegmentEntities);
    }
}