using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities;
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

            List<IfcNodeEntity> nodeEntities = new List<IfcNodeEntity>();
            IfcPipeSegment[] pipeSegments = model.Instances.OfType<IfcPipeSegment>().ToArray();
            IfcPipeSegmentEntity[] pipeSegmentEntities = new IfcPipeSegmentEntity[pipeSegments.Length];
            for (int i = 0; i < pipeSegments.Length; i++)
            {
                pipeSegmentEntities[i] = IfcPipeSegmentEntity.CreateFromIfc(pipeSegments[i]);
            }
        }
    }
}