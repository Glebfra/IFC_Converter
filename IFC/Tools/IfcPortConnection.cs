using System.Linq;
using IFC.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC.Tools
{
    public static class IfcPortConnection
    {
        public static IfcDistributionPort[] GetPipeClosestPorts(XbimMatrix3D ObjectMatrix3D, IfcPipeEntity[] pipeEntities)
        {
            return (
                from port in pipeEntities.SelectMany(pipe => pipe.Ports)
                let distance = (port.ObjectPlacement.ToMatrix3D().Translation - ObjectMatrix3D.Translation).Length
                orderby distance
                select port
            ).Take(pipeEntities.Length).ToArray();
        }
        
        public static IfcRelConnectsPorts[] ConnectPorts(IModel model, IfcDistributionPort[] ports, IfcElement realizingElement)
        {
            IfcRelConnectsPorts[] connectsPorts = new IfcRelConnectsPorts[Enumerable.Range(1, ports.Length).Sum()];
            
            int index = 0;
            for (int i = 0; i < ports.Length; i++)
            {
                for(int j = i+1; j < ports.Length; j++)
                {
                    int i1 = i;
                    int j1 = j;
                    connectsPorts[index++] = model.Instances.New<IfcRelConnectsPorts>(ifcRelConnectsPorts =>
                    {
                        ifcRelConnectsPorts.Name = $"{ports[i1].GlobalId}|{ports[j1].GlobalId}";
                        ifcRelConnectsPorts.Description = "Flow";
                        ifcRelConnectsPorts.RelatingPort = ports[i1];
                        ifcRelConnectsPorts.RelatedPort = ports[j1];
                        ifcRelConnectsPorts.RealizingElement = realizingElement;
                    });
                }
            }

            return connectsPorts;
        }
    }
}