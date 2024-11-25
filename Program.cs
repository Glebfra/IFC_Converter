using IFC_Converter.IFC;
using IFC_Converter.IFC.Entities;
using IFC_Converter.Math;
using IFC_Converter.Start;
using IFC_Converter.Start.Entities;
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
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.IO;

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

            IfcPipeEntity ifcPipeEntity = new IfcPipeEntity(startPipeEntity);
            ifcConverter.AddEntity(ifcPipeEntity);
        }

        var startWeldedTeeEntities = startProject.GetWeldingTees();
        foreach (var startWeldedTee in startWeldedTeeEntities)
        {
            Console.WriteLine($"Added welded tee: {startWeldedTee.Id}");

            StartNodeEntity node = startProject.GetConnNode(startWeldedTee);
            StartPipeEntity[] connPipes = startProject.GetConnPipes(node);
            IfcWeldedTeeEntity ifcWeldedTeeEntity = new(startWeldedTee, node, connPipes);
            ifcConverter.AddEntity(ifcWeldedTeeEntity);
        }

        ifcConverter.SaveAs(outputFilepath);
    }
}