using IFC_Converter.Math;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC_Converter.IFC.Entities;

public class IfcNodeEntity : IfcAbstractEntity
{
    private StartNodeEntity _nodeEntity;

    public Vector3 Coordinates { get; private set; }
    public IfcLocalPlacement LocalPlacement { get; private set; }
    public IfcDistributionPort Port { get; private set; }

    public IfcNodeEntity(StartNodeEntity nodeEntity)
    {
        _nodeEntity = nodeEntity;
        Coordinates = _nodeEntity.GetCoordinates();
    }

    public override void CreateAndAdd(IModel model)
    {
        LocalPlacement = CreateLocalPlacement(model, Coordinates);

        Port = model.Instances.New<IfcDistributionPort>(p =>
        {
            p.Name = _nodeEntity.GetName();
            p.Description = _nodeEntity.GetDescription();
            p.ObjectPlacement = LocalPlacement;
            p.FlowDirection = IfcFlowDirectionEnum.NOTDEFINED;
            p.PredefinedType = IfcDistributionPortTypeEnum.PIPE;
        });

        AddProperties(model, Port, _nodeEntity);
    }
}