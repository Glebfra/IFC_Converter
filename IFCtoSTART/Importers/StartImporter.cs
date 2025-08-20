using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Segments;
using Xbim.Ifc4.HvacDomain;

namespace IFCtoSTART.Importers
{
    internal class StartImporter : StandardImporter
    {
        public override IfcPipeSegmentEntity[] CreatePipeSegments(IfcPipeSegment[] pipeSegments)
        {
            return base.CreatePipeSegments(pipeSegments);
        }

        public override IfcWeldedTeeEntity[] CreateWeldedTees(IfcPipeFitting[] tees, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            return base.CreateWeldedTees(tees, abstractSegmentEntities);
        }

        public override IfcCadBendEntity[] CreateBends(IfcPipeFitting[] bends, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            return base.CreateBends(bends, abstractSegmentEntities);
        }
    }
}