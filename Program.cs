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
        /*XbimEditorCredentials editor = new()
        {
            ApplicationDevelopersName = "Start Developer",
            ApplicationFullName = "xbim toolkit",
            ApplicationIdentifier = "xbim",
            ApplicationVersion = "4.0",
            EditorsFamilyName = "Santini Aichel",
            EditorsGivenName = "Johann Blasius",
            EditorsOrganisationName = "Independent Architecture"
        };

        using IfcStore model = IfcStore.Create(editor, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);
        using ITransaction transaction = model.BeginTransaction();

        IfcProject project = model.Instances.New<IfcProject>(p => p.Name = "Project");
        project.Initialize(ProjectUnits.SIUnitsUK);

        IfcSIUnit lengthUnit =
            model.Instances.FirstOrDefault<IfcSIUnit>(unit => unit.UnitType == IfcUnitEnum.LENGTHUNIT);
        lengthUnit.Name = IfcSIUnitName.METRE;
        lengthUnit.Prefix = null;

        IfcCircleProfileDef circle = model.Instances.New<IfcCircleProfileDef>(def =>
        {
            def.ProfileType = IfcProfileTypeEnum.AREA;
            def.Radius = 1;
            def.Position = model.Instances.New<IfcAxis2Placement2D>(placement2D =>
            {
                placement2D.Location = model.Instances.New<IfcCartesianPoint>(point => point.SetXY(0, 0));
                placement2D.RefDirection = model.Instances.New<IfcDirection>(direction => direction.SetXY(1, 0));
            });
        });

        IfcExtrudedAreaSolid extrudedCircle1 = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
        {
            solid.SweptArea = circle;
            solid.ExtrudedDirection = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(0, 0, 1));
            solid.Position = model.Instances.New<IfcAxis2Placement3D>(placement3D =>
            {
                placement3D.Location = model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(0, 0, 0));
                placement3D.Axis = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(0, 0, 1));
                placement3D.RefDirection = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(1, 0, 0));
            });
            solid.Depth = 2;
        });

        IfcExtrudedAreaSolid extrudedCircle2 = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
        {
            solid.SweptArea = circle;
            solid.ExtrudedDirection = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(0, 0, 1));
            solid.Position = model.Instances.New<IfcAxis2Placement3D>(placement3D =>
            {
                placement3D.Location = model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(0, 0, 0));
                placement3D.Axis = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(1, 0, 0));
                placement3D.RefDirection = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(0, 1, 0));
            });
            solid.Depth = 2;
        });
        
        IfcExtrudedAreaSolid extrudedCircle3 = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
        {
            solid.SweptArea = circle;
            solid.ExtrudedDirection = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(0, 0, 1));
            solid.Position = model.Instances.New<IfcAxis2Placement3D>(placement3D =>
            {
                placement3D.Location = model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(0, 0, 0));
                placement3D.Axis = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(-1, 0, 0));
                placement3D.RefDirection = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(0, -1, 0));
            });
            solid.Depth = 2;
        });

        IfcShapeRepresentation shapeRepresentation = model.Instances.New<IfcShapeRepresentation>(representation =>
        {
            representation.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            representation.RepresentationIdentifier = "Body";
            representation.RepresentationType = "SweptSolid";
            representation.Items.Add(extrudedCircle1);
            representation.Items.Add(extrudedCircle2);
            representation.Items.Add(extrudedCircle3);
        });

        IfcProductDefinitionShape productDefinitionShape = model.Instances.New<IfcProductDefinitionShape>(shape =>
        {
            shape.Representations.Add(shapeRepresentation);
        });

        IfcWall wall = model.Instances.New<IfcWall>(ifcWall =>
        {
            ifcWall.Representation = productDefinitionShape;
            ifcWall.Name = "Name";
        });*/

        /*IfcAxis2Placement3D axis2Placement3D = model.Instances.New<IfcAxis2Placement3D>(placement3D =>
        {
            placement3D.Location = model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(0, 0, 0));
            placement3D.Axis = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(0, 0, 1));
            placement3D.RefDirection = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(1, 0, 0));
        });

        IfcCircleProfileDef circle1 = model.Instances.New<IfcCircleProfileDef>(def =>
        {
            def.ProfileType = IfcProfileTypeEnum.AREA;
            def.Radius = 1;
        });

        IfcCircleProfileDef circle2 = model.Instances.New<IfcCircleProfileDef>(def =>
        {
            def.ProfileType = IfcProfileTypeEnum.AREA;
            def.Radius = 1;
        });

        IfcExtrudedAreaSolid extrudedAreaSolid1 = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
        {
            solid.ExtrudedDirection = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(0, 0, 1));
            solid.SweptArea = circle1;
            solid.Depth = 1;
            solid.Position = axis2Placement3D;
        });
        
        IfcExtrudedAreaSolid extrudedAreaSolid2 = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
        {
            solid.ExtrudedDirection = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(0, 0, 1));
            solid.SweptArea = circle2;
            solid.Depth = 1;
            solid.Position = axis2Placement3D;
        });

        IfcBooleanResult booleanResult = model.Instances.New<IfcBooleanResult>(result =>
        {
            result.Operator = IfcBooleanOperator.UNION;
            result.FirstOperand = extrudedAreaSolid1;
            result.SecondOperand = extrudedAreaSolid2;
        });

        IfcShapeRepresentation shapeRepresentation = model.Instances.New<IfcShapeRepresentation>(representation =>
        {
            representation.Items.Add(booleanResult);
        });

        IfcProductDefinitionShape productDefinitionShape = model.Instances.New<IfcProductDefinitionShape>(shape =>
        {
            shape.Representations.Add(shapeRepresentation);
        });

        IfcPipeSegment pipeSegment = model.Instances.New<IfcPipeSegment>(segment =>
        {
            segment.Name = "Pipe";
            segment.Representation = productDefinitionShape;
            segment.ObjectPlacement = model.Instances.New<IfcLocalPlacement>(placement =>
            {
                placement.RelativePlacement = axis2Placement3D;
            });
        });*/

        // transaction.Commit();
        // model.SaveAs("D:\\Test.ifc");

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