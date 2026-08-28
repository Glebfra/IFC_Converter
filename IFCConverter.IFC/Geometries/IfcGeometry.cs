using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Attributes;
using IFCConverter.IFC.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.PresentationAppearanceResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFCConverter.IFC.Geometries
{
    public class IfcGeometry : IIfcGeometry
    {
        public IfcGeometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext representationContext = null)
        {
            GeometryBuilders = new[]
            {
                geometryBuilder
            };
            RepresentationContext = representationContext;
        }

        public IfcGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
        {
            GeometryBuilders = geometryBuilders;
            RepresentationContext = representationContext;
        }

        public IEnumerable<IIfcBuilder> GeometryBuilders { get; }
        public IIfcRepresentationContext RepresentationContext { get; }
        public IColor Color { get; private set; }

        [Pure]
        public IIfcShapeRepresentation CreateShapeRepresentation(IModel model)
        {
            IfcRepresentationItem[] representationItems = GeometryBuilders
                .Select(builder => builder.Build(model))
                .Cast<IfcRepresentationItem>()
                .ToArray();

            if (Color != null)
                StyleItems(model, (Color)Color, representationItems);

            Type geometryType = GetType();
            IfcRepresentationIdentifierAttribute representationIdentifierAttribute =
                geometryType.GetCustomAttribute<IfcRepresentationIdentifierAttribute>();
            IfcRepresentationTypeAttribute representationTypeAttribute =
                geometryType.GetCustomAttribute<IfcRepresentationTypeAttribute>();

            string representationIdentifier = representationIdentifierAttribute.RepresentationIdentifier.ToString();
            string representationType = representationTypeAttribute.IfcRepresentationType.ToString();

            IfcShapeRepresentation shapeRepresentation = model.Instances.New<IfcShapeRepresentation>(
                representation =>
                {
                    representation.ContextOfItems = RepresentationContext as IfcRepresentationContext ??
                                                    model.Instances.OfType<IfcGeometricRepresentationContext>()
                                                        .FirstOrDefault();
                    representation.RepresentationIdentifier = representationIdentifier;
                    representation.RepresentationType = representationType;
                    representation.Items.AddRange(representationItems);
                });

            return shapeRepresentation;
        }

        [Pure]
        public IIfcProductDefinitionShape CreateProductDefinitionShape(IModel model,
            IIfcShapeRepresentation shapeRepresentation)
        {
            IfcProductDefinitionShape productDefinitionShape = model.Instances.New<IfcProductDefinitionShape>(
                shape => shape.Representations.Add((IfcShapeRepresentation)shapeRepresentation)
            );
            return productDefinitionShape;
        }

        public void AssignColor(IColor color)
        {
            Color = color;
        }

        [Pure]
        protected static int[][] ChangeTriangleIndexation(int[][] triangleIndices)
        {
            int[][] changedTriangleIndices = new int[triangleIndices.Length][];
            for (int i = 0; i < triangleIndices.Length; i++)
            {
                changedTriangleIndices[i] = triangleIndices[i];
                for (int j = 0; j < triangleIndices[i].Length; j++)
                {
                    changedTriangleIndices[i][j]++;
                }
            }

            return changedTriangleIndices;
        }

        private void StyleItems(IModel model, IColor color, IEnumerable<IIfcRepresentationItem> representationItems)
        {
            IIfcColourRgb colourRgb = CreateColourRgb(model, color);
            IIfcSurfaceStyleShading surfaceStyleShading = CreateSurfaceStyleShading(model, colourRgb);
            IIfcSurfaceStyle surfaceStyle = CreateSurfaceStyle(model, surfaceStyleShading);

            foreach (IIfcRepresentationItem ifcRepresentationItem in representationItems)
                model.Instances.New<IfcStyledItem>(item =>
                {
                    item.Item = (IfcRepresentationItem)ifcRepresentationItem;
                    item.Styles.Add(surfaceStyle);
                });
        }

        [Pure]
        private static IIfcColourRgb CreateColourRgb(IModel model, IColor color)
        {
            double[] normalizedColour = color.ToNormal();
            IfcColourRgb colourRgb = model.Instances.New<IfcColourRgb>(colour =>
            {
                colour.Red = normalizedColour[0];
                colour.Green = normalizedColour[1];
                colour.Blue = normalizedColour[2];
            });
            return colourRgb;
        }

        [Pure]
        private static IIfcSurfaceStyleShading CreateSurfaceStyleShading(IModel model, IIfcColourRgb colourRgb)
        {
            IIfcSurfaceStyleShading styleShading = model.Instances.New<IfcSurfaceStyleShading>(shading =>
            {
                shading.SurfaceColour = (IfcColourRgb)colourRgb;
            });
            return styleShading;
        }

        [Pure]
        private static IIfcSurfaceStyle CreateSurfaceStyle(IModel model,
            IfcSurfaceStyleElementSelect surfaceStyleElementSelect)
        {
            IIfcSurfaceStyle surfaceStyle = model.Instances.New<IfcSurfaceStyle>(style =>
            {
                style.Styles.Add(surfaceStyleElementSelect);
                style.Side = IfcSurfaceSide.BOTH;
            });
            return surfaceStyle;
        }
    }
}