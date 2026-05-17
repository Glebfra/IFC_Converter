using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IFCConverter.Converters.Importers;
using IFCConverter.Interfaces;
using IFCConverter.Utils;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Start.API;
using Start.Extensions;
using Start.Interfaces;
using Utils;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using IEntityProxy = IFCConverter.Interfaces.IEntityProxy;
using IfcProject = Ifc.API.IfcProject;

namespace IFCConverter.Converters
{
    internal class IfcToStartConverter
    {
        private readonly ImportDataContainer _importDataContainer;
        private readonly Logger _logger = Logger.GetInstance();
        
        private readonly StartNodeRegistry _nodeRegistry = new StartNodeRegistry();

        public IfcToStartConverter(ImportDataContainer importDataContainer)
        {
            _importDataContainer = importDataContainer;
        }

        public void Convert(IStartDocument startDocument)
        {
            _logger.System($"STARTtoIFC converter v.{Assembly.GetExecutingAssembly().GetName().Version}");

            IEnumerable<IEntityProxy> proxies;
            using (IfcProject ifcProject = IfcProject.OpenProject(_importDataContainer.InputFilePath))
            {
                IfcSIUnit[] units = ifcProject.Model.Instances.OfType<IfcSIUnit>().ToArray();
                ImporterRegistry registry = ImporterRegistry.GetInstance();
                IImporter importer = registry.CreateImporter(ifcProject);

                IEnumerable<IfcProduct> products = ifcProject.Model.Instances.OfType<IfcProduct>();
                proxies = importer.ImportEntities(products);
            }

            ITopologyProxy[] topologyProxies = proxies.OfType<ITopologyProxy>().ToArray();
            using (IStartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                foreach (IEntityProxy entityProxy in proxies)
                {
                    IStartEntity startEntity = entityProxy.ToStartEntity();
                    
                    IEnumerable<ITopologyProxy> connectedProxies = 
                        GetConnectedEntities((ITopologyProxy)entityProxy, topologyProxies);
                    foreach (ITopologyProxy connectedProxy in connectedProxies)
                    {
                        if (startEntity is not IStartClippableEntity clippableEntity ||
                            connectedProxy is not IFittingProxy fittingProxy) 
                            continue;
                        
                        IEnumerable<Vector<double>> points = connectedProxy.GetBoundaryPoints();
                        Vector<double> fittingPoint = fittingProxy.Position;
                        double length = points
                            .Select(point => (fittingPoint - point).L2Norm())
                            .OrderBy(l => l)
                            .First();
                            
                        clippableEntity.Clip(fittingPoint, -length);
                    }

                    StartEntityProxy startEntityProxy = startProject.AddEntity(startEntity);
                    StartEntityProxy[] nodeProxies = _nodeRegistry.GetOrCreateNodes(startProject, startEntity);
                    ConnectNodes(startEntityProxy, nodeProxies);
                }
            }
        }

        private static IEnumerable<ITopologyProxy> GetConnectedEntities(
            ITopologyProxy proxy,
            IEnumerable<ITopologyProxy> entityProxies)
        {
            List<ITopologyProxy> result = new List<ITopologyProxy>();

            IEnumerable<Vector<double>> boundaryPoints = proxy.GetBoundaryPoints();
            
            foreach (ITopologyProxy otherProxy in entityProxies)
            {
                IEnumerable<Vector<double>> otherBoundaryPoints = otherProxy.GetBoundaryPoints();
                bool isConnected = boundaryPoints.Any(
                    p1 => otherBoundaryPoints.Any(
                        p2 => p1.AlmostEqual(p2, 1e-3)
                    )
                );
                if (isConnected)
                    result.Add(otherProxy);
            }
            
            return result;
        }

        private static void ConnectNodes(StartEntityProxy entity, params StartEntityProxy[] nodes)
        {
            switch (nodes.Length)
            {
                case 1:
                    entity.StartBaseRoot.SetSNode(nodes[0].Index);
                    break;
                case 2:
                    entity.StartBaseRoot.SetSNode(nodes[0].Index);
                    entity.StartBaseRoot.SetENode(nodes[1].Index);
                    break;
            }
            ConnectObjects(entity, nodes);
        }

        private static void ConnectObjects(StartEntityProxy entity, params StartEntityProxy[] objects)
        {
            foreach (StartEntityProxy @object in objects)
            {
                entity.StartBaseRoot.SetConnElem(@object.Index);
            }
        }
    }
}