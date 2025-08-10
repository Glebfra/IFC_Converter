using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Anchors
{
    #if NEW

    public class IfcFixedAnchorEntity : IfcAbstractFixedSupportEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> XDim { get; }
        public override ActionProperty<double> YDim { get; }
        
        public IfcFixedAnchorEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix, double xDim, double yDim) 
            : base(objectMatrix)
        {
            Name = name;
            Tag = tag;
            XDim = xDim;
            YDim = yDim;
        }
    }
    
    #else
    
    public sealed class IfcFixedAnchorEntity : IfcAbstractFixedSupportEntity
    {
        public override double XDim { get; protected set; }
        public override double YDim { get; protected set; }

        public IfcFixedAnchorEntity(StartAnchorEntity anchorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities)
            : base(anchorEntity, nodeEntity, abstractSegmentEntities)
        {
            XDim = abstractSegmentEntities[0].Diameter * 2;
            YDim = XDim;
        }
    }

    #endif
}