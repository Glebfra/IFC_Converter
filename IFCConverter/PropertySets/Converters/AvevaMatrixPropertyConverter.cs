using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Xbim.Common.Collections;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.PropertySets.Converters
{
    internal class AvevaMatrixPropertyConverter : 
        AbstractPropertyConverter<ProxyItemSet<IfcValue, IIfcValue>, Matrix<double>>
    {
        public override Matrix<double> ReadTyped(ProxyItemSet<IfcValue, IIfcValue> source)
        {
            double[] values = source
                .Select(measure => Convert.ToDouble(measure.Value))
                .ToArray();

            return new DenseMatrix(3, 3, values);
        }
    }
}