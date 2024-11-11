using IFC_Converter.IFC;
using IFC_Converter.IFC.Entities;
using IFC_Converter.Start;

namespace IFC_Converter;

public static class Program
{
    public static void Main(string[] args)
    {
        string inputFilepath = "D:\\testDemoApi.ctp";
        string outputFilepath = "D:\\testDemoApi.ifc";

        using StartProject startProject = new StartProject(inputFilepath);
        using IFCConverter ifcConverter = new IFCConverter("Ifc Project");
        
        var startPipeEntities = startProject.GetPipes();
        foreach (var startPipeEntity in startPipeEntities)
        {
            using (startPipeEntity)
            {
                IfcPipeEntity ifcPipeEntity = new IfcPipeEntity(startPipeEntity);
                ifcConverter.AddPipe(ifcPipeEntity);
            }
        }
        ifcConverter.SaveAs(outputFilepath);

        /*XbimEditorCredentials editor = new()
        {
            ApplicationDevelopersName = "xbim developer",
            ApplicationFullName = "xbim toolkit",
            ApplicationIdentifier = "xbim",
            ApplicationVersion = "4.0",
            EditorsFamilyName = "Santini Aichel",
            EditorsGivenName = "Johann Blasius",
            EditorsOrganisationName = "Independent Architecture"
        };

        using IfcStore model = IfcStore.Create(editor, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);
        using ITransaction txn = model.BeginTransaction("Hello Wall");
        //there should always be one project in the model
        IfcProject? project = model.Instances.New<IfcProject>(p => p.Name = "Basic Creation");
        //our shortcut to define basic default units
        project.Initialize(ProjectUnits.SIUnitsUK);

        //create simple object and use lambda initializer to set the name
        IfcWall? wall = model.Instances.New<IfcWall>(w => w.Name = "The very first wall");

        //set a few basic properties
        model.Instances.New<IfcRelDefinesByProperties>(rel =>
        {
            rel.RelatedObjects.Add(wall);
            rel.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(pset =>
            {
                pset.Name = "Basic set of properties";
                pset.HasProperties.AddRange(new[]
                {
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Text property";
                        p.NominalValue = new IfcText("Any arbitrary text you like");
                    }),
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Length property";
                        p.NominalValue = new IfcLengthMeasure(56.0);
                    }),
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Number property";
                        p.NominalValue = new IfcNumericMeasure(789.2);
                    }),
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Logical property";
                        p.NominalValue = new IfcLogical(true);
                    })
                });
            });
        });

        txn.Commit();

        model.SaveAs("BasicWall.ifc");*/
    }
}