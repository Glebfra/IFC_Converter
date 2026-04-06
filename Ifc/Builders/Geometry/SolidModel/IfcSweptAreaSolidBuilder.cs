using System;
using Ifc.Extensions;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Builders.Geometry.SolidModel
{
    public class IfcSweptAreaSolidBuilder<T> : IfcSolidModelBuilder<T>, IIfcSweptAreaSolidBuilder<T>
        where T : IIfcSweptAreaSolid, IInstantiableEntity
    {
        public IfcSweptAreaSolidBuilder(IIfcProfileDef profileDef)
        {
            ProfileDef = profileDef;
        }

        public IIfcProfileDef? ProfileDef { get; }
        public IIfcAxis2Placement3D? Position { get; private set; }

        public virtual IIfcAxis2Placement3D CreatePosition(IModel model, Matrix<double> matrix)
        {
            Position = matrix.ToAxis2Placement3D(model);
            return Position;
        }

        public override T CreateSolidModel(IModel model)
        {
            if (Position == null)
                throw new NullReferenceException(
                    $"{nameof(IfcSweptAreaSolidBuilder<T>)}: {nameof(Position)}. Call {nameof(CreatePosition)} before {nameof(CreateSolidModel)}");

            T solid = base.CreateSolidModel(model);
            solid.Position = Position;
            solid.SweptArea = ProfileDef;
            return solid;
        }
    }
}