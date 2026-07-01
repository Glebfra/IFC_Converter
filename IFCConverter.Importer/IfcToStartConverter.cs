using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Importers;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Topology;
using IFCConverter.Importer.TopologyAugmenter;
using IFCConverter.Utils;
using MathNet.Numerics.LinearAlgebra;
using Start.API;
using Start.Extensions;
using Start.Interfaces;
using Utils;
using Xbim.Ifc4.Kernel;
using IfcProject = Ifc.API.IfcProject;

namespace IFCConverter.Importer
{
    public class IfcToStartConverter
    {
        private readonly BoundaryResolver _boundaryResolver = BoundaryResolver.GetInstance();
        private readonly ConnectionResolver _connectionResolver = ConnectionResolver.GetInstance();
        private readonly INodeTopologyResolver _nodeTopologyResolver;
        
        private const double VectorTolerance = 1e-3;
        private readonly VectorComparer _comparer = new(VectorTolerance);

        private readonly ImportDataContainer _importDataContainer;

        private readonly Logger _logger = Logger.GetInstance();
        private readonly StartNodeRegistry _nodeRegistry;

        private readonly List<ITopologyModelAugmenter> _modelAugmenters = new List<ITopologyModelAugmenter>();

        public IfcToStartConverter(ImportDataContainer importDataContainer)
        {
            _importDataContainer = importDataContainer;
            _nodeTopologyResolver = new NodeTopologyResolver(_comparer);
            _nodeRegistry = new StartNodeRegistry(_comparer);
            
            _modelAugmenters.Add(new FittingsSegmentsAugmenter(_comparer));
        }

        public void Convert(IStartDocument startDocument)
        {
            _logger.System($"STARTtoIFC converter v.{Assembly.GetExecutingAssembly().GetName().Version}");
            
            IEnumerable<IEntityProxy> proxies = ImportProxies();

            ITopologyModel model = TopologyModel.Create(proxies, _comparer);
            foreach (ITopologyModelAugmenter topologyModelAugmenter in _modelAugmenters)
            {
                topologyModelAugmenter.Augment(ref model);
            }

            using (IStartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                foreach (ITopologyEntity topologyEntity in model.Entities)
                {
                    IStartEntity startEntity = topologyEntity.ToStartEntity();
                    Vector<double>[] nodePositions = topologyEntity.Nodes.Select(node => node.Position).ToArray();

                    StartEntityProxy startEntityProxy = startProject.AddEntity(startEntity);
                    StartEntityProxy[] nodeProxies = _nodeRegistry.GetOrCreateNodes(startProject, nodePositions);
                    startEntityProxy.ConnectNodes(nodeProxies);
                }
                
                startProject.OnImportFinish();
            }
        }

        private IEnumerable<IEntityProxy> ImportProxies()
        {
            using (IfcProject ifcProject = IfcProject.OpenProject(_importDataContainer.InputFilePath))
            {
                ImporterRegistry registry = ImporterRegistry.GetInstance();
                IImporter importer = registry.CreateImporter(ifcProject);
                IReadOnlyCollection<IfcProduct> products = ifcProject.Model.Instances.OfType<IfcProduct>().ToArray();
                return importer.ImportProxies(products);
            }
        }
    }
}