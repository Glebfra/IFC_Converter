using IFC_Converter.IFC.Entities.Abstract;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC_Converter.IFC.Entities;

public class IfcNodeEntity : IfcAbstractEntity
{
    public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
    
    public readonly List<IfcAbstractEntity> ConnEntities = new List<IfcAbstractEntity>();

    public XbimVector3D Coordinates => ObjectMatrix3D.Translation;
    public IfcDistributionPort Port { get; private set; }
    
    private StartNodeEntity _nodeEntity;

    public IfcNodeEntity(StartNodeEntity nodeEntity)
    {
        _nodeEntity = nodeEntity;
        ObjectMatrix3D = XbimMatrix3D.CreateWorld(_nodeEntity.GetCoordinates(), new XbimVector3D(1, 0, 0), new XbimVector3D(0, 0, 1));
    }

    public override IfcProduct CreateAndAdd(IModel model)
    {
        AddProperties(model);
        return null;
    }

    private void AddProperties(IModel model)
    {
        #region Pset_PipeFittingStart

        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(Port);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeFittingStart";
                foreach (var kvp in _nodeEntity.GetData())
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
            properties.RelatedObjects.Add(Port);
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