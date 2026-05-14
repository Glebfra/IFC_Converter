using System;
using System.Collections.Generic;
using System.Linq;
using Ifc.Extensions;
using IFCConverter.Converters.Importers.Proxies;
using IFCConverter.Extensions;
using IFCConverter.Interfaces;
using IFCConverter.PropertySets.Aveva;
using MathNet.Numerics.LinearAlgebra;
using Utils;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;

namespace IFCConverter.Converters.Importers.Aveva
{
    internal class AvevaBendImporter : AbstractEntityImporter<IfcBuildingElementProxy, BendProxy>
    {
        public override BendProxy ReadTyped(IfcBuildingElementProxy source)
        {
            IIfcRepresentationItem[] representationItems = GetRepresentationItems(source).ToArray();
            if (representationItems.Length != 1)
                throw new Exception("Expected exactly one representation item for the given source.");

            if (representationItems[0] is not IIfcRevolvedAreaSolid revolvedAreaSolid)
                throw new Exception("The representation item is not a revolved area solid.");

            Matrix<double> areaMatrix = revolvedAreaSolid.Position.ToMatrix();
            Vector<double> axisPosition = revolvedAreaSolid.Axis.Location.ToVector();
            double radius = axisPosition.L2Norm();

            IEnumerable<IPropertySet> propertySets = source.GetPropertySets();
            AvevaPset? avevaPset = propertySets.OfType<AvevaPset>().FirstOrDefault();
            if (avevaPset == null)
                throw new Exception("The required Aveva property set is missing.");

            Vector<double> bendPosition = avevaPset.Pos;
            Matrix<double> bendMatrix = avevaPset.Ori;

            double lengthPower = GetLengthPower(source);

            return new BendProxy
            (
                position: bendPosition * lengthPower,
                angle: revolvedAreaSolid.Angle,
                radius: radius * lengthPower,
                axisPosition: (areaMatrix.GetOffset() + axisPosition) * lengthPower,
                refDirection: areaMatrix.GetRotation().Multiply(axisPosition).Normalize(2)
            )
            {
                Name = source.Name
            };
        }
    }
}