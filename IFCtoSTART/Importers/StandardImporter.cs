using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Segments;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCtoSTART.Importers
{
    internal class StandardImporter : IImporter
    {
        public virtual IfcPipeSegment[] GetPipeSegments(IfcProduct[] products)
        {
            return products
                .OfType<IfcPipeSegment>()
                .ToArray();
        }

        public virtual IfcPipeFitting[] GetBends(IfcProduct[] products)
        {
            return products
                .OfType<IfcPipeFitting>()
                .Where(fitting => fitting.PredefinedType == IfcPipeFittingTypeEnum.BEND)
                .ToArray();
        }

        public virtual IfcPipeFitting[] GetTees(IfcProduct[] products)
        {
            return products
                .OfType<IfcPipeFitting>()
                .Where(fitting => fitting.PredefinedType == IfcPipeFittingTypeEnum.JUNCTION)
                .ToArray();
        }

        public virtual IfcPipeSegmentEntity[] CreatePipeSegments(IfcPipeSegment[] pipeSegments)
        {
            throw new System.NotImplementedException();
        }

        public virtual IfcCadBendEntity[] CreateBends(IfcPipeFitting[] bends, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            throw new System.NotImplementedException();
        }

        public virtual IfcWeldedTeeEntity[] CreateWeldedTees(IfcPipeFitting[] tees, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            throw new System.NotImplementedException();
        }
    }
}