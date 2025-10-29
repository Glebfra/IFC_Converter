namespace IFC.Tools
{
    public static class IfcRepresentationType
    {
        // Solid models
        public const string SolidModel = "SolidModel";
        public const string SweptSolid = "SweptSolid";
        public const string AdvancedSweptSolid = "AdvancedSweptSolid";
        public const string Brep = "Brep";
        public const string AdvancedBrep = "AdvancedBrep";
        public const string CSG = "CSG";
        public const string Clipping = "Clipping";
        
        // Surface models
        public const string SurfaceModel = "SurfaceModel";
        public const string Tessellation = "Tessellation";
        
        // Geometric sets
        public const string GeometricSet = "GeometricSet";
        public const string GeometricCurveSet = "GeometricCurveSet";
        public const string Annotation2D = "Annotation2D";
        
        // Other
        public const string Point = "Point";
        public const string PointCloud = "PointCloud";
        public const string Curve = "Curve";
        public const string Curve2D = "Curve2D";
        public const string Curve3D = "Curve3D";
        public const string Surface = "Surface";
        public const string Surface2D = "Surface2D";
        public const string Surface3D = "Surface3D";
        public const string FillArea = "FillArea";
        public const string Text = "Text";
        public const string AdvancedSurface = "AdvancedSurface";
        public const string BoundingBox = "BoundingBox";
        public const string SectionedSpine = "SectionedSpine";
        public const string LightSource = "LightSource";
        public const string MappedRepresentation = "MappedRepresentation";
    }
}