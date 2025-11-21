using System;
using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities;
using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Fittings.Vertex;
using IFC.Entities.Segments;
using IFC.Extensions;
using IFC.PropertySets;
using IFC.Tools;
using IFCConverter.Extensions.Entities;
using IFCConverter.Extensions.Tools;
using IFCConverter.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;

namespace IFCConverter.Importers
{
    internal class AvevaImporter : IImporter
    {
        public IEnumerable<IfcProduct> Products => _products;
        
        private readonly IfcSIUnit _lengthUnit;
        private readonly IfcProduct[] _products;

        public AvevaImporter(IFCProject ifcProject)
        {
            _products = ifcProject.GetProducts().ToArray();
            _lengthUnit = ifcProject.LengthUnit;
        }

        public IEnumerable<IfcPipeSegmentEntity> CreateSegments()
        {
            List<IfcPipeSegmentEntity> abstractSegmentEntities = new List<IfcPipeSegmentEntity>();
            
            IEnumerable<IfcElement> pipes = GetElementByType(_products, "TUBING");
            abstractSegmentEntities.AddRange(CreatePipeSegments(pipes.ToArray()));
            
            return abstractSegmentEntities;
        }

        public IEnumerable<IfcAbstractFittingEntity> CreateFittings(List<IfcPipeSegmentEntity> pipeSegmentEntities)
        {
            List<IfcAbstractFittingEntity> abstractFittingEntities = new List<IfcAbstractFittingEntity>();
            
            IEnumerable<IfcElement> bends = GetElementByType(_products, "ELBOW");
            abstractFittingEntities.AddRange(CreateBends(bends.ToArray(), pipeSegmentEntities));

            IEnumerable<IfcElement> tees = GetElementByType(_products, "TEE");
            abstractFittingEntities.AddRange(CreateWeldedTees(tees.ToArray(), pipeSegmentEntities));

            IEnumerable<IfcElement> reducers = GetElementByType(_products, "REDUCER");
            abstractFittingEntities.AddRange(CreateReducers(reducers.ToArray(), pipeSegmentEntities));

            IEnumerable<IfcElement> valves = GetElementByType(_products, "VALVE");
            abstractFittingEntities.AddRange(CreateValves(valves.ToArray(), pipeSegmentEntities));

            return abstractFittingEntities;
        }

        public IEnumerable<IfcAbstractAnchorEntity> CreateAnchors(List<IfcPipeSegmentEntity> pipeSegmentEntities)
        {
            List<IfcAbstractAnchorEntity> abstractAnchorEntities = new List<IfcAbstractAnchorEntity>();

            IEnumerable<IfcElement> anchors = GetElementByType(_products, "ATTACHMENT");
            abstractAnchorEntities.AddRange(CreateAnchors(anchors.ToArray(), pipeSegmentEntities));

            return abstractAnchorEntities;
        }

        private IEnumerable<IfcPipeSegmentEntity> CreatePipeSegments(IReadOnlyCollection<IfcElement> pipes)
        {
            Logger logger = Logger.GetInstance();
            
            logger.Info($"Searching {nameof(IfcPipeSegmentEntity)} objects");
            List<IfcPipeSegmentEntity> pipeSegmentEntities = new List<IfcPipeSegmentEntity>(capacity: pipes.Count);
            foreach (IfcElement pipeElement in pipes)
            {
                try
                {
                    IfcLabel name = pipeElement.Name ?? new IfcLabel("");
                    IfcIdentifier tag = pipeElement.Tag ?? new IfcIdentifier("");
                    
                    IIfcRepresentationItem? representationItem = pipeElement.GetRepresentationItems().FirstOrDefault();
                    if (representationItem == null)
                        throw new Exception($"Cannot find representation item for {nameof(pipeElement)} with ID: {pipeElement.GlobalId}");

                    PipeProperties pipeProperties = representationItem switch
                    {
                        IfcExtrudedAreaSolid extrudedAreaSolid => extrudedAreaSolid.GetPipeProperties(),
                        IfcTriangulatedFaceSet triangulatedFaceSet => triangulatedFaceSet.GetPipeProperties(),
                        _ => throw new Exception($"Representation item can be only: {nameof(IfcTriangulatedFaceSet)}, {nameof(IfcExtrudedAreaSolid)}")
                    };

                    XbimVector3D coordinates = pipeProperties.Coordinates * _lengthUnit.Power;
                    XbimMatrix3D shapeMatrix3D = MatrixExtensions.CreateWorld(coordinates, pipeProperties.Direction);
                    double length = pipeProperties.Length * _lengthUnit.Power;
                    double diameter = pipeProperties.Radius * 2 * _lengthUnit.Power;
                    
                    IfcPipeSegmentEntity pipeSegmentEntity = new IfcPipeSegmentEntity(name, tag, shapeMatrix3D, length, diameter);
                    IPropertySet[] propertySets = pipeElement.GetPropertySets().ToArray();
                    pipeSegmentEntity.PropertySets.AddRange(propertySets);
                    pipeSegmentEntities.Add(pipeSegmentEntity);
                    
                    logger.Info($"Created {pipeSegmentEntity}, ID: {pipeElement.GlobalId}");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    logger.Error($"Failed to create pipe segment entity with ID: {pipeElement.GlobalId}");
                }
            }
            logger.Info($"Found {pipeSegmentEntities.Count} {nameof(IfcPipeSegmentEntity)} objects");

            return pipeSegmentEntities;
        }

        private IEnumerable<IfcCadBendEntity> CreateBends(IReadOnlyCollection<IfcElement> bends, ICollection<IfcPipeSegmentEntity> abstractSegmentEntities)
        {
            Logger logger = Logger.GetInstance();
            
            logger.Info($"Searching {nameof(IfcCadBendEntity)} objects");
            List<IfcCadBendEntity> bendEntities = new List<IfcCadBendEntity>(capacity: bends.Count);
            foreach (IfcElement bendElement in bends)
            {
                try
                {
                    IfcLabel name = bendElement.Name ?? new IfcLabel("");
                    IfcIdentifier tag = bendElement.Tag ?? new IfcIdentifier("");
                    
                    IPropertySet[] propertySets = bendElement.GetPropertySets().ToArray();
                    
                    AVEVA_EntityParameters? avevaEntityParameters = propertySets.OfType<AVEVA_EntityParameters>().FirstOrDefault();
                    if (avevaEntityParameters == null)
                        throw new Exception("Bend does not have AVEVA_EntityParameters property set.");
                    
                    AVEVA_Pset? avevaPset = propertySets.OfType<AVEVA_Pset>().FirstOrDefault();
                    if (avevaPset == null)
                        throw new Exception("Bend does not have AVEVA_Pset property set.");
                    
                    IIfcRepresentationItem? representationItem = bendElement.GetRepresentationItems().FirstOrDefault();
                    if (representationItem == null)
                        throw new Exception($"Cannot find representation item for {nameof(bendElement)} with ID: {bendElement.GlobalId}");
                    
                    BendProperties bendProperties = representationItem switch
                    {
                        IfcRevolvedAreaSolid revolvedAreaSolid => revolvedAreaSolid.GetBendProperties(),
                        IfcTriangulatedFaceSet triangulatedFaceSet => triangulatedFaceSet.GetBendProperties(avevaPset),
                        _ => throw new Exception($"Representation item can be only: {nameof(IfcTriangulatedFaceSet)}, {nameof(IfcRevolvedAreaSolid)}")
                    };
                    
                    XbimVector3D[] boundPoints = bendProperties.BoundPoints
                        .Select(point => point * _lengthUnit.Power)
                        .ToArray();
                    List<IfcAbstractSegmentEntity> connectedSegments = abstractSegmentEntities
                        .GetConnectedSegments(boundPoints)
                        .ToList();
                    
                    XbimVector3D coordinates = avevaPset.GetPosition() * _lengthUnit.Power;
                    double bendRadius = bendProperties.Radius * _lengthUnit.Power;
                    double angle = bendProperties.Angle;
                    double length = bendRadius * angle;
                    double pipeDiameter = bendProperties.PipeDiameter * _lengthUnit.Power;
                    double clipLength = bendRadius * Math.Tan(angle / 2);
                    
                    if (connectedSegments.Count is 0)
                        throw new NullReferenceException("Cannot find connected segments for bend.");
                    if (connectedSegments.Count is 1)
                    {
                        IfcLabel pipeName = new IfcLabel($"{name}_AutoPipe");
                        IfcIdentifier pipeTag = new IfcIdentifier($"{tag}_AutoPipe");
                        
                        XbimVector3D newPipePoint = boundPoints.First(point => !connectedSegments[0].IsContainPoint(point));
                        XbimVector3D direction = (newPipePoint - coordinates).Normalized();
                        XbimMatrix3D pipeMatrix3D = MatrixExtensions.CreateWorld(coordinates, direction);
                        IfcNodeEntity[] pipeNodeEntities = new IfcNodeEntity[]
                        {
                            new IfcNodeEntity(pipeMatrix3D),
                            new IfcNodeEntity(pipeMatrix3D.Translate(pipeMatrix3D.Forward * clipLength))
                        };
                        IfcPipeSegmentEntity newPipeSegmentEntity = new IfcPipeSegmentEntity(
                            pipeName, pipeTag, pipeMatrix3D, 0, pipeDiameter, pipeNodeEntities
                        );
                        
                        abstractSegmentEntities.Add(newPipeSegmentEntity);
                        connectedSegments.Add(newPipeSegmentEntity);
                    }

                    XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(coordinates, connectedSegments, out _);
                    double pipeRadius = Math.Max(connectedSegments[0].Diameter, connectedSegments[1].Diameter);
                    
                    IfcCadBendEntity bendEntity = new IfcCadBendEntity(name, tag, objectMatrix3D, length, angle, bendRadius, pipeRadius);
                    bendEntity.PropertySets.AddRange(propertySets);
                    bendEntity.ConnectedEntities.AddRange(connectedSegments);
                    bendEntities.Add(bendEntity);
                    
                    foreach (IfcAbstractSegmentEntity connectedSegment in connectedSegments)
                    {
                        IfcNodeEntity bendNodeEntity = bendEntity.NodeEntity;
                        IndexedResult<IfcNodeEntity> connectedNodeEntity = connectedSegment.NodeEntities.GetNearestNode(bendNodeEntity);
                        connectedSegment.Clip(bendNodeEntity, -clipLength);
                        connectedSegment.NodeEntities[connectedNodeEntity.Index] = bendNodeEntity;
                    }
                    
                    logger.Info($"Created {bendEntity}, ID: {bendElement.GlobalId}");
                } 
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    logger.Error($"Failed to create bend entity with ID: {bendElement.GlobalId}");
                }
            }
            logger.Info($"Found {bendEntities.Count} {nameof(IfcCadBendEntity)} objects");

            return bendEntities;
        }

        private IEnumerable<IfcWeldedTeeEntity> CreateWeldedTees(IReadOnlyCollection<IfcElement> tees, ICollection<IfcPipeSegmentEntity> abstractSegmentEntities)
        {
            Logger logger = Logger.GetInstance();

            logger.Info($"Searching {nameof(IfcWeldedTeeEntity)} objects");
            List<IfcWeldedTeeEntity> weldedTeeEntities = new List<IfcWeldedTeeEntity>(capacity: tees.Count);
            foreach (IfcElement teeElement in tees)
            {
                try
                {
                    IfcLabel name = teeElement.Name ?? new IfcLabel("");
                    IfcIdentifier tag = teeElement.Tag ?? new IfcIdentifier("");

                    IPropertySet[] propertySets = teeElement.GetPropertySets().ToArray();
                    
                    AVEVA_EntityParameters? avevaEntityParameters = propertySets.OfType<AVEVA_EntityParameters>().FirstOrDefault();
                    if (avevaEntityParameters == null)
                        throw new Exception("Bend does not have AVEVA_EntityParameters property set.");
                    
                    AVEVA_Pset? avevaPset = propertySets.OfType<AVEVA_Pset>().FirstOrDefault();
                    if (avevaPset == null)
                        throw new Exception("Bend does not have AVEVA_Pset property set.");
                    
                    IfcExtrudedAreaSolid[] extrudedAreaSolids = teeElement
                        .GetRepresentationItems()
                        .OfType<IfcExtrudedAreaSolid>()
                        .ToArray();
                    XbimVector3D[] boundPoints = extrudedAreaSolids
                        .SelectMany(solid => solid.GetBoundPoints())
                        .ToArray();
                    boundPoints = boundPoints
                        .Select(point => point * _lengthUnit.Power)
                        .ToArray();
                    
                    IfcAbstractSegmentEntity[] connectedSegments = abstractSegmentEntities
                        .GetConnectedSegments(boundPoints)
                        .ToArray();

                    XbimVector3D coordinates = avevaPset.GetPosition() * _lengthUnit.Power;
                    XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(coordinates, connectedSegments,
                        out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes
                    );
                    
                    double branchDiameter = Math.Max(branchPipes[0].Diameter, branchPipes[1].Diameter);
                    double headDiameter = headPipe.Diameter;
                    
                    IfcExtrudedAreaSolid headExtrudedAreaSolid = extrudedAreaSolids
                        .First(solid => solid.Position.ToObjectMatrix3D().Forward.IsParallel(headPipe.ObjectMatrix3D.Value.Forward));
                    IfcExtrudedAreaSolid branchExtrudedAreaSolid = extrudedAreaSolids
                        .First(solid => solid.Position.ToObjectMatrix3D().Forward.IsNormal(headPipe.ObjectMatrix3D.Value.Forward, 1e-2));

                    double length = branchExtrudedAreaSolid.GetLength() * _lengthUnit.Power;
                    double height = headExtrudedAreaSolid.GetLength() * _lengthUnit.Power;
                    
                    IfcWeldedTeeEntity weldedTeeEntity = new IfcWeldedTeeEntity(name, tag, objectMatrix3D, length, branchDiameter, headDiameter, height, angle);
                    weldedTeeEntity.PropertySets.AddRange(propertySets);
                    weldedTeeEntity.ConnectedEntities.AddRange(connectedSegments);
                    weldedTeeEntities.Add(weldedTeeEntity);

                    IfcNodeEntity weldedTeeNode = weldedTeeEntity.NodeEntity;
                    foreach (IfcAbstractSegmentEntity branchPipe in branchPipes)
                    {
                        branchPipe.Clip(weldedTeeNode, -length / 2);
                        IndexedResult<IfcNodeEntity> branchNodeResult = branchPipe.NodeEntities.GetNearestNode(weldedTeeNode);
                        branchPipe.NodeEntities[branchNodeResult.Index] = weldedTeeNode;
                    }

                    headPipe.Clip(weldedTeeNode, -height);
                    IndexedResult<IfcNodeEntity> headNodeResult = headPipe.NodeEntities.GetNearestNode(weldedTeeNode);
                    headPipe.NodeEntities[headNodeResult.Index] = weldedTeeNode;
                    
                    logger.Info($"Created {weldedTeeEntity}, ID: {teeElement.GlobalId}");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    logger.Error($"Failed to create tee entity with ID: {teeElement.GlobalId}");
                }
            }
            
            logger.Info($"Found {weldedTeeEntities.Count} {nameof(IfcWeldedTeeEntity)} objects");
            
            return weldedTeeEntities.ToArray();
        }

        private IEnumerable<IfcAbstractReducerEntity> CreateReducers(IReadOnlyCollection<IfcElement> reducers, ICollection<IfcPipeSegmentEntity> abstractSegmentEntities)
        {
            Logger logger = Logger.GetInstance();

            logger.Info($"Searching {nameof(IfcAbstractReducerEntity)} objects");
            List<IfcAbstractReducerEntity> reducerEntities = new List<IfcAbstractReducerEntity>(capacity: reducers.Count);
            foreach (IfcElement reducerElement in reducers)
            {
                try
                {
                    IfcLabel name = reducerElement.Name ?? new IfcLabel("");
                    IfcIdentifier tag = reducerElement.Tag ?? new IfcIdentifier("");
                    
                    IPropertySet[] propertySets = reducerElement.GetPropertySets().ToArray();
                    
                    AVEVA_EntityParameters? avevaEntityParameters = propertySets.OfType<AVEVA_EntityParameters>().FirstOrDefault();
                    if (avevaEntityParameters == null)
                        throw new Exception("Bend does not have AVEVA_EntityParameters property set.");
                    
                    AVEVA_Pset? avevaPset = propertySets.OfType<AVEVA_Pset>().FirstOrDefault();
                    if (avevaPset == null)
                        throw new Exception("Bend does not have AVEVA_Pset property set.");
                    
                    IfcTriangulatedFaceSet? faceSet = reducerElement
                        .GetRepresentationItems()
                        .OfType<IfcTriangulatedFaceSet>()
                        .FirstOrDefault();
                    if (faceSet == null)
                        throw new NullReferenceException("Reducer does not have IfcTriangulatedFaceSet representation.");
                    
                    ReducerProperties reducerProperties = faceSet.GetReducerProperties(avevaPset);
                    double[] diameters = reducerProperties.Radiuses
                        .Select(radius => radius * _lengthUnit.Power * 2)
                        .ToArray();
                    XbimVector3D[] boundPoints = reducerProperties.BoundPoints
                        .Select(point => point * _lengthUnit.Power)
                        .ToArray();
                    double length = reducerProperties.Length * _lengthUnit.Power;
                    XbimVector3D coordinates = boundPoints[1];

                    XbimVector3D axisDisplacement = reducerProperties.AxisDisplacement * _lengthUnit.Power;
                    XbimMatrix3D reducerMatrix = reducerProperties.ObjectMatrix3D;
                    XbimVector3D reducerForward = reducerMatrix.Forward;
                    
                    List<IfcAbstractSegmentEntity> connectedSegments = abstractSegmentEntities
                        .GetConnectedSegments(boundPoints, 1e-2)
                        .ToList();

                    if (connectedSegments.Count is 0)
                        throw new NullReferenceException("Cannot find connected segments for reducer.");
                    if (connectedSegments.Count is 1)
                    {
                        IfcLabel pipeName = new IfcLabel($"{name}_AutoPipe");
                        IfcIdentifier pipeTag = new IfcIdentifier($"{tag}_AutoPipe");

                        IndexedResult<XbimVector3D> indexedNewPipePoint = boundPoints
                            .Select((point, index) => new IndexedResult<XbimVector3D>(point, index))
                            .First(result => !connectedSegments[0].IsContainPoint(result.Object));
                        
                        XbimVector3D direction = (indexedNewPipePoint.Object - coordinates).Normalized();
                        direction = (direction.DotProduct(reducerForward) * reducerForward).Normalized();
                        XbimMatrix3D pipeMatrix3D = MatrixExtensions.CreateWorld(coordinates, direction);

                        double pipeDiameter = diameters[indexedNewPipePoint.Index];
                        double pipeLength = coordinates.IsEqualFixed(reducerProperties.Center) ? 0.0 : length;
                        IfcPipeSegmentEntity newPipeSegmentEntity = new IfcPipeSegmentEntity(
                            pipeName, pipeTag, pipeMatrix3D, pipeLength, pipeDiameter
                        );
                        
                        abstractSegmentEntities.Add(newPipeSegmentEntity);
                        connectedSegments.Add(newPipeSegmentEntity);
                    }
                    
                    XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(coordinates, connectedSegments, out double angle);
                    
                    IfcAbstractReducerEntity reducerEntity;
                    if (axisDisplacement.IsEqualFixed(XbimVector3D.Zero, 1e-5D))
                    {
                        reducerEntity = new IfcVertexReducerConcentricEntity(
                            name, tag, objectMatrix3D, length, diameters, 16
                        );
                    }
                    else
                    {
                        reducerEntity = new IfcVertexReducerEccentricEntity(
                            name, tag, objectMatrix3D, length, 0, diameters, 16
                        );
                    }
                    
                    reducerEntity.PropertySets.AddRange(propertySets);
                    reducerEntity.ConnectedEntities.AddRange(connectedSegments);
                    reducerEntities.Add(reducerEntity);
                    
                    IfcNodeEntity reducerNode = reducerEntity.NodeEntity;
                    foreach (IfcAbstractSegmentEntity connectedSegment in connectedSegments)
                    {
                        double clipLength = connectedSegment.IsContainPoint(boundPoints[0]) ? -length : 0.0;
                        connectedSegment.Clip(reducerNode, clipLength);
                        IndexedResult<IfcNodeEntity> connectedNodeResult = connectedSegment.NodeEntities.GetNearestNode(reducerNode);
                        connectedSegment.NodeEntities[connectedNodeResult.Index] = reducerNode;
                    }
                    
                    logger.Info($"Created {reducerEntity}, ID: {reducerElement.GlobalId}");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    logger.Error($"Failed to create reducer entity with ID: {reducerElement.GlobalId}");
                }
            }
            
            logger.Info($"Found {reducerEntities.Count} {nameof(IfcAbstractReducerEntity)} objects");

            return reducerEntities;
        }
        
        private IEnumerable<IfcAbstractAnchorEntity> CreateAnchors(IReadOnlyCollection<IfcElement> anchors, ICollection<IfcPipeSegmentEntity> abstractSegmentEntities)
        {
            Logger logger = Logger.GetInstance();
            
            logger.Info($"Searching {nameof(IfcAbstractAnchorEntity)} objects");
            List<IfcAbstractAnchorEntity> abstractAnchorEntities = new List<IfcAbstractAnchorEntity>(capacity: anchors.Count);
            foreach (IfcElement anchorElement in anchors)
            {
                try
                {
                    IfcLabel name = anchorElement.Name ?? new IfcLabel("");
                    IfcIdentifier tag = anchorElement.Tag ?? new IfcIdentifier("");
                    
                    IPropertySet[] propertySets = anchorElement.GetPropertySets().ToArray();
                    
                    AVEVA_EntityParameters? avevaEntityParameters = propertySets.OfType<AVEVA_EntityParameters>().FirstOrDefault();
                    if (avevaEntityParameters == null)
                        throw new Exception("Bend does not have AVEVA_EntityParameters property set.");
                    
                    AVEVA_Pset? avevaPset = propertySets.OfType<AVEVA_Pset>().FirstOrDefault();
                    if (avevaPset == null)
                        throw new Exception("Bend does not have AVEVA_Pset property set.");
                    
                    XbimVector3D coordinates = avevaPset.GetPosition() * _lengthUnit.Power;
                    IfcAbstractSegmentEntity? nearestSegment = abstractSegmentEntities.GetNearestSegments(coordinates, 1).FirstOrDefault();
                    if (nearestSegment == null)
                        throw new Exception("Cannot find anchor's connected segment");
                    
                    double divisionRate = (coordinates - nearestSegment.StartPoint).Length / nearestSegment.Length;
                    IfcPipeSegmentEntity newDividedSegment = nearestSegment.DividePipe(divisionRate);
                    abstractSegmentEntities.Add(newDividedSegment);

                    double diameter = nearestSegment.Diameter;
                    XbimMatrix3D supportObjectMatrix = StartToIfcPlacement.CreateStandardObjectMatrix(coordinates);
                    IfcHingedAnchorEntity anchorEntity = new IfcHingedAnchorEntity(name, tag, supportObjectMatrix, diameter, diameter * 2, 16);
                    nearestSegment.EndNode = anchorEntity.NodeEntity;
                    newDividedSegment.StartNode = anchorEntity.NodeEntity;

                    abstractAnchorEntities.Add(anchorEntity);
                    
                    logger.Info($"Created {anchorEntity}, ID: {anchorElement.GlobalId}");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    logger.Error($"Failed to create anchor entity with ID: {anchorElement.GlobalId}");
                }
            }
            
            logger.Info($"Found {abstractAnchorEntities.Count} {nameof(IfcAbstractAnchorEntity)} objects");

            return abstractAnchorEntities;
        }

        private IEnumerable<IfcVertexValveEntity> CreateValves(IReadOnlyCollection<IfcElement> valves, ICollection<IfcPipeSegmentEntity> abstractSegmentEntities)
        {
            Logger logger = Logger.GetInstance();
            
            logger.Info($"Searching {nameof(IfcVertexValveEntity)} objects");
            List<IfcVertexValveEntity> valveEntities = new List<IfcVertexValveEntity>(capacity: valves.Count);
            foreach (IfcElement valveElement in valves)
            {
                try
                {
                    IfcLabel name = valveElement.Name ?? new IfcLabel("");
                    IfcIdentifier tag = valveElement.Tag ?? new IfcIdentifier("");
                    
                    IPropertySet[] propertySets = valveElement.GetPropertySets().ToArray();
                    
                    AVEVA_EntityParameters? avevaEntityParameters = propertySets.OfType<AVEVA_EntityParameters>().FirstOrDefault();
                    if (avevaEntityParameters == null)
                        throw new Exception("Bend does not have AVEVA_EntityParameters property set.");
                    
                    AVEVA_Pset? avevaPset = propertySets.OfType<AVEVA_Pset>().FirstOrDefault();
                    if (avevaPset == null)
                        throw new Exception("Bend does not have AVEVA_Pset property set.");
                    
                    XbimVector3D coordinates = avevaPset.GetPosition() * _lengthUnit.Power;
                    IfcAbstractSegmentEntity[] nearestSegmentEntities = abstractSegmentEntities.GetNearestSegments(coordinates, 2).ToArray();
                    if (nearestSegmentEntities.Length < 2)
                        throw new Exception("Cannot find valve connected segments");
                    
                    XbimVector3D[] valvePoints = nearestSegmentEntities.Select(segment => segment.NodeEntities.GetNearestNode(coordinates).Object.ObjectMatrix3D.Translation).ToArray();
                    double length = (valvePoints[1] - valvePoints[0]).Length;
                    
                    double diameter = Math.Max(nearestSegmentEntities[0].Diameter, nearestSegmentEntities[1].Diameter);
                    XbimMatrix3D valveObjectMatrix = StartToIfcPlacement.CreateFittingObjectMatrix(coordinates, nearestSegmentEntities, out double angle);
                    IfcVertexValveEntity valveEntity = new IfcVertexValveEntity(name, tag, valveObjectMatrix, length, diameter, angle, 16);
                    IfcNodeEntity valveNode = valveEntity.NodeEntity;
                    
                    foreach (IfcAbstractSegmentEntity nearestSegment in nearestSegmentEntities)
                    {
                        nearestSegment.Clip(valveNode, -length / 2);
                        IndexedResult<IfcNodeEntity> branchNodeResult = nearestSegment.NodeEntities.GetNearestNode(valveNode);
                        nearestSegment.NodeEntities[branchNodeResult.Index] = valveNode;
                    }
                    
                    valveEntities.Add(valveEntity);
                    
                    logger.Info($"Created {valveEntity}, ID: {valveElement.GlobalId}");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    logger.Error($"Failed to create valve entity with ID: {valveElement.GlobalId}");
                }
            }
            
            logger.Info($"Found {valveEntities.Count} {nameof(IfcVertexValveEntity)} objects");

            return valveEntities;
        }

        private static IEnumerable<IfcElement> GetElementByType(IEnumerable<IfcProduct> products, IfcText type)
        {
            return products
                .Select(product => new { Product = product, PropertySets = product.GetPropertySets() })
                .Where(item =>
                {
                    AVEVA_EntityParameters? pset = item.PropertySets.OfType<AVEVA_EntityParameters>().FirstOrDefault();
                    return pset?.E3DType.Value == type;
                })
                .Select(item => item.Product)
                .Cast<IfcElement>()
                .ToArray();
        }
    }
}