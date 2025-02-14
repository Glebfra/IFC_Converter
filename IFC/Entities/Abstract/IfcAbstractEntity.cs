using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.Entities.Abstract;

public abstract class IfcAbstractEntity
{
    protected abstract IfcIdentifier Tag { get; set; }
    public abstract XbimMatrix3D ObjectMatrix3D { get; protected set; }

    protected IfcCartesianPoint? _point;
    protected IfcDirection? _forwardDirection;
    protected IfcDirection? _rightDirection;
    protected IfcAxis2Placement3D? _axis2Placement3D;
    protected IfcLocalPlacement? _localPlacement;

    public virtual IfcProduct? CreateAndAdd(IModel model)
    {
        _point = IfcAxis.CreatePoint(model, ObjectMatrix3D.Translation);
        _forwardDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Forward);
        _rightDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Right);
        _axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, _point, _forwardDirection, _rightDirection);
        _localPlacement = IfcAxis.CreateLocalPlacement(model, _axis2Placement3D);

        return null;
    }

    protected virtual void AddProperties(IModel model, IfcProduct product)
    {
        #region DEBUG

        #if DEBUG
        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(product);
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

