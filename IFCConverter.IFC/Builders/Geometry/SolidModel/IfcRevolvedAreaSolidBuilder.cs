using System;
using IFCConverter.IFC.Extensions;
using IFCConverter.IFC.Interfaces.Geometry.SolidModel;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Builders.Geometry.SolidModel
{
    public class IfcRevolvedAreaSolidBuilder<T> : IfcSweptAreaSolidBuilder<T>, IIfcRevolvedAreaSolidBuilder<T>
        where T : IIfcRevolvedAreaSolid, IInstantiableEntity
    {
        public IfcRevolvedAreaSolidBuilder(double angle, IIfcProfileDef profileDef) : base(profileDef)
        {
            Angle = angle;
        }

        public IIfcAxis1Placement Axis { get; private set; }
        public IfcPlaneAngleMeasure Angle { get; }

        public IIfcAxis1Placement CreateAxis(IModel model, FixedVector<Dim3> axisPosition, FixedVector<Dim3> axisDirection)
        {
            Axis = IfcVectorExtensions.CreateAxis1Placement(model, axisPosition, axisDirection);
            return Axis;
        }

        public override T CreateSolidModel(IModel model)
        {
            if (Axis == null)
                throw new NullReferenceException(
                    $"{nameof(IfcRevolvedAreaSolidBuilder<T>)}: {nameof(Axis)}. Call {nameof(CreateAxis)} before {nameof(CreateSolidModel)}"
                );

            T solid = base.CreateSolidModel(model);
            solid.Angle = Angle;
            solid.Axis = Axis;
            return solid;
        }
    }
}