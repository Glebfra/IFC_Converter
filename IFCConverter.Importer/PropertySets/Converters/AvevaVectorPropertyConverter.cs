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
    internal sealed class AvevaVectorPropertyConverter : AbstractPropertyConverter<ProxyItemSet<IfcValue, IIfcValue>, Vector<double>>
    {
        [Pure]
        public override Vector<double> ReadTyped(ProxyItemSet<IfcValue, IIfcValue> source)
        {
            double[] values = source
                .Select(measure => Convert.ToDouble(measure.Value))
                .ToArray();

            return new DenseVector(values);
        }
    }
}