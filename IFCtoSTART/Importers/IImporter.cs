using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Segments;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;

namespace IFCtoSTART.Importers
{
    internal interface IImporter
    {
        public IfcPipeSegment[] GetPipeSegments(IfcProduct[] products);
        public IfcPipeFitting[] GetBends(IfcProduct[] products);
        public IfcPipeFitting[] GetTees(IfcProduct[] products);
        
        public IfcPipeSegmentEntity[] CreatePipeSegments(IfcPipeSegment[] pipeSegments);
        public IfcCadBendEntity[] CreateBends(IfcPipeFitting[] bends, IfcAbstractSegmentEntity[] abstractSegmentEntities);
        public IfcWeldedTeeEntity[] CreateWeldedTees(IfcPipeFitting[] tees, IfcAbstractSegmentEntity[] abstractSegmentEntities);
    }
}