using System;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Utils.Mathematics;
using Xbim.Common.Collections;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.Importer.PropertySets.Converters
{
    internal sealed class AvevaMatrixPropertyConverter : AbstractPropertyConverter<ProxyItemSet<IfcValue, IIfcValue>, FixedMatrix<Dim3>>
    {
        [Pure]
        public override FixedMatrix<Dim3> ReadTyped(ProxyItemSet<IfcValue, IIfcValue> source)
        {
            double[] values = source
                .Select(measure => Convert.ToDouble(measure.Value))
                .ToArray();

            return FixedMatrix<Dim3>.Builder.Dense(values);
        }
    }
}