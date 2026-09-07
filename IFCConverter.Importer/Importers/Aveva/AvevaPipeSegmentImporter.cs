using System;
using System.Linq;
using IFCConverter.IFC.Extensions;
using IFCConverter.Importer.Proxies;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;

namespace IFCConverter.Importer.Importers.Aveva
{
    internal class AvevaPipeSegmentImporter : AbstractEntityImporter<IfcBuildingElementProxy, PipeSegmentProxy>
    {
        public override PipeSegmentProxy ReadTyped(IfcBuildingElementProxy source)
        {
            IIfcRepresentationItem[] representationItems = GetRepresentationItems(source).ToArray();
            if (representationItems.Length != 1)
                throw new Exception("Expected exactly one representation item for the given source.");

            if (!(representationItems[0] is IIfcExtrudedAreaSolid extrudedAreaSolid))
                throw new Exception("The representation item is not an extruded area solid.");

            Matrix<double> position = extrudedAreaSolid.Position.ToMatrix();
            Matrix<double> rotation = position.GetRotation();
            Vector<double> extrudedDirection = extrudedAreaSolid.ExtrudedDirection.ToVector();
            Vector<double> pipeDirection = rotation.LeftMultiply(extrudedDirection);

            IIfcCircleProfileDef profileDef = extrudedAreaSolid.SweptArea as IIfcCircleProfileDef;
            if (profileDef == null)
                throw new Exception("The swept area is not a circle profile definition.");

            double length = extrudedAreaSolid.Depth;
            double diameter = profileDef.Radius * 2;

            return new PipeSegmentProxy
            (
                diameter * GetLengthPower(source),
                length * GetLengthPower(source),
                position.GetOffset() * GetLengthPower(source),
                pipeDirection
            )
            {
                Name = source.Name
            };
        }
    }
}