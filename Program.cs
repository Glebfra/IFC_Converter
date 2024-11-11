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
    }
}