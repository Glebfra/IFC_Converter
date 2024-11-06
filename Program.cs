using IFC_Converter.Start.API;

namespace IFC_Converter;

public static class Program
{
    public static void Main(string[] args)
    {
        string fileName = "D:\\testDemoApi.ctp";
        
        using (StartAutoServer startAutoServer = new StartAutoServer())
        {
            StartDocument document = startAutoServer.LoadFile(fileName);
            Console.WriteLine(startAutoServer.GetFullName());
        }
        
        /*
        string fileName = "D:\\testDemoApi.ctp";
        using (var api = StartApiWrapper.Create(fileName))
        {
            //Creating 2 nodes
            object node1 = api.AddElement(1);
            int interanlId1 = api.GetNumber(node1);
            api.SetName(node1, 1);
            object node2 = api.AddElement(1);
            int interanlId2 = api.GetNumber(node2);
            api.SetName(node2, 2);

            // Adding the pipe object between 2 nodes
            object pipe = api.AddElement(0);
            api.SetBeginNode(pipe, interanlId1);
            api.SetEndNode(pipe, interanlId2);
            api.SetDataReal(pipe, 128, 4);
            api.SetDataReal(pipe, 129, 0);
            api.SetDataReal(pipe, 130, 0);
            api.SetDataReal(pipe, 4, 0.108);

            api.Finish();
        }*/

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