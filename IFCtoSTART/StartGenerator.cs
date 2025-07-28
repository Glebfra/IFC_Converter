using System.Linq;
using IFC;
using IFCtoSTART.Tools;
using Start.API;
using Xbim.Ifc4.Kernel;

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
            using (IFCProject ifcProject = IFCProject.OpenProject(_dataContainer.IfcFilePath))
            {
                IfcProduct[] products = ifcProject.GetProducts().ToArray();
            }
        }
    }
}