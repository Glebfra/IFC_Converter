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

        var startNodeEntities = startProject.GetNodes();
        foreach (var startNodeEntity in startNodeEntities)
        {
            Console.WriteLine($"Added node {startNodeEntity.Id}");

            IfcNodeEntity ifcNodeEntity = new IfcNodeEntity(startNodeEntity);
            ifcConverter.AddEntity(ifcNodeEntity);
        }

        var startPipeEntities = startProject.GetPipes();
        foreach (var startPipeEntity in startPipeEntities)
        {
            Console.WriteLine($"Added pipe {startPipeEntity.Id}");

            IfcPipeEntity ifcNodeEntity = new IfcPipeEntity(startPipeEntity);
            ifcConverter.AddEntity(ifcNodeEntity);
        }

        var startWeldedTeeEntities = startProject.GetWeldingTees();
        foreach (var startWeldedTee in startWeldedTeeEntities)
        {
            Console.WriteLine($"Added welded tee: {startWeldedTee.Id}");

            StartNodeEntity node = startProject.GetConnNode(startWeldedTee);
            StartPipeEntity[] connPipes = startProject.GetConnPipes(node);
            IfcWeldingTeeEntity ifcWeldingTeeEntity = new(startWeldedTee, node, connPipes);
            ifcConverter.AddEntity(ifcWeldingTeeEntity);
        }

        ifcConverter.SaveAs(outputFilepath);
    }
}