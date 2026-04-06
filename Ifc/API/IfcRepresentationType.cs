namespace Ifc.API
{
    public enum IfcRepresentationType
    {
        // Solid models
        SolidModel,
        SweptSolid,
        AdvancedSweptSolid,
        Brep,
        AdvancedBrep,
        CSG,
        Clipping,

        // Surface models
        SurfaceModel,
        Tessellation,

        // Geometric sets
        GeometricSet,
        GeometricCurveSet,
        Annotation2D,

        // Other
        Point,
        PointCloud,
        Curve,
        Curve2D,
        Curve3D,
        Surface,
        Surface2D,
        Surface3D,
        FillArea,
        Text,
        AdvancedSurface,
        BoundingBox,
        SectionedSpine,
        LightSource,
        MappedRepresentation
    }
}