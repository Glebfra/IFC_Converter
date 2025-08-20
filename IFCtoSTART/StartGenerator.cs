using System.Linq;
using IFC;
using IFCtoSTART.Importers;
using IFCtoSTART.Tools;
using Start;
using Start.API;
using Xbim.Ifc4.Kernel;
using Xbim.IO.Step21;

namespace IFCtoSTART
{
    internal class StartGenerator
    {
        private DataContainer _dataContainer;
        
        public StartGenerator(DataContainer dataContainer)
        {
            _dataContainer = dataContainer;
        }

        public void Convert(StartDocument startDocument)
        {
            IImporter importer = ImporterFactory.CreateImporter(_dataContainer.ImportTypeEnum);
            
            using (IFCProject ifcProject = IFCProject.OpenProject(_dataContainer.InputFilePath))
            {
                IfcProduct[] products = ifcProject.GetProducts().ToArray();
            }
        }
    }
}