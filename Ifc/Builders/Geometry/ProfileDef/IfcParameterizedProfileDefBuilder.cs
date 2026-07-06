using System;
using Ifc.Extensions;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Builders.Geometry.ProfileDef
{
    public class IfcParameterizedProfileDefBuilder<T> : IfcProfileDefBuilder<T>, IIfcParameterizedProfileDefBuilder<T>
        where T : IIfcParameterizedProfileDef, IInstantiableEntity
    {
        public IfcParameterizedProfileDefBuilder(IfcProfileTypeEnum profileTypeEnum, IfcLabel profileName) : base(
            profileTypeEnum, profileName)
        {
        }

        public IIfcAxis2Placement2D? Position { get; private set; }

        public IIfcAxis2Placement2D CreatePosition(IModel model, Matrix<double> matrix)
        {
            Position = matrix.ToAxis2Placement2D(model);
            return Position;
        }

        public override T CreateProfileDef(IModel model)
        {
            if (Position == null)
                throw new NullReferenceException(
                    $"{nameof(IfcParameterizedProfileDefBuilder<T>)}: {nameof(Position)}. Call {nameof(CreatePosition)} before {nameof(CreateProfileDef)}");

            T profileDef = base.CreateProfileDef(model);
            profileDef.Position = Position;
            return profileDef;
        }
    }
}