using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Ifc.Interfaces;
using IFCConverter.Converters.Elements;
using IFCConverter.Converters.Importers;
using IFCConverter.Converters.Importers.Proxies;
using IFCConverter.Interfaces;
using IFCConverter.Utils;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Start.API;
using Start.Extensions;
using Start.Interfaces;
using Utils;
using Xbim.Ifc4.Kernel;
using IEntityProxy = IFCConverter.Interfaces.IEntityProxy;
using IfcProject = Ifc.API.IfcProject;

namespace IFCConverter.Converters
{
    internal class IfcToStartConverter
    {
        private const double _vectorTolerance = 1e-3;
        
        private readonly ImportDataContainer _importDataContainer;

        private readonly Logger _logger = Logger.GetInstance();
        private readonly VectorComparer _comparer = new VectorComparer(_vectorTolerance);
        private readonly StartNodeRegistry _nodeRegistry = new StartNodeRegistry(_vectorTolerance);

        public IfcToStartConverter(ImportDataContainer importDataContainer)
        {
            _importDataContainer = importDataContainer;
        }
        
        public void Convert(IStartDocument startDocument)
        {
            _logger.System($"STARTtoIFC converter v.{Assembly.GetExecutingAssembly().GetName().Version}");

            using IfcProject ifcProject = IfcProject.OpenProject(_importDataContainer.InputFilePath);
            IReadOnlyCollection<IEntityProxy> proxies = ImportProxies(ifcProject);
            
            using IStartProject startProject = StartProject.OpenFromDocument(startDocument);
            foreach (IEntityProxy entityProxy in proxies)
            {
                IStartEntity startEntity = entityProxy.ToStartEntity();

                IEnumerable<IEntityProxy> connectedProxies = GetConnectedEntities(entityProxy, proxies);
                foreach (IEntityProxy connectedProxy in connectedProxies)
                {
                    if (startEntity is not IStartClippableEntity clippableEntity ||
                        connectedProxy is not IFittingProxy fittingProxy) 
                        continue;
                    
                    Vector<double> fittingPoint = fittingProxy.Position;
                    IEnumerable<Vector<double>> points = startEntity.GetPositions();
                    double length = points
                        .Select(point => (fittingPoint - point).L2Norm())
                        .OrderBy(l => l)
                        .First();
                    
                    clippableEntity.Clip(fittingPoint, -length);

                    if (connectedProxy is not ReducerProxy reducerProxy || 
                        startEntity is not IStartSegmentEntity segmentEntity)
                        continue;

                    if (segmentEntity.IsStartPosition(fittingPoint))
                        segmentEntity.StartPosition = fittingPoint;
                    else
                        segmentEntity.StartPosition = fittingPoint - segmentEntity.Projection;
                }

                StartEntityProxy startEntityProxy = startProject.AddEntity(startEntity);
                StartEntityProxy[] nodeProxies = _nodeRegistry.GetOrCreateNodes(startProject, startEntity);
                ConnectNodes(startEntityProxy, nodeProxies);
            }
            
            startProject.OnImportFinish();
        }

        [Pure]
        private static IReadOnlyCollection<IEntityProxy> ImportProxies(IIfcProject ifcProject)
        {
            ImporterRegistry registry = ImporterRegistry.GetInstance();
            IImporter importer = registry.CreateImporter(ifcProject);
            IEnumerable<IfcProduct> products = ifcProject.Model.Instances.OfType<IfcProduct>();
            return importer.ImportEntities(products).ToArray();
        }

        [Pure]
        private static IEnumerable<IEntityProxy> GetConnectedEntities(
            IEntityProxy proxy,
            IEnumerable<IEntityProxy> entityProxies)
        {
            List<IEntityProxy> result = new List<IEntityProxy>();

            IEnumerable<Vector<double>> boundaryPoints = proxy.Boundary;
            
            foreach (IEntityProxy otherProxy in entityProxies)
            {
                IEnumerable<Vector<double>> otherBoundaryPoints = otherProxy.Boundary;
                bool isConnected = boundaryPoints.Any(
                    p1 => otherBoundaryPoints.Any(
                        p2 => p1.AlmostEqual(p2, 1e-3)
                    )
                );
                if (isConnected && otherProxy != proxy)
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