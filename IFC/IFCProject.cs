using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IFC.Entities.Interfaces;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.RepresentationResource;
using Xbim.IO;

namespace IFC
{
    public class IFCProject : IDisposable
    {
        public IModel Model => _model;
        
        private ITransaction _transaction;
        private readonly IfcStore _model;
        private readonly IfcBuilding _building;
        private readonly List<IfcProduct> _ifcObjects;

        public IFCProject(IfcStore model)
        {
            _model = model;
            _building = _model.Instances.FirstOrDefault<IfcBuilding>();
            _transaction = _model.BeginTransaction("Objects adding");

            _ifcObjects = new List<IfcProduct>();
        }
        
        public static IFCProject CreateProject(string name)
        {
            // TODO update application version
            XbimEditorCredentials editor = new()
            {
                ApplicationFullName = "PASS/Start-Prof",
                ApplicationVersion = $"04.87 R2 (STARTtoIFC: {Assembly.GetExecutingAssembly().GetName().Version.ToString()})",
            };
        
            IfcStore model = IfcStore.Create(editor, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);
            model.Header.FileDescription.Description.Add("ViewDefinition [DesignTransferView]");
            model.Header.FileDescription.Description.Add("Version 2.0");
            model.Header.FileName.Name = name;

            ITransaction transaction = model.BeginTransaction("Model creation");
            IfcProject project = model.Instances.New<IfcProject>(p => p.Name = name);
            GenerateUnits(model, project);

            XbimVector3D coordinates = XbimVector3D.Zero;
            XbimVector3D forward = new XbimVector3D(0, 0, 1);
            XbimVector3D up = new XbimVector3D(0, 1, 0);
            XbimMatrix3D worldMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, worldMatrix3D);

            IfcGeometricRepresentationContext context = model.Instances.New<IfcGeometricRepresentationContext>(representationContext =>
            {
                representationContext.ContextIdentifier = "Start context";
                representationContext.Precision = 1e-5;
                representationContext.ContextType = "Model";
                representationContext.CoordinateSpaceDimension = 3;
                representationContext.WorldCoordinateSystem = objectPlacement.Axis2Placement3D;
            });
        
            IfcSite site = model.Instances.New<IfcSite>(ifcSite =>
            {
                ifcSite.Name = "Site";
                ifcSite.CompositionType = IfcElementCompositionEnum.ELEMENT;
                ifcSite.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            project.AddSite(site);

            IfcBuilding building = model.Instances.New<IfcBuilding>(ifcBuilding =>
            {
                ifcBuilding.Name = "Building";
                ifcBuilding.CompositionType = IfcElementCompositionEnum.ELEMENT;
                ifcBuilding.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            site.AddBuilding(building);
            transaction.Commit();

            return new IFCProject(model);
        }

        public static IFCProject OpenProject(string filePath)
        {
            IfcStore model;
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                model = IfcStore.Open(stream, StorageType.Ifc, XbimSchemaVersion.Ifc4, XbimModelType.MemoryModel);
            }

            return new IFCProject(model);
        }

        public IEnumerable<IfcProduct> GetProducts()
        {
            return _model.Instances.OfType<IfcProduct>();
        }

        public void AddEntity(IIfcEntity entity)
        {
            IfcProduct ifcProduct = entity.CreateAndAdd(_model);
            _ifcObjects.Add(ifcProduct);
            _building.AddElement(ifcProduct);
        }

        public void AddEntities(IEnumerable<IIfcEntity> entities)
        {
            foreach (IIfcEntity entity in entities)
            {
                AddEntity(entity);
            }
        }

        public void AddEntityRaw(IfcProduct product)
        {
            _ifcObjects.Add(product);
            _building.AddElement(product);
        }

        public void GroupObjects(string groupName)
        {
            IfcSystem pipeSystem = _model.Instances.New<IfcSystem>(sys => { sys.Name = groupName; });
            IfcRelAssignsToGroup relAssignsToGroup = _model.Instances.New<IfcRelAssignsToGroup>(rel =>
            {
                rel.RelatingGroup = pipeSystem;
                rel.RelatedObjects.AddRange(_ifcObjects);
            });
            _ifcObjects.Clear();
        }

        public void SaveAs(string filepath)
        {
            _transaction.Commit();
            _model.SaveAs(filepath, StorageType.Ifc);
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _model.Dispose();
        }

        private static void GenerateUnits(IModel model, IIfcProject project)
        {
            project.Initialize(ProjectUnits.SIUnitsUK);
            
            IfcSIUnit lengthUnit = model.Instances.FirstOrDefault<IfcSIUnit>(unit => unit.UnitType == IfcUnitEnum.LENGTHUNIT);
            lengthUnit.Name = IfcSIUnitName.METRE;
            lengthUnit.Prefix = null;

            IfcSIUnit massUnit = model.Instances.FirstOrDefault<IfcSIUnit>(unit => unit.UnitType == IfcUnitEnum.MASSUNIT);
            massUnit.Name = IfcSIUnitName.GRAM;
            massUnit.Prefix = IfcSIPrefix.KILO;
        }
    }
}