using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IFCConverter.Converters.Importers;
using IFCConverter.Interfaces;
using IFCConverter.Utils;
using MathNet.Numerics.LinearAlgebra;
using Start.API;
using Start.Entities;
using Start.Extensions;
using Start.Interfaces;
using Utils;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using IfcProject = Ifc.API.IfcProject;

namespace IFCConverter.Converters
{
    internal class IfcToStartConverter
    {
        private readonly ImportDataContainer _importDataContainer;
        private readonly Logger _logger = Logger.GetInstance();

        private readonly Dictionary<Type, StartElementTypeEnum> _startElementTypesCache =
            new Dictionary<Type, StartElementTypeEnum>();

        private readonly Dictionary<Vector<double>, StartEntityProxy> _nodeEntitiesCache =
            new Dictionary<Vector<double>, StartEntityProxy>();

        private int _nodeIndexCounter = 1;

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

            IEnumerable<IBoundaryEntityProxy> boundaryEntityProxies = proxies.OfType<IBoundaryEntityProxy>();
            foreach (IBoundaryEntityProxy boundaryEntityProxy in boundaryEntityProxies)
            {
                IEnumerable<Vector<double>> boundPoints = boundaryEntityProxy.GetBoundaryPoints();
            }

            IStartEntity[] startEntities = proxies.Select(proxy => proxy.ToStartEntity()).ToArray();
            ConnectStartEntities(startEntities);
            
            using (IStartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                foreach (IStartEntity startEntity in startEntities)
                {
                    StartEntityProxy startEntityProxy = startProject.AddEntity(startEntity);
                    StartEntityProxy[] nodeEntities = GetOrCreateNodeEntities(startProject, startEntity).ToArray();
                    ConnectNodes(startEntityProxy, nodeEntities);
                }
                startProject.OnImportFinish();
            }
        }

        private static void ConnectStartEntities(IStartEntity[] startEntities)
        {
            foreach (IStartEntity startEntity in startEntities)
            {
                IEnumerable<IStartEntity> connectedEntities = startEntities
                    .Where(entity => entity.IsConnectedTo(startEntity) && entity != startEntity);
                startEntity.ConnectedEntities.AddRange(connectedEntities);
            }
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

        private IEnumerable<StartEntityProxy> GetOrCreateNodeEntities(IStartProject startProject, IStartEntity entity)
        {
            Vector<double>[]? positions = entity.GetPositions()?.ToArray();
            if (positions == null)
                throw new Exception(
                    $"Unsupported entity type. Only entities {nameof(IStartOneNodeEntity)}, {nameof(IStartTwoNodeEntity)} are supported."
                );

            return positions
                .Select(position => _nodeEntitiesCache.GetOrAdd(position, vector =>
                {
                    IStartEntity nodeEntity = new StartNodeEntity { Position = vector };
                    StartEntityProxy proxy = startProject.AddEntity(nodeEntity);
                    proxy.StartBaseRoot.SetName((_nodeIndexCounter++).ToString());
                    return proxy;
                }));
        }
    }
}