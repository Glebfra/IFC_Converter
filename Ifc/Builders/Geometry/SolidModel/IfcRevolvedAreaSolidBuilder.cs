using System;
using Ifc.Extensions;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Builders.Geometry.SolidModel
{
    public class IfcRevolvedAreaSolidBuilder<T> : IfcSweptAreaSolidBuilder<T>, IIfcRevolvedAreaSolidBuilder<T>
        where T : IIfcRevolvedAreaSolid, IInstantiableEntity
    {
        public IfcRevolvedAreaSolidBuilder(double angle, IIfcProfileDef profileDef) : base(profileDef)
        {
            Angle = angle;
        }

        public IIfcAxis1Placement? Axis { get; private set; }
        public IfcPlaneAngleMeasure Angle { get; }

        public IIfcAxis1Placement CreateAxis(IModel model, Vector<double> axisPosition, Vector<double> axisDirection)
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