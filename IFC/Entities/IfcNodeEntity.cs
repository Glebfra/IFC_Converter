using System.Collections.Generic;
using IFC.Entities.Abstract;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC.Entities;

public class IfcNodeEntity : IfcAbstractEntity
{
    protected override IfcIdentifier Tag { get; set; } = "Node";
    
    public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
    
    public readonly List<IfcAbstractEntity> ConnEntities = new List<IfcAbstractEntity>();

    public XbimVector3D Coordinates => ObjectMatrix3D.Translation;
    public IfcDistributionPort Port { get; private set; }
    
    private StartNodeEntity _nodeEntity;

    public IfcNodeEntity(StartNodeEntity nodeEntity)
    {
        _nodeEntity = nodeEntity;
        XbimVector3D coordinates = new XbimVector3D(nodeEntity.GetXCoord(), nodeEntity.GetYCoord(), nodeEntity.GetZCoord());
        ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, new XbimVector3D(1, 0, 0), new XbimVector3D(0, 0, 1));
    }

    public override IfcProduct CreateAndAdd(IModel model)
    {
        return null;
    }
}