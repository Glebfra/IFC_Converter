using IFC.Entities.Abstract;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities;

public class IfcFabricatedTeeEntity : IfcAbstractTeeEntity
{
    public readonly double Length;
    public readonly double Height;

    public IfcFabricatedTeeEntity(StartFabricatedTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcPipeEntity[] connPipes) 
        : base(teeEntity, nodeEntity, connPipes)
    {
        Length = teeEntity.GetHeaderLength();
        Height = teeEntity.GetBranchHeight() + _branchPipes[0].Diameter / 2;
        _nodeEntity.ConnEntities.Add(this);
    }

    public override IfcProduct CreateAndAdd(IModel model)
    {
        _pipeFitting = CreateTeeEntity(model, Length, Height);
        return _pipeFitting;
    }
}