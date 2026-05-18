using IFCConverter.Converters.Importers.Proxies;
using Xbim.Ifc4.SharedBldgElements;

namespace IFCConverter.Converters.Importers.Aveva
{
    internal class AvevaReducerImporter : AbstractEntityImporter<IfcBuildingElementProxy, ReducerProxy>
    {
        public override ReducerProxy ReadTyped(IfcBuildingElementProxy source)
        {
            throw new System.NotImplementedException();
        }
    }
}