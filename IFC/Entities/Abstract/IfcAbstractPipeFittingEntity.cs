using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC.Entities.Abstract;

public abstract class IfcAbstractPipeFittingEntity : IfcAbstractEntity
{
    public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }

    protected readonly IfcNodeEntity _nodeEntity;
    protected readonly IfcPipeEntity[] _pipeEntities;
    protected readonly double Angle;

    protected abstract IfcPipeFitting? _pipeFitting { get; set; }

    protected IfcAbstractPipeFittingEntity(IfcNodeEntity nodeEntity, IfcPipeEntity[] pipeEntities)
    {
        _nodeEntity = nodeEntity;
        _pipeEntities = pipeEntities;

        XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
        XbimVector3D[] directionToPipes = pipeEntities.Select(entity => IfcAxis.GetDirectionToPipe(entity, coordinates)).ToArray();
        XbimVector3D forward = directionToPipes[0].Negated();
        XbimVector3D up;

        if (_pipeEntities.Length != 1)
        {
            Angle = forward.Angle(directionToPipes[1]);
        }
        if (Angle != 0)
        {
            up = XbimVector3D.CrossProduct(forward, directionToPipes[1]).Normalized();
        }
        else
        {
            XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
            if (forward != WorldUp && forward != WorldUp.Negated())
            {
                up = WorldUp;
            }
            else
            {
                up = new XbimVector3D(0, 1, 0);
            }
        }

        ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
    }

    protected IfcRelConnectsPorts[] ConnectPorts(IModel model)
    {
        IfcRelConnectsPorts[] connectedPorts = new IfcRelConnectsPorts[Enumerable.Range(1, _pipeEntities.Length).Sum()];
        
        IfcDistributionPort[] closestPorts = (
            from port in _pipeEntities.SelectMany(pipe => pipe.Ports)
            let distance = (port.ObjectPlacement.ToMatrix3D().Translation - ObjectMatrix3D.Translation).Length
            orderby distance
            select port
        ).Take(_pipeEntities.Length).ToArray();

        int index = 0;
        for (int i = 0; i < closestPorts.Length; i++)
        {
            for(int j = i+1; j < closestPorts.Length; j++)
            {
                connectedPorts[index++] = model.Instances.New<IfcRelConnectsPorts>(ports =>
                {
                    ports.Name = $"{closestPorts[i].GlobalId}|{closestPorts[j].GlobalId}";
                    ports.Description = "Flow";
                    ports.RelatingPort = closestPorts[i];
                    ports.RelatedPort = closestPorts[j];
                    ports.RealizingElement = _pipeFitting;
                });
            }
        }

        return connectedPorts;
    }
}