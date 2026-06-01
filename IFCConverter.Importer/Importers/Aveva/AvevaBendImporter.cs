using System;
using System.Collections.Generic;
using System.Linq;
using Ifc.Extensions;
using IFCConverter.Importer.Entities.Proxies;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.PropertySets.Aveva;
using MathNet.Numerics.LinearAlgebra;
using Utils;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;

namespace IFCConverter.Importer.Importers.Aveva
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
            
            if (revolvedAreaSolid.SweptArea is not IIfcCircleProfileDef circleProfileDef)
                throw new Exception("The representation item is not a circle profile def.");

            double diameter = circleProfileDef.Radius * 2;

            Matrix<double> areaMatrix = revolvedAreaSolid.Position.ToMatrix();
            Vector<double> axisLocalPosition = revolvedAreaSolid.Axis.Location.ToVector();
            Vector<double> axisGlobalPosition = areaMatrix.GetRotation().LeftMultiply(axisLocalPosition) +
                                                areaMatrix.GetOffset();
            double radius = axisLocalPosition.L2Norm();

            IEnumerable<IPropertySet> propertySets = source.GetPropertySets();
            AvevaPset? avevaPset = propertySets.OfType<AvevaPset>().FirstOrDefault();
            if (avevaPset == null)
                throw new Exception("The required Aveva property set is missing.");

            Vector<double> bendPosition = avevaPset.Pos;
            Matrix<double> bendMatrix = avevaPset.Ori;

            double lengthPower = GetLengthPower(source);

            return new BendProxy
            (
                bendPosition * lengthPower,
                revolvedAreaSolid.Angle,
                radius * lengthPower,
                axisGlobalPosition * lengthPower,
                areaMatrix.GetY(),
                diameter * lengthPower
            )
            {
                Name = source.Name
            };
        }
    }
}