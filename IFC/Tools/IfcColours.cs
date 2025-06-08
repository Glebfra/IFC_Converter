using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.PresentationAppearanceResource;

namespace IFC.Tools
{
    public static class IfcColours
    {
        public static IfcColourRgb CreateColour(IModel model, Colour colour)
        {
            double[] normalizedColour = colour.ToNormal();
            return model.Instances.New<IfcColourRgb>(colourRgb =>
            {
                colourRgb.Red = normalizedColour[0];
                colourRgb.Green = normalizedColour[1];
                colourRgb.Blue = normalizedColour[2];
            });
        }

        public static IfcSurfaceStyleShading CreateSurfaceStyleShading(IModel model, IfcColourRgb colourRgb)
        {
            return model.Instances.New<IfcSurfaceStyleShading>(shading =>
            {
                shading.SurfaceColour = colourRgb;
            });
        }

        public static IfcSurfaceStyle CreateSurfaceStyle(IModel model, IfcSurfaceStyleShading surfaceStyleShading)
        {
            return model.Instances.New<IfcSurfaceStyle>(style =>
            {
                style.Styles.Add(surfaceStyleShading);
            });
        }

        public static IEnumerable<IfcStyledItem> StyleItems(IModel model, IfcSurfaceStyle surfaceStyle, IEnumerable<IfcRepresentationItem> representationItems)
        {
            List<IfcStyledItem> styledItems = new List<IfcStyledItem>();
            foreach (IfcRepresentationItem ifcRepresentationItem in representationItems)
            {
                styledItems.Add(model.Instances.New<IfcStyledItem>(item =>
                {
                    item.Item = ifcRepresentationItem;
                    item.Styles.Add(surfaceStyle);
                }));
            }

            return styledItems;
        }

        public static IEnumerable<IfcStyledItem> StyleItems(IModel model, Colour colour, IEnumerable<IfcRepresentationItem> representationItems)
        {
            IfcColourRgb colourRgb = CreateColour(model, colour);
            IfcSurfaceStyleShading surfaceStyleShading = CreateSurfaceStyleShading(model, colourRgb);
            IfcSurfaceStyle surfaceStyle = CreateSurfaceStyle(model, surfaceStyleShading);
            
            return StyleItems(model, surfaceStyle, representationItems);
        }
        
        public static IEnumerable<IfcStyledItem> StyleItems(IModel model, Colour colour, IfcRepresentationItem representationItem)
        {
            return StyleItems(model, colour, new[] { representationItem });
        }
    }
}