using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Start.Attributes;
using Start.Entities;
using Start.Extensions;
using Start.Interfaces;
using Utils;

namespace Start.API
{
    /// <summary>
    ///     Represents a high-level API wrapper over START project data.
    ///     Provides methods for opening projects, accessing elements,
    ///     resolving connections between entities, and building runtime entity graph.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The project can be opened either from:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>Existing <see cref="StartDocument" /></description>
    ///             </item>
    ///             <item>
    ///                 <description><see cref="StartAutoServer" /> instance</description>
    ///             </item>
    ///             <item>
    ///                 <description>File path</description>
    ///             </item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         After loading raw data from <see cref="StartBaseRootDataArray" />,
    ///         the class builds domain entities, resolves connections,
    ///         and calculates spatial positions.
    ///     </para>
    /// </remarks>
    public class StartProject : IStartProject, IDisposable
    {
        private static readonly Logger _logger = Logger.GetInstance();

        private static readonly Dictionary<StartElementTypeEnum, Type> _startEntityTypesByElementType =
            AttributeFinder.GetClassesWithAttribute<StartElementAttribute>().SelectMany(type =>
                type.GetCustomAttributes<StartElementAttribute>()
                    .Select(attr => new { TypeEnum = attr.Type, Type = type })
            ).ToDictionary(x => x.TypeEnum, x => x.Type);

        private readonly IStartAutoServer? _autoServer;
        private readonly IStartBaseRootDataArray _dataArray;
        private readonly IStartDocument? _document;

        /// <summary>
        ///     Initializes a new instance of <see cref="StartProject" />.
        /// </summary>
        /// <param name="autoServer">Optional automation server instance.</param>
        /// <param name="document">Optional START document.</param>
        /// <param name="dataArray">Underlying data array dispatch object.</param>
        public StartProject(IStartAutoServer? autoServer, IStartDocument? document, IStartBaseRootDataArray dataArray)
        {
            _autoServer = autoServer;
            _document = document;
            _dataArray = dataArray;
        }

        /// <summary>
        ///     Performs post-import visualization adjustments.
        ///     Sets view orientation and zoom.
        /// </summary>
        public void OnImportFinish()
        {
            _document?.SetViewOfModel(new[] { 1, 3 });
            _document?.DrawViewAll();
            _document?.DrawFitAll();
            _dataArray.DeleteElement(0);
        }

        /// <summary>
        ///     Returns connected entities of specified type.
        /// </summary>
        /// <param name="entity">Source entity.</param>
        /// <param name="type">Element type filter.</param>
        /// <returns>Array of connected entities.</returns>
        [Pure]
        public IStartBaseRoot[] GetConnEntities(IStartBaseRoot entity, StartElementTypeEnum type)
        {
            int elementsNumber = _dataArray.GetNumberConns(entity.Id, type, type);
            IStartBaseRoot[] startEntities = new IStartBaseRoot[elementsNumber];
            for (int i = 0; i < elementsNumber; i++) startEntities[i] = entity.GetConnElemOnType(type, i);

            return startEntities;
        }

        /// <summary>
        ///     Returns first connected entity of specified type.
        /// </summary>
        /// <param name="entity">Source entity.</param>
        /// <param name="type">Element type filter.</param>
        /// <returns>Connected entity.</returns>
        [Pure]
        public IStartBaseRoot GetConnEntity(StartBaseRoot entity, StartElementTypeEnum type)
        {
            return entity.GetConnElemOnType(type, 0);
        }

        /// <summary>
        ///     Returns all entities within specified type range.
        /// </summary>
        /// <param name="minType">Minimum element type.</param>
        /// <param name="maxType">Maximum element type.</param>
        /// <returns>Array of entities.</returns>
        [Pure]
        public IStartBaseRoot[] GetEntities(StartElementTypeEnum minType, StartElementTypeEnum maxType)
        {
            int elementsNumber = _dataArray.GetNumberElements(minType, maxType);
            IStartBaseRoot[] startEntities = new IStartBaseRoot[elementsNumber];
            for (int i = 0; i < elementsNumber; i++)
                startEntities[i] = _dataArray.GetElementDispatch(i, minType, maxType);

            return startEntities;
        }

        /// <summary>
        ///     Returns number of elements within specified type range.
        /// </summary>
        [Pure]
        public int GetNumberElements(StartElementTypeEnum minType, StartElementTypeEnum maxType)
        {
            return _dataArray.GetNumberElements(minType, maxType);
        }

        /// <summary>
        ///     Adds new element of specified type.
        /// </summary>
        /// <param name="type">Element type.</param>
        /// <param name="index">Output index of created element.</param>
        /// <returns>Created <see cref="StartBaseRoot" />.</returns>
        public IStartBaseRoot AddElement(StartElementTypeEnum type, out int index)
        {
            return new StartBaseRoot(_dataArray.AddElement(type, out index));
        }

        /// <summary>
        ///     Adds new element and connects it to start and end nodes.
        /// </summary>
        public IStartBaseRoot AddElementAndNode(StartElementTypeEnum type, int nSNode, int nENode, out int index)
        {
            return new StartBaseRoot(_dataArray.AddElementAndNode(type, nSNode, nENode, out index));
        }

        /// <summary>
        ///     Adds new element connected to single node.
        /// </summary>
        public IStartBaseRoot AddElementAndNode(StartElementTypeEnum type, int nSNode, out int index)
        {
            return new StartBaseRoot(_dataArray.AddElementAndNode(type, nSNode, out index));
        }

        /// <summary>
        ///     Returns full project data serialized to JSON.
        /// </summary>
        /// <returns>JSON string representation of all elements.</returns>
        [Pure]
        public string GetDataJson()
        {
            return _dataArray.GetDataJson(StartElementTypeEnum.ALL, StartElementTypeEnum.ALL);
        }

        public IStartEntity[] GetStartEntities()
        {
            StartDataArrayItem[] dataArrayItems = GetDataArrayItems();
            return dataArrayItems.Select(item => item.Entity).ToArray();
        }

        /// <summary>
        ///     Releases unmanaged resources associated with
        ///     <see cref="StartBaseRootDataArray" />,
        ///     <see cref="StartDocument" />,
        ///     and <see cref="StartAutoServer" />.
        /// </summary>
        public void Dispose()
        {
            _dataArray.Dispose();
            _document?.Dispose();
            _autoServer?.Dispose();
        }

        /// <summary>
        ///     Opens project from an already loaded <see cref="StartDocument" />.
        /// </summary>
        /// <param name="document">Loaded START document.</param>
        /// <returns>Initialized <see cref="StartProject" /> instance.</returns>
        [Pure]
        public static IStartProject OpenFromDocument(StartDocument document)
        {
            IStartBaseRootDataArray baseRootDataArray = document.GetDataArrayDispatch();
            return new StartProject(null, document, baseRootDataArray);
        }

        /// <summary>
        ///     Opens project using automation server.
        /// </summary>
        /// <param name="autoServer">START automation server.</param>
        /// <param name="startDocument">Associated START document.</param>
        /// <returns>Initialized <see cref="StartProject" /> instance.</returns>
        [Pure]
        public static IStartProject OpenFromAutoServer(StartAutoServer autoServer, StartDocument startDocument)
        {
            IStartBaseRootDataArray baseRootDataArray = autoServer.GetDataArrayDispatch();
            return new StartProject(autoServer, startDocument, baseRootDataArray);
        }

        /// <summary>
        ///     Opens a START project from file.
        /// </summary>
        /// <param name="filepath">Path to project file.</param>
        /// <param name="mode">Open mode (default: 0x4).</param>
        /// <returns>Initialized <see cref="StartProject" /> instance.</returns>
        [Pure]
        public static IStartProject OpenProject(string filepath, int mode = 0x4)
        {
            IStartAutoServer autoServer = new StartAutoServer();
            IStartDocument document = autoServer.LoadStartDocument(mode, filepath);
            IStartBaseRootDataArray baseRootDataArray = document.GetDataArrayDispatch();

            return new StartProject(autoServer, document, baseRootDataArray);
        }

        /// <summary>
        ///     Deserializes project data into strongly-typed entities,
        ///     resolves connections and calculates spatial positions.
        /// </summary>
        /// <returns>Array of populated <see cref="StartDataArrayItem" />.</returns>
        /// <exception cref="NullReferenceException">
        ///     Thrown when JSON deserialization fails.
        /// </exception>
        [Pure]
        internal StartDataArrayItem[] GetDataArrayItems()
        {
            string json = GetDataJson();
            StartDataArrayItem[]? allDataArrayItems = JsonConvert.DeserializeObject<StartDataArrayItem[]>(json);
            if (allDataArrayItems == null)
                throw new NullReferenceException($"{nameof(GetDataArrayItems)} Cannot deserialize objects");

            // Filtering and creating start entities
            StartDataArrayItem[] dataArrayItems = allDataArrayItems.Select(item =>
            {
                try
                {
                    AddEntityToStartDataArrayItem(item);
                    return item;
                }
                catch (Exception)
                {
                    return null;
                }
            }).Where(item => item?.Entity != null).ToArray()!;

            AddConnectionsToEntities(ref dataArrayItems);
            AddPositionsToEntities(ref dataArrayItems);

            return dataArrayItems;
        }

        /// <summary>
        ///     Resolves node connections and assigns connected entities.
        /// </summary>
        private static void AddConnectionsToEntities(ref StartDataArrayItem[] dataArrayItems)
        {
            foreach (StartDataArrayItem startDataArrayItem in dataArrayItems)
            {
                // Add connected elements to Entity
                IEnumerable<StartDataArrayItem> connElements = dataArrayItems.GetConnElements(startDataArrayItem);
                IEnumerable<IStartEntity> connEntities = connElements
                    .Select(item => item.Entity);
                startDataArrayItem.Entity.ConnectedEntities.AddRange(connEntities);
            }
        }

        /// <summary>
        ///     Calculates spatial positions for entities based on connected nodes.
        /// </summary>
        /// <remarks>
        ///     For two-node entities (e.g. pipes), position may be adjusted
        ///     if connected to eccentric reducer.
        /// </remarks>
        private static void AddPositionsToEntities(ref StartDataArrayItem[] dataArrayItems)
        {
            foreach (StartDataArrayItem startDataArrayItem in dataArrayItems)
                try
                {
                    // Calculating and setting positions for entities
                    IStartEntity entity = startDataArrayItem.Entity;
                    StartNodeEntity[] connNodes = entity.ConnectedEntities
                        .OfType<StartNodeEntity>()
                        .ToArray();
                    switch (entity)
                    {
                        case IStartOneNodeEntity oneNodeEntity:
                        {
                            oneNodeEntity.Position = connNodes[0].Position;
                            break;
                        }
                        case IStartTwoNodeEntity twoNodeEntity:
                        {
                            twoNodeEntity.StartPosition = connNodes[0].Position;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e.ToString());
                }
        }

        /// <summary>
        ///     Creates and assigns entity instance to data array item
        ///     using reflection and <see cref="StartElementAttribute" /> mapping.
        /// </summary>
        private static void AddEntityToStartDataArrayItem(StartDataArrayItem dataArrayItem)
        {
            if (!_startEntityTypesByElementType.TryGetValue(dataArrayItem.Type, out Type startEntityType))
                return;
            if (JsonConvert.DeserializeObject(dataArrayItem.Data.ToString(),
                    startEntityType) is not IStartEntity entity)
                return;

            _logger.Info($"Found object with type: {dataArrayItem.Type}, id: {dataArrayItem.DataArrayIndex}. " +
                         $"Added entity of type {startEntityType.Name} to data array item.");

            dataArrayItem.Entity = entity;
            dataArrayItem.Entity.ID = dataArrayItem.Type == StartElementTypeEnum.NODE
                ? dataArrayItem.NodeIds[0]
                : dataArrayItem.DataArrayIndex;
            if (dataArrayItem.Type == StartElementTypeEnum.NODE)
                dataArrayItem.Entity.Name = dataArrayItem.NodeIds[0].ToString();
        }
    }
}