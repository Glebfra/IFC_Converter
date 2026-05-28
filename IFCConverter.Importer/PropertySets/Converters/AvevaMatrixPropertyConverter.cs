using System;
using System.Diagnostics.Contracts;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Xbim.Common.Collections;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.Importer.PropertySets.Converters
{
    internal class AvevaMatrixPropertyConverter : 
        AbstractPropertyConverter<ProxyItemSet<IfcValue, IIfcValue>, Matrix<double>>
    {
        [Pure]
        public override Matrix<double> ReadTyped(ProxyItemSet<IfcValue, IIfcValue> source)
        {
            double[] values = source
                .Select(measure => Convert.ToDouble(measure.Value))
                .ToArray();

            return new DenseMatrix(3, 3, values);
        }
    }
}