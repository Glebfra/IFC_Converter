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
    internal class AvevaTeeImporter : AbstractEntityImporter<IfcBuildingElementProxy, TeeProxy>
    {
        private const double _vectorTolerance = 1e-3;
        private readonly VectorComparer _vectorComparer = new(_vectorTolerance);

        public override TeeProxy ReadTyped(IfcBuildingElementProxy source)
        {
            IIfcRepresentationItem[] representationItems = GetRepresentationItems(source).ToArray();
            if (representationItems.Length != 2)
                throw new Exception("Expected exactly two representation items for the given source.");

            IEnumerable<IPropertySet> propertySets = source.GetPropertySets();
            AvevaPset? avevaPset = propertySets.OfType<AvevaPset>().FirstOrDefault();
            if (avevaPset == null)
                throw new Exception("The required Aveva property set is missing.");

            Vector<double> mainProjection = default!, headProjection = default!;
            double mainDiameter = default, headDiameter = default;
            Vector<double> teePosition = avevaPset.Pos;

            IIfcExtrudedAreaSolid[] extrudedAreaSolids = representationItems.Cast<IIfcExtrudedAreaSolid>().ToArray();
            foreach (IIfcExtrudedAreaSolid extrudedAreaSolid in extrudedAreaSolids)
            {
                if (extrudedAreaSolid.SweptArea is not IIfcCircleProfileDef profileDef)
                    throw new Exception("The swept area is not a circle profile definition.");

                double teeBranchDiameter = profileDef.Radius * 2;

                Matrix<double> matrix = extrudedAreaSolid.Position.ToMatrix();
                Matrix<double> rotation = matrix.GetRotation();

                Vector<double> extrudedDir = extrudedAreaSolid.ExtrudedDirection.ToVector();
                Vector<double> teeBranchDir = rotation.LeftMultiply(extrudedDir).Normalize(2);
                double teeBranchLength = extrudedAreaSolid.Depth;

                Vector<double> startPos = matrix.GetOffset();
                Vector<double> projection = teeBranchDir * teeBranchLength;
                Vector<double> endPos = startPos + projection;

                if (_vectorComparer.Equals(startPos, teePosition))
                {
                    headProjection = projection;
                    headDiameter = teeBranchDiameter;
                }
                else if (_vectorComparer.Equals(endPos, teePosition))
                {
                    headProjection = -projection;
                    headDiameter = teeBranchDiameter;
                }
                else
                {
                    mainProjection = projection;
                    mainDiameter = teeBranchDiameter;
                }
            }

            double lengthPower = GetLengthPower(source);

            return new TeeProxy(
                teePosition * lengthPower,
                mainProjection * lengthPower,
                headProjection * lengthPower,
                headDiameter: headDiameter * lengthPower,
                mainDiameter: mainDiameter * lengthPower
            )
            {
                Name = source.Name
            };
        }
    }
}