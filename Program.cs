using IFC_Converter.IFC;
using IFC_Converter.IFC.Entities;
using IFC_Converter.Start;
using IFC_Converter.Start.API;
using IFC_Converter.Start.Entities;

namespace IFC_Converter;

public static class Program
{
    public static void Main(string[] args)
    {
        string inputFilepath = "D:\\Bend.ctp";
        string outputFilepath = "D:\\Bend.ifc";
        
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

        var startNodeEntities = startProject.GetNodes();
        foreach (var startNodeEntity in startNodeEntities)
        {
            IfcNodeEntity ifcNodeEntity = new IfcNodeEntity(startNodeEntity);
            ifcConverter.AddNode(ifcNodeEntity);
        }
        
        ifcConverter.SaveAs(outputFilepath);
    }
}