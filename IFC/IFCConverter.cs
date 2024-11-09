using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.IO;

namespace IFC_Converter.IFC;

public static class IFCConverter
{
    private static readonly XbimEditorCredentials editor = new()
    {
        ApplicationDevelopersName = "Start Developer",
        ApplicationFullName = "xbim toolkit",
        ApplicationIdentifier = "xbim",
        ApplicationVersion = "4.0",
        EditorsFamilyName = "Santini Aichel",
        EditorsGivenName = "Johann Blasius",
        EditorsOrganisationName = "Independent Architecture"
    };

    public static void ConvertToIfc(string outputFilepath)
    {
        using IfcStore _model = IfcStore.Create(editor, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);
        using ITransaction transaction = _model.BeginTransaction();

        IfcProject project = _model.Instances.New<IfcProject>(p => p.Name = "Ifc Project");
        project.Initialize(ProjectUnits.SIUnitsUK);
        var lengthUnit = _model.Instances.FirstOrDefault<IfcSIUnit>(unit => unit.UnitType == IfcUnitEnum.LENGTHUNIT);
        lengthUnit.Name = IfcSIUnitName.METRE;
        lengthUnit.Prefix = null;
        
        GeneratePipes(_model);

        transaction.Commit();
        _model.SaveAs(outputFilepath);
    }

    private static void GeneratePipes(IfcStore model)
    {
        IfcLocalPlacement? localPlacement = model.Instances.New<IfcLocalPlacement>(lp =>
        {
            lp.RelativePlacement = model.Instances.New<IfcAxis2Placement3D>(pos =>
            {
                pos.Location = model.Instances.New<IfcCartesianPoint>(pt => pt.SetXYZ(0, 0, 0));
                pos.RefDirection = model.Instances.New<IfcDirection>(dir => dir.SetXYZ(1, 0, 0));
                pos.Axis = model.Instances.New<IfcDirection>(dir => dir.SetXYZ(0, 0, 1));
            });
        });
        
        IfcPipeSegment? pipeSegment = model.Instances.New<IfcPipeSegment>(p =>
        {
            p.Name = "Example Pipe";
            p.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
        });
        pipeSegment.ObjectPlacement = localPlacement;
        
        IfcCircleProfileDef? profileDef = model.Instances.New<IfcCircleProfileDef>(c =>
        {
            c.ProfileType = IfcProfileTypeEnum.AREA;
            c.Radius = 0.1;
        });

        IfcExtrudedAreaSolid? extrudedSolid = model.Instances.New<IfcExtrudedAreaSolid>(s =>
        {
            s.SweptArea = profileDef;
            s.ExtrudedDirection = model.Instances.New<IfcDirection>(d => d.SetXYZ(0, 0, 1)); // Extrusion direction
            s.Depth = 2.0;
        });
        
        IfcShapeRepresentation? shapeRep = model.Instances.New<IfcShapeRepresentation>(sr =>
        {
            sr.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            sr.RepresentationIdentifier = "Body";
            sr.RepresentationType = "SweptSolid";
            sr.Items.Add(extrudedSolid);
        });
        
        IfcProductDefinitionShape? productDefShape = model.Instances.New<IfcProductDefinitionShape>();
        productDefShape.Representations.Add(shapeRep);
        pipeSegment.Representation = productDefShape;

        model.Instances.New<IfcRelDefinesByProperties>(rel =>
        {
            rel.RelatedObjects.Add(pipeSegment);
            rel.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pipe properties";
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(prop =>
                {
                    prop.Name = "Radius";
                    prop.NominalValue = new IfcPositiveLengthMeasure(1.0);
                }));
            });
        });
    }
}