using System;
using System.Collections.Generic;
using System.Linq;
using Ifc.Extensions;
using IFCConverter.Converters.Importers.Proxies;
using MathNet.Numerics.LinearAlgebra;
using Utils;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;

namespace IFCConverter.Converters.Importers.Aveva
{
    internal class AvevaPipeSegmentImporter : AbstractEntityImporter<IfcBuildingElementProxy, PipeSegmentProxy>
    {
        public override PipeSegmentProxy ReadTyped(IfcBuildingElementProxy source)
        {
            IIfcRepresentationItem[] representationItems = GetRepresentationItems(source).ToArray();
            if (representationItems.Length != 1)
                throw new Exception("Expected exactly one representation item for the given source.");

            IIfcExtrudedAreaSolid? extrudedAreaSolid = representationItems[0] as IfcExtrudedAreaSolid;
            if (extrudedAreaSolid == null)
                throw new Exception("The representation item is not an extruded area solid.");

            Matrix<double> position = extrudedAreaSolid.Position.ToMatrix();
            Matrix<double> rotation = position.GetRotation();
            Vector<double> extrudedDirection = extrudedAreaSolid.ExtrudedDirection.ToVector();
            Vector<double> pipeDirection = rotation.LeftMultiply(extrudedDirection);

            IIfcCircleProfileDef? profileDef = extrudedAreaSolid.SweptArea as IIfcCircleProfileDef;
            if (profileDef == null)
                throw new Exception("The swept area is not a circle profile definition.");

            double length = extrudedAreaSolid.Depth;
            double diameter = profileDef.Radius * 2;

            PipeSegmentProxy pipeSegmentProperties = new PipeSegmentProxy
            (
                diameter: diameter * GetLengthPower(source),
                length: length * GetLengthPower(source),
                position: position.GetOffset() * GetLengthPower(source),
                direction: pipeDirection
            );

            pipeSegmentProperties.Name = source.Name;

            return pipeSegmentProperties;
        }
    }
}