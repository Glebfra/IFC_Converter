using IFC.Entities.Abstract.Anchors;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Anchors
{
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
}