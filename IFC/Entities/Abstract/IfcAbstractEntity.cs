using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractEntity
    {
        protected abstract IfcIdentifier Tag { get; set; }
        public abstract XbimMatrix3D ObjectMatrix3D { get; protected set; }

        public abstract IfcProduct CreateAndAdd(IModel model);

        protected static void CreateObjectPlacement(
            IModel model,
            XbimMatrix3D ObjectMatrix3D,
            out IfcCartesianPoint point,
            out IfcDirection forwardDirection,
            out IfcDirection rightDirection,
            out IfcAxis2Placement3D axis2Placement3D,
            out IfcLocalPlacement localPlacement
        )
        {
            point = IfcAxis.CreatePoint(model, ObjectMatrix3D.Translation);
            forwardDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Forward);
            rightDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Right);
            axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point, forwardDirection, rightDirection);
            localPlacement = IfcAxis.CreateLocalPlacement(model, axis2Placement3D);
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
                    set.Name = "DEBUG";
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
}

