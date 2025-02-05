using IFC_Converter.IFC.Tools;
using IFC_Converter.Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.PropertyResource;

namespace IFC_Converter.IFC.Entities.Abstract;

public abstract class IfcAbstractReducerEntity : IfcAbstractEntity
{
    protected abstract IfcPipeFitting? _pipeFitting { get; set; }
    
    protected readonly StartAbstractReducerEntity _startReducer;
    protected readonly IfcPipeEntity[] _pipeEntities;
    protected readonly IfcNodeEntity _nodeEntity;

    public XbimMatrix3D ObjectMatrix3D { get; }
    public double Length { get; }

    protected IfcAbstractReducerEntity(StartAbstractReducerEntity startReducer, IfcNodeEntity nodeEntity, IfcPipeEntity[] pipeEntities)
    {
        _startReducer = startReducer;
        _nodeEntity = nodeEntity;
        _pipeEntities = pipeEntities;
        _nodeEntity.connEntities.Add(this);
        
        XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
        XbimVector3D directionToPipe = IfcAxis.GetDirectionToPipe(pipeEntities[1], coordinates);
        
        XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
        XbimVector3D forward = directionToPipe.Normalized();
        if (forward == WorldUp || forward == -1 * WorldUp)
            WorldUp = new XbimVector3D(0, 1, 0);
        XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp).Normalized();

        ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, WorldUp);
        Length = _startReducer.GetLengthOfConicalPart();
    }
    
    protected IfcRelConnectsPorts ConnectPorts(IModel model)
    {
        var closestPorts = (
            from port in _pipeEntities.SelectMany(pipe => pipe.Ports)
            let distance = (port.ObjectPlacement.ToMatrix3D().Translation - ObjectMatrix3D.Translation).Length
            orderby distance
            select port
        ).Take(2).ToArray();

        return model.Instances.New<IfcRelConnectsPorts>(ports =>
        {
            ports.Name = $"{closestPorts[0].GlobalId}|{closestPorts[1].GlobalId}";
            ports.Description = "Flow";
            ports.RelatingPort = closestPorts[0];
            ports.RelatedPort = closestPorts[1];
            ports.RealizingElement = _pipeFitting;
        });
    }
    
    protected void AddProperties(IModel model)
    {
        if (_pipeFitting == null)
            throw new Exception("The required field is null. First call the CreateAndAdd method");
        
        #region Pset_PipeFittingTypeStart
        
        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(_pipeFitting);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeFittingTypeStart";
                foreach (var kvp in _startReducer.GetData())
                {
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = kvp.Key;
                        value.NominalValue = new IfcText(kvp.Value);
                    }));
                }
            });
        });

        #endregion
        
        #region DEBUG
        
        #if DEBUG
        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(_pipeFitting);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Debug Properties";
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Coordinates";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Translation.ToString());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Forward direction";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Forward.ToString());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Right direction";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Right.ToString());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Up direction";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Up.ToString());
                }));
            });
        });
        #endif

        #endregion
    }
}