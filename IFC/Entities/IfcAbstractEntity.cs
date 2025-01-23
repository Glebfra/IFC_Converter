using Xbim.Common;
using Xbim.Ifc4.Kernel;

namespace IFC_Converter.IFC.Entities;

public abstract class IfcAbstractEntity
{
    public abstract IfcProduct CreateAndAdd(IModel model);
}