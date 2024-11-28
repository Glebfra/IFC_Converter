using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC_Converter.IFC.Entities;

public class IfcNodeEntity : IfcAbstractEntity
{
    private StartNodeEntity _nodeEntity;

    public XbimVector3D Coordinates { get; private set; }
    public IfcLocalPlacement LocalPlacement { get; private set; }
    public IfcDistributionPort Port { get; private set; }

    public IfcNodeEntity(StartNodeEntity nodeEntity)
    {
        _nodeEntity = nodeEntity;
        Coordinates = _nodeEntity.GetCoordinates();
    }

    public override IfcObject CreateAndAdd(IModel model)
    {
        LocalPlacement = CreateLocalPlacement(model, Coordinates);
        Port = CreatePort(model, _nodeEntity.GetName(), _nodeEntity.GetDescription(), LocalPlacement);
        AddProperties(model, Port, _nodeEntity);

        return Port;
    }
}