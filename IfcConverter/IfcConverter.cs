using System.Linq;
using IFC;
using IFC.Entities.Segments;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;

namespace IfcConverter
{
    public class IfcConverter
    {
        public void Import(string filePath)
        {
            IFCProject ifcProject = IFCProject.OpenProject(filePath);
            IModel model = ifcProject.GetModel();
            IfcPipeSegment[] pipeSegments = model.Instances.OfType<IfcPipeSegment>().ToArray();
            IfcPipeSegmentEntity[] pipeSegmentEntities = new IfcPipeSegmentEntity[pipeSegments.Length];
            foreach (IfcPipeSegment ifcPipeSegment in pipeSegments)
            {
                IfcPipeSegmentEntity.CreateFromIfc(ifcPipeSegment, null);
            }
        }
    }
}