using System;
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

namespace Ifc.Geometries
{
    /// <summary>
    /// Represents the supported beam cross-section geometry types.
    /// </summary>
    public enum BendGeometryType
    {
        /// <summary>
        /// I-shaped beam profile.
        /// </summary>
        IBEAM,
        
        /// <summary>
        /// U-shaped (channel) beam profile.
        /// </summary>
        CHANNEL,
        
        /// <summary>
        /// T-shaped beam profile.
        /// </summary>
        TBEAM,
        
        /// <summary>
        /// L-shaped (angle) beam profile.
        /// </summary>
        CORNERBEAM,
        
        /// <summary>
        /// Rectangular beam profile.
        /// </summary>
        RECTANGULARBEAM,
        
        /// <summary>
        /// Circular beam profile.
        /// </summary>
        CIRCLEBEAM
    }
    
    /// <summary>
    /// Contains the geometric properties required to construct a beam geometry.
    /// </summary>
    public struct BeamGeometryProperties
    {
        public double Height;
        public double Width;
        public double Length;
        public double Diameter;
        public Vector<double> Position;
        public Vector<double> Direction;
        public Vector<double> RefDirection;
        public BendGeometryType GeometryType;
    }
    
    /// <summary>
    /// Represents a BRep geometry for beam elements.
    /// </summary>
    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Brep)]
    public class BeamGeometry : IfcGeometry
    {
        private const double HeightToBeamFactor2 = 1.0;
        private const double HeightToBeamFactor1 = 0.8;
        private const double HeightToCenterFactor = 0.7;
        
        private const double WidthToBeamFactor2 = 1.0;
        private const double WidthToBeamFactor1 = 0.7;
        private const double WidthToCenterFactor = 0.3;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BeamGeometry"/> class.
        /// </summary>
        /// <param name="geometryBuilder">The geometry builder.</param>
        /// <param name="representationContext">The optional IFC representation context.</param>
        public BeamGeometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext? representationContext = null) 
            : base(geometryBuilder, representationContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BeamGeometry"/> class.
        /// </summary>
        /// <param name="geometryBuilders">The collection of geometry builders.</param>
        /// <param name="representationContext">The optional IFC representation context.</param>
        public BeamGeometry(IEnumerable<IIfcBuilder> geometryBuilders, IIfcRepresentationContext? representationContext = null) 
            : base(geometryBuilders, representationContext)
        {
        }

        /// <summary>
        /// Creates a beam geometry from the specified properties.
        /// </summary>
        /// <param name="model">The IFC model.</param>
        /// <param name="properties">The beam geometry properties.</param>
        /// <returns>A new <see cref="BeamGeometry"/> instance.</returns>
        [Pure]
        public static BeamGeometry CreateGeometry(IModel model, BeamGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();
            
            Vector<double> z = properties.Direction;
            Vector<double> x = properties.RefDirection;
            Vector<double> y = z.CrossProduct(x).Normalize(2);
            
            Vector<double> xHeight = x * properties.Height / 2;
            Vector<double> yWidth = y * properties.Width / 2;

            Vector<double>[] startPoints = GenerateStartPoints(properties, xHeight, yWidth);
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

        /// <summary>
        /// Generates the cross-section points for the specified beam type.
        /// </summary>
        /// <param name="properties">The beam properties.</param>
        /// <param name="xHeight">The local X-axis scaled by half the beam height.</param>
        /// <param name="yWidth">The local Y-axis scaled by half the beam width.</param>
        /// <returns>An array of cross-section points.</returns>
        [Pure]
        private static Vector<double>[] GenerateStartPoints(BeamGeometryProperties properties, Vector<double> xHeight, Vector<double> yWidth)
        {
            return properties.GeometryType switch
            {
                BendGeometryType.IBEAM => GenerateIBeamStartPoints(properties, xHeight, yWidth),
                BendGeometryType.CHANNEL => GenerateChannelBeamStartPoints(properties, xHeight, yWidth),
                BendGeometryType.TBEAM => GenerateTBeamStartPoints(properties, xHeight, yWidth),
                BendGeometryType.CORNERBEAM => GenerateCornerBeamStartPoints(properties, xHeight, yWidth),
                BendGeometryType.CIRCLEBEAM => GenerateCircleBeamStartPoints(properties, xHeight, yWidth),
                BendGeometryType.RECTANGULARBEAM => GenerateRectangularBeamStartPoints(properties, xHeight, yWidth),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Generates the cross-section points for an I-beam profile.
        /// </summary>
        /// <param name="properties">The beam properties.</param>
        /// <param name="xHeight">The local X-axis scaled by half the beam height.</param>
        /// <param name="yWidth">The local Y-axis scaled by half the beam width.</param>
        /// <returns>An array of cross-section points.</returns>
        [Pure]
        private static Vector<double>[] GenerateIBeamStartPoints(BeamGeometryProperties properties, Vector<double> xHeight, Vector<double> yWidth)
        {
            return new Vector<double>[]
            {
                properties.Position - xHeight * HeightToCenterFactor - yWidth * WidthToCenterFactor,
                properties.Position - xHeight * HeightToBeamFactor1 - yWidth * WidthToBeamFactor2,
                properties.Position - xHeight * HeightToBeamFactor2 - yWidth * WidthToBeamFactor2,
                properties.Position - xHeight * HeightToBeamFactor2 + yWidth * WidthToBeamFactor2,
                properties.Position - xHeight * HeightToBeamFactor1 + yWidth * WidthToBeamFactor2,
                properties.Position - xHeight * HeightToCenterFactor + yWidth * WidthToCenterFactor,
                properties.Position + xHeight * HeightToCenterFactor + yWidth * WidthToCenterFactor,
                properties.Position + xHeight * HeightToBeamFactor1 + yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToBeamFactor2 + yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToBeamFactor2 - yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToBeamFactor1 - yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToCenterFactor - yWidth * WidthToCenterFactor
            };
        }

        /// <summary>
        /// Generates the cross-section points for a channel beam profile.
        /// </summary>
        /// <param name="properties">The beam properties.</param>
        /// <param name="xHeight">The local X-axis scaled by half the beam height.</param>
        /// <param name="yWidth">The local Y-axis scaled by half the beam width.</param>
        /// <returns>An array of cross-section points.</returns>
        [Pure]
        private static Vector<double>[] GenerateChannelBeamStartPoints(BeamGeometryProperties properties, Vector<double> xHeight, Vector<double> yWidth)
        {
            return new Vector<double>[]
            {
                properties.Position - xHeight * HeightToCenterFactor + yWidth * WidthToCenterFactor,
                properties.Position - xHeight * HeightToBeamFactor1 + yWidth * WidthToBeamFactor2,
                properties.Position - xHeight * HeightToBeamFactor2 + yWidth * WidthToBeamFactor2,
                properties.Position - xHeight * HeightToBeamFactor2 - yWidth * WidthToCenterFactor,
                properties.Position + xHeight * HeightToBeamFactor2 - yWidth * WidthToCenterFactor,
                properties.Position + xHeight * HeightToBeamFactor2 + yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToBeamFactor1 + yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToCenterFactor + yWidth * WidthToCenterFactor
            };
        }

        /// <summary>
        /// Generates the cross-section points for a T-beam profile.
        /// </summary>
        /// <param name="properties">The beam properties.</param>
        /// <param name="xHeight">The local X-axis scaled by half the beam height.</param>
        /// <param name="yWidth">The local Y-axis scaled by half the beam width.</param>
        /// <returns>An array of cross-section points.</returns>
        [Pure]
        private static Vector<double>[] GenerateTBeamStartPoints(BeamGeometryProperties properties, Vector<double> xHeight, Vector<double> yWidth)
        {
            return new Vector<double>[]
            {
                properties.Position + xHeight * HeightToCenterFactor + yWidth * WidthToCenterFactor,
                properties.Position + xHeight * HeightToBeamFactor1 + yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToBeamFactor2 + yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToBeamFactor2 - yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToBeamFactor1 - yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToCenterFactor - yWidth * WidthToCenterFactor,
                properties.Position - xHeight * HeightToCenterFactor - yWidth * WidthToCenterFactor,
                properties.Position - xHeight * HeightToCenterFactor + yWidth * WidthToCenterFactor
            };
        }

        /// <summary>
        /// Generates the cross-section points for an angle (L-shaped) beam profile.
        /// </summary>
        /// <param name="properties">The beam properties.</param>
        /// <param name="xHeight">The local X-axis scaled by half the beam height.</param>
        /// <param name="yWidth">The local Y-axis scaled by half the beam width.</param>
        /// <returns>An array of cross-section points.</returns>
        [Pure]
        private static Vector<double>[] GenerateCornerBeamStartPoints(BeamGeometryProperties properties, Vector<double> xHeight, Vector<double> yWidth)
        {
            return new Vector<double>[]
            {
                properties.Position - xHeight * HeightToBeamFactor2 - yWidth * WidthToBeamFactor1,
                properties.Position - xHeight * HeightToBeamFactor2 - yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToBeamFactor2 - yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToBeamFactor2 + yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToBeamFactor1 + yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToBeamFactor1 - yWidth * WidthToBeamFactor1,
            };
        }

        /// <summary>
        /// Generates the cross-section points for a circular beam profile.
        /// </summary>
        /// <param name="properties">The beam properties.</param>
        /// <param name="xHeight">The local X-axis scaled by half the beam height.</param>
        /// <param name="yWidth">The local Y-axis scaled by half the beam width.</param>
        /// <returns>An array of cross-section points approximating a circle.</returns>
        [Pure]
        private static Vector<double>[] GenerateCircleBeamStartPoints(BeamGeometryProperties properties, Vector<double> xHeight, Vector<double> yWidth)
        {
            const int numPoints = 16;
            Vector<double>[] points = new Vector<double>[numPoints];
            double radius = properties.Diameter / 2;

            Vector<double> xHeightNorm = xHeight.Normalize(2);
            Vector<double> yWidthNorm = yWidth.Normalize(2);
            
            for (int i = 0; i < numPoints; i++)
            {
                double angle = 2 * Math.PI * i / numPoints;
                points[i] = properties.Position + radius * Math.Cos(angle) * xHeightNorm + radius * Math.Sin(angle) * yWidthNorm;
            }

            return points;
        }

        /// <summary>
        /// Generates the cross-section points for a rectangular beam profile.
        /// </summary>
        /// <param name="properties">The beam properties.</param>
        /// <param name="xHeight">The local X-axis scaled by half the beam height.</param>
        /// <param name="yWidth">The local Y-axis scaled by half the beam width.</param>
        /// <returns>An array of cross-section corner points.</returns>
        [Pure]
        private static Vector<double>[] GenerateRectangularBeamStartPoints(BeamGeometryProperties properties, Vector<double> xHeight, Vector<double> yWidth)
        {
            return new Vector<double>[]
            {
                properties.Position - xHeight * HeightToBeamFactor2 - yWidth * WidthToBeamFactor2,
                properties.Position - xHeight * HeightToBeamFactor2 + yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToBeamFactor2 + yWidth * WidthToBeamFactor2,
                properties.Position + xHeight * HeightToBeamFactor2 - yWidth * WidthToBeamFactor2
            };
        }
    }
}