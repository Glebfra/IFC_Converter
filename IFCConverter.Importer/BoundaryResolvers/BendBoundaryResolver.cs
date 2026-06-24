using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Entities.Proxies;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;
using MatrixExtensions = Utils.MatrixExtensions;

namespace IFCConverter.Importer.BoundaryResolvers
{
    internal sealed class BendBoundaryResolver : IBoundaryResolver
    {
        public IEnumerable<Vector<double>> ResolveBoundary(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies)
        {
            if (proxy is not BendProxy bendProxy)
                throw new InvalidCastException();

            Vector<double> axis = (bendProxy.Position - bendProxy.AxisPosition).Normalize(2);
            Vector<double> upDirection = axis.CrossProduct(bendProxy.RefDirection).Normalize(2);

            Matrix<double>[] rotationMatrices =
            {
                MatrixExtensions.CreateRotationAroundVector(upDirection, bendProxy.Angle / 2).GetRotation(),
                MatrixExtensions.CreateRotationAroundVector(upDirection, -bendProxy.Angle / 2).GetRotation()
            };

            return rotationMatrices.Select(matrix => matrix.Multiply(axis * bendProxy.Radius) + bendProxy.AxisPosition).ToArray();
        }
    }
}