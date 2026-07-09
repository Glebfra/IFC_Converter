using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Ifc.API;
using Ifc.Attributes;
using Ifc.Builders.Geometry.Tessellated;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using VectorExtensions = Utils.VectorExtensions;

namespace Ifc.Geometries
{
    public struct BeamGeometryProperties
    {
        public double Height;
        public double Width;
        public double Length;
        public Vector<double> Position;
        public Vector<double> Direction;
    }
    
    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Brep)]
    public class BeamGeometry : IfcGeometry
    {
        private const double HeightToBeamFactor2 = 1.0;
        private const double HeightToBeamFactor1 = 0.8;
        private const double HeightToCenterFactor = 0.7;
        
        private const double WidthToBeamFactor2 = 1.0;
        private const double WidthToCenterFactor = 0.3;
        
        public BeamGeometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext? representationContext = null) : base(geometryBuilder, representationContext)
        {
        }

        public BeamGeometry(IEnumerable<IIfcBuilder> geometryBuilders, IIfcRepresentationContext? representationContext = null) : base(geometryBuilders, representationContext)
        {
        }

        [Pure]
        public static BeamGeometry CreateGeometry(IModel model, BeamGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();
            
            Vector<double> z = properties.Direction;
            Vector<double> x = z.CreateNormalVector();
            Vector<double> y = z.CrossProduct(x).Normalize(2);
            
            Vector<double> xWidth = x * properties.Width / 2;
            Vector<double> yHeight = y * properties.Height / 2;
            
            Vector<double>[] startPoints = new Vector<double>[]
            {
                -yHeight * HeightToCenterFactor - xWidth * WidthToCenterFactor,
                -yHeight * HeightToBeamFactor1 - xWidth * WidthToBeamFactor2,
                -yHeight * HeightToBeamFactor2 - xWidth * WidthToBeamFactor2,
                -yHeight * HeightToBeamFactor2 + xWidth * WidthToBeamFactor2,
                -yHeight * HeightToBeamFactor1 + xWidth * WidthToBeamFactor2,
                -yHeight * HeightToCenterFactor + xWidth * WidthToCenterFactor,
                yHeight * HeightToCenterFactor + xWidth * WidthToCenterFactor,
                yHeight * HeightToBeamFactor1 + xWidth * WidthToBeamFactor2,
                yHeight * HeightToBeamFactor2 + xWidth * WidthToBeamFactor2,
                yHeight * HeightToBeamFactor2 - xWidth * WidthToBeamFactor2,
                yHeight * HeightToBeamFactor1 - xWidth * WidthToBeamFactor2,
                yHeight * HeightToCenterFactor - xWidth * WidthToCenterFactor
            };
            Vector<double>[] endPoints = startPoints.Select(startPoint => startPoint + properties.Direction * properties.Length).ToArray();

            ExtrudedBodyTriangulatedGeometryProperties bodyProperties = new ExtrudedBodyTriangulatedGeometryProperties()
            {
                StartPoints = startPoints,
                ExtrudedDirection = properties.Direction,
                Length = properties.Length
            };
            IfcTriangulatedProperties triangulatedProperties = IfcTriangulatedProperties.CreateExtrudedBody(bodyProperties);
            IIfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet> triangulatedFaceSetBuilder = new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
            triangulatedFaceSetBuilder.CreateCoordinates(model, triangulatedProperties.Coordinates);
            triangulatedFaceSetBuilder.AssignTriangleIndices(triangulatedProperties.TriangleIndices);
            triangulatedFaceSetBuilder.AssignNormals(triangulatedProperties.Normals);
            builders.Add(triangulatedFaceSetBuilder);

            return new BeamGeometry(builders);
        }
    }
}