using IFC_Converter.Math;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC_Converter.IFC.Entities;

public class IfcNodeEntity
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

    public void CreateAndAddNode(IModel model)
    {
        LocalPlacement = CreateLocalPlacement(model);
        
        Port = model.Instances.New<IfcDistributionPort>(p =>
        {
            p.Name = _nodeEntity.GetName();
            p.Description = _nodeEntity.GetDescription();
            p.ObjectPlacement = LocalPlacement;
            p.FlowDirection = IfcFlowDirectionEnum.NOTDEFINED;
            p.PredefinedType = IfcDistributionPortTypeEnum.PIPE;
        });

        IfcRelDefinesByProperties properties = model.Instances.New<IfcRelDefinesByProperties>(rel =>
        {
            rel.RelatedObjects.Add(Port);
            rel.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Node properties";
                foreach (var kvp in _nodeEntity.GetData())
                {
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(prop =>
                    {
                        prop.Name = kvp.Key;
                        prop.NominalValue = new IfcText(kvp.Value);
                    }));
                }
            });
        });
    }

    public IfcLocalPlacement CreateLocalPlacement(IModel model)
    {
        IfcLocalPlacement? localStartPlacement = model.Instances.New<IfcLocalPlacement>(lp =>
        {
            lp.RelativePlacement = model.Instances.New<IfcAxis2Placement3D>(pos =>
            {
                pos.Location = model.Instances.New<IfcCartesianPoint>(pt => pt.SetXYZ(
                    Coordinates.x, Coordinates.y, Coordinates.z
                ));
            });
        });

        return localStartPlacement;
    }
}