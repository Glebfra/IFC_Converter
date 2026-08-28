using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.PropertySets.Aveva;
using IFCConverter.Importer.Proxies;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Ifc4.SharedBldgElements;

namespace IFCConverter.Importer.Importers.Aveva
{
    internal sealed class AvevaPcomImporter : AbstractEntityImporter<IfcBuildingElementProxy, PcomProxy>
    {
        public override PcomProxy ReadTyped(IfcBuildingElementProxy source)
        {
            IEnumerable<IPropertySet> propertySets = source.GetPropertySets();
            AvevaPset avevaPset = propertySets.OfType<AvevaPset>().FirstOrDefault();
            if (avevaPset == null)
                throw new Exception("The required Aveva property set is missing.");

            Vector<double> pcomPosition = avevaPset.Pos;
            double lengthPower = GetLengthPower(source);

            return new PcomProxy
            (
                pcomPosition * lengthPower
            )
            {
                Name = source.Name
            };
        }
    }
}