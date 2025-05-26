using IFC.Entities.Abstract;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcNonstandardExpansionJointEntity : IfcAbstractNonStandardExpansionJointEntity
    {
        public override double Length { get; protected set; }
        public override double Radius { get; protected set; }
        
        public IfcNonstandardExpansionJointEntity(StartNonstandardExpansionJointEntity nonstandardExpansion, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(nonstandardExpansion, ifcNodeEntity, segmentEntities)
        {
            Length = nonstandardExpansion.Length.SIProperty;
            Radius = segmentEntities[0].Diameter / 2;
        }
    }
}