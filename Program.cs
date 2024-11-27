using System.Reflection;
using IFC_Converter.IFC;
using IFC_Converter.IFC.Entities;
using IFC_Converter.Start;
using IFC_Converter.Start.API;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;
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
        
        IfcProject project = model.Instances.New<IfcProject>();
        project.Initialize(ProjectUnits.SIUnitsUK);
        
        var lengthUnit = model.Instances.FirstOrDefault<IfcSIUnit>(unit => unit.UnitType == IfcUnitEnum.LENGTHUNIT);
        lengthUnit.Name = IfcSIUnitName.METRE;
        lengthUnit.Prefix = null;
        
        Vector3 circleCenter = Vector3.Zero;

        IfcCircle circle = model.Instances.New<IfcCircle>(ifcCircle =>
        {
            ifcCircle.Radius = 2;
            ifcCircle.Position = model.Instances.New<IfcAxis2Placement3D>(placement3D =>
            {
                placement3D.Location = model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(circleCenter.x, circleCenter.y, circleCenter.z));
                placement3D.Axis = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(0, 0, 1));
                placement3D.RefDirection = model.Instances.New<IfcDirection>(direction => direction.SetXYZ(1, 0, 0));
            });
        });

        IfcTrimmedCurve arcCurve = model.Instances.New<IfcTrimmedCurve>(curve =>
        {
            curve.BasisCurve = circle;
            curve.Trim1.Add(model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(0, -2, 0)));
            curve.Trim2.Add(model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(-2, 0, 0)));
            curve.SenseAgreement = true;
            curve.MasterRepresentation = IfcTrimmingPreference.CARTESIAN;
        });
        
        IfcPlane surface = model.Instances.New<IfcPlane>(plane =>
        {
            plane.Position = model.Instances.New<IfcAxis2Placement3D>(pos =>
            {
                pos.Location = model.Instances.New<IfcCartesianPoint>(pt => pt.SetXYZ(0, 0, 0));
                pos.Axis = model.Instances.New<IfcDirection>(dir => dir.SetXYZ(0, 0, 1));
                pos.RefDirection = model.Instances.New<IfcDirection>(dir => dir.SetXYZ(1, 0, 0));
            });
        });

        IfcCircleProfileDef profileDef = model.Instances.New<IfcCircleProfileDef>(def =>
        {
            def.ProfileType = IfcProfileTypeEnum.AREA;
            def.Radius = 1;
            def.Position = model.Instances.New<IfcAxis2Placement2D>(placement2D =>
            {
                placement2D.Location = model.Instances.New<IfcCartesianPoint>(point => point.SetXY(0, 0));
                placement2D.RefDirection = model.Instances.New<IfcDirection>(direction => direction.SetXY(1, 0));
            });
        });
        
        var sweptSolid = model.Instances.New<IfcSurfaceCurveSweptAreaSolid>(solid =>
        {
            solid.SweptArea = profileDef;
            solid.Directrix = arcCurve;
            solid.ReferenceSurface = surface;
        });

        IfcShapeRepresentation representation = model.Instances.New<IfcShapeRepresentation>(sr =>
        {
            sr.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            sr.RepresentationIdentifier = "Body";
            sr.RepresentationType = "SweptSolid";
            sr.Items.Add(sweptSolid);
        });
        
        IfcProductDefinitionShape shape = model.Instances.New<IfcProductDefinitionShape>(shape => shape.Representations.Add(representation));
        IfcPipeSegment pipe = model.Instances.New<IfcPipeSegment>(segment =>
        {
            segment.Name = "Pipe Bend";
            segment.Representation = shape;
            segment.ObjectPlacement = model.Instances.New<IfcLocalPlacement>(placement =>
            {
                placement.RelativePlacement = model.Instances.New<IfcAxis2Placement3D>(placement3D =>
                {
                    placement3D.Location = model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(0, 0, 0));
                });
            });
        });
        
        transaction.Commit();
        model.SaveAs("D:\\Test.ifc");*/

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

        var startBendEntities = startProject.GetEntities<StartBendEntity>(StartElementType.ELBOW);
        foreach (var startBendEntity in startBendEntities)
        {
            Console.WriteLine($"Added pipe bend: {startBendEntity.Id}");

            StartNodeEntity node = startProject.GetConnEntity<StartNodeEntity>(startBendEntity, StartElementType.NODE);
            StartPipeEntity[] connPipes = startProject.GetConnEntities<StartPipeEntity>(node, StartElementType.PIPE_ELEMENT);
            IfcBendEntity ifcBendEntity = new(startBendEntity, node, connPipes);
            ifcConverter.AddEntity(ifcBendEntity);
        }

        ifcConverter.SaveAs(outputFilepath);
    }
}