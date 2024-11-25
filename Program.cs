using System.Reflection;
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

        var startNodeEntities = startProject.GetEntities<StartNodeEntity>(StartElementType.NODE);
        foreach (var startNodeEntity in startNodeEntities)
        {
            Console.WriteLine($"Added node {startNodeEntity.Id}");

            IfcNodeEntity ifcNodeEntity = new IfcNodeEntity(startNodeEntity);
            ifcConverter.AddEntity(ifcNodeEntity);
        }

        var startPipeEntities = startProject.GetEntities<StartPipeEntity>(StartElementType.PIPE_ELEMENT);
        foreach (var startPipeEntity in startPipeEntities)
        {
            Console.WriteLine($"Added pipe {startPipeEntity.Id}");

            IfcPipeEntity ifcPipeEntity = new IfcPipeEntity(startPipeEntity);
            ifcConverter.AddEntity(ifcPipeEntity);
        }

        var startWeldedTeeEntities = startProject.GetEntities<StartWeldedTeeEntity>(StartElementType.WELDED_TEE);
        foreach (var startWeldedTee in startWeldedTeeEntities)
        {
            Console.WriteLine($"Added welded tee: {startWeldedTee.Id}");

            StartNodeEntity node = startProject.GetConnEntity<StartNodeEntity>(startWeldedTee, StartElementType.NODE);
            StartPipeEntity[] connPipes = startProject.GetConnEntities<StartPipeEntity>(node, StartElementType.PIPE_ELEMENT);
            IfcWeldedTeeEntity ifcWeldedTeeEntity = new(startWeldedTee, node, connPipes);
            ifcConverter.AddEntity(ifcWeldedTeeEntity);
        }

        ifcConverter.SaveAs(outputFilepath);
    }
}