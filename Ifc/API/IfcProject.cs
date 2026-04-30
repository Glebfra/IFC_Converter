using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.RepresentationResource;
using Xbim.IO;
using IIfcProject = Ifc.Interfaces.IIfcProject;

namespace Ifc.API
{
    public class IfcProject : IIfcProject, IDisposable
    {
        private readonly IfcBuilding _building;

        private readonly IfcStore _model;
        private readonly ITransaction _transaction;

        public IfcProject(IfcStore model)
        {
            _model = model;
            _building = _model.Instances.FirstOrDefault<IfcBuilding>();
            _transaction = _model.BeginTransaction("Model creation");
        }

        public IModel Model => _model;

        public void Dispose()
        {
            _model.Dispose();
            _transaction.Dispose();
        }

        [Pure]
        public static IfcProject CreateProject(string name)
        {
            XbimEditorCredentials editor = new()
            {
                ApplicationFullName = "PASS/Start-Prof",
                ApplicationVersion = $"04.88 (STARTtoIFC: {Assembly.GetExecutingAssembly().GetName().Version})"
            };

            IfcStore model = IfcStore.Create(editor, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);
            model.Header.FileDescription.Description.Add("ViewDefinition [DesignTransferView]");
            model.Header.FileDescription.Description.Add("Version 2.0");
            model.Header.FileName.TimeStamp = DateTime.Now.ToString("s");
            model.Header.FileName.AuthorName.Add(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            model.Header.FileName.PreprocessorVersion = $"STARTtoIFC converter v.{Assembly.GetExecutingAssembly().GetName().Version}";

            using (ITransaction transaction = model.BeginTransaction("Model creation"))
            {
                Xbim.Ifc4.Kernel.IfcProject project =
                    model.Instances.New<Xbim.Ifc4.Kernel.IfcProject>(p => p.Name = name);

                IfcGeometricRepresentationContext context = model.Instances.New<IfcGeometricRepresentationContext>(
                    representationContext =>
                    {
                        representationContext.ContextIdentifier = "Start context";
                        representationContext.Precision = 1e-5;
                        representationContext.ContextType = "Model";
                        representationContext.CoordinateSpaceDimension = 3;
                    });

                IfcSite site = model.Instances.New<IfcSite>(ifcSite =>
                {
                    ifcSite.Name = "Site";
                    ifcSite.CompositionType = IfcElementCompositionEnum.ELEMENT;
                });
                project.AddSite(site);

                IfcBuilding building = model.Instances.New<IfcBuilding>(ifcBuilding =>
                {
                    ifcBuilding.Name = "Building";
                    ifcBuilding.CompositionType = IfcElementCompositionEnum.ELEMENT;
                });
                site.AddBuilding(building);
                transaction.Commit();
            }

            return new IfcProject(model);
        }

        [Pure]
        public static IfcProject OpenProject(string filePath)
        {
            IfcStore model;
            using (FileStream stream = new(filePath, FileMode.Open))
            {
                model = IfcStore.Open(stream, StorageType.Ifc, XbimSchemaVersion.Ifc4, XbimModelType.MemoryModel);
            }

            return new IfcProject(model);
        }

        public void AddEntityRaw(IfcProduct product)
        {
            _building.AddElement(product);
        }

        public void SaveAs(string filepath)
        {
            _model.Header.FileName.Name = Path.GetFileName(filepath);
            _model.SaveAs(filepath, StorageType.Ifc);
        }
    }
}