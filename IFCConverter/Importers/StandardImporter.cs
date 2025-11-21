using System;
using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities;
using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Fittings.Vertex;
using IFC.Entities.Segments;
using IFC.Extensions;
using IFC.PropertySets;
using IFC.Tools;
using IFCConverter.Extensions.Entities;
using IFCConverter.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.SharedComponentElements;

namespace IFCConverter.Importers
{
    internal class StandardImporter : IImporter
    {
        public IEnumerable<IfcProduct> Products => _products;
        
        private readonly IfcSIUnit _LengthUnit;
        private readonly IfcProduct[] _products;
        
        public StandardImporter(IFCProject ifcProject)
        {
            _products = ifcProject.GetProducts().ToArray();
            _LengthUnit = ifcProject.LengthUnit;
        }

        public IEnumerable<IfcPipeSegmentEntity> CreateSegments()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IfcAbstractFittingEntity> CreateFittings(List<IfcPipeSegmentEntity> pipeSegmentEntities)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IfcAbstractAnchorEntity> CreateAnchors(List<IfcPipeSegmentEntity> pipeSegmentEntities)
        {
            throw new NotImplementedException();
        }

        // ReSharper disable once CoVariantArrayConversion
        private IfcElement[] GetBends(IfcProduct[] products)
        {
            return products
                .OfType<IfcPipeFitting>()
                .Where(fitting => fitting.PredefinedType == IfcPipeFittingTypeEnum.BEND)
                .ToArray();
        }

        // ReSharper disable once CoVariantArrayConversion
        private IfcElement[] GetTees(IfcProduct[] products)
        {
            return products
                .OfType<IfcPipeFitting>()
                .Where(fitting => fitting.PredefinedType == IfcPipeFittingTypeEnum.JUNCTION)
                .ToArray();
        }

        // ReSharper disable once CoVariantArrayConversion
        private IfcElement[] GetReducers(IfcProduct[] products)
        {
            return products
                .OfType<IfcPipeFitting>()
                .Where(fitting => fitting.PredefinedType == IfcPipeFittingTypeEnum.CONNECTOR)
                .ToArray();
        }
        
        // ReSharper disable once CoVariantArrayConversion
        private IfcElement[] GetAnchors(IfcProduct[] products)
        {
            return products
                .OfType<IfcDiscreteAccessory>()
                .ToArray();
        }

        // ReSharper disable once CoVariantArrayConversion
        private IfcElement[] GetValves(IfcProduct[] products)
        {
            return products
                .OfType<IfcValve>()
                .ToArray();
        }

        private IEnumerable<IfcPipeSegmentEntity> CreatePipeSegments(IReadOnlyList<IfcElement> pipes)
        {
            IfcPipeSegmentEntity[] pipeSegmentEntities = new IfcPipeSegmentEntity[pipes.Count];

            for (int i = 0; i < pipes.Count; i++)
            {
                IfcElement pipeSegment = pipes[i];
                IfcLabel name = pipeSegment.Name ?? new IfcLabel("");
                IfcIdentifier tag = pipeSegment.Tag ?? new IfcIdentifier("");
                XbimMatrix3D objectMatrix3D = pipeSegment.ObjectPlacement.ToObjectMatrix3D();

                IPropertySet[] propertySets = pipeSegment.GetPropertySets().ToArray();
                double length = GetPipeLength(propertySets);
                double diameter = GetPipeDiameter(propertySets);

                IfcPipeSegmentEntity pipeSegmentEntity = new IfcPipeSegmentEntity(name, tag, objectMatrix3D, length, diameter);
                pipeSegmentEntity.PropertySets.AddRange(propertySets);
                pipeSegmentEntities[i] = pipeSegmentEntity;
            }

            return pipeSegmentEntities;
        }

        private IfcCadBendEntity[] CreateBends(IReadOnlyList<IfcElement> bends, ICollection<IfcPipeSegmentEntity> abstractSegmentEntities)
        {
            IfcCadBendEntity[] bendEntities = new IfcCadBendEntity[bends.Count];
            for (int i = 0; i < bends.Count; i++)
            {
                IfcElement bend = bends[i];
                IfcLabel name = bend.Name ?? new IfcLabel("");
                IfcIdentifier tag = bend.Tag ?? new IfcIdentifier("");
                
                XbimMatrix3D objectMatrix3D = bend.ObjectPlacement.ToObjectMatrix3D();
                XbimVector3D coordinates = objectMatrix3D.Translation;
                
                IfcAbstractSegmentEntity[] nearestSegments = abstractSegmentEntities.GetNearestSegments(coordinates, 2).ToArray();

                IPropertySet[] propertySets = bend.GetPropertySets().ToArray();
                double angle = GetBendAngle(propertySets);
                double bendRadius = GetBendRadius(propertySets);
                double length = angle * bendRadius;
                double pipeDiameter = nearestSegments.Max(segment => segment.Diameter.Value);

                IfcCadBendEntity cadBendEntity = new IfcCadBendEntity(name, tag, objectMatrix3D, length, angle, bendRadius, pipeDiameter / 2);
                cadBendEntity.ConnectedEntities.AddRange(nearestSegments);
                cadBendEntity.PropertySets.AddRange(propertySets);
                
                double clipLength = bendRadius * Math.Tan(angle / 2);
                foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in nearestSegments)
                {
                    ifcAbstractSegmentEntity.Clip(cadBendEntity.NodeEntity, -clipLength);
                    IndexedResult<IfcNodeEntity> nearestNodeResult = ifcAbstractSegmentEntity.NodeEntities.GetNearestNode(cadBendEntity.NodeEntity);
                    ifcAbstractSegmentEntity.NodeEntities[nearestNodeResult.Index] = cadBendEntity.NodeEntity;
                }
                
                bendEntities[i] = cadBendEntity;
            }

            return bendEntities;
        }

        private IfcWeldedTeeEntity[] CreateWeldedTees(IfcElement[] tees, List<IfcPipeSegmentEntity> abstractSegmentEntities)
        {
            IfcWeldedTeeEntity[] weldedTeeEntities = new IfcWeldedTeeEntity[tees.Length];
            for (int i = 0; i < tees.Length; i++)
            {
                IfcElement tee = tees[i];
                IfcLabel name = tee.Name ?? new IfcLabel("");
                IfcIdentifier tag = tee.Tag ?? new IfcIdentifier("");
                
                XbimMatrix3D objectMatrix3D = tee.ObjectPlacement.ToObjectMatrix3D();
                XbimVector3D coordinates = objectMatrix3D.Translation;
                
                IfcAbstractSegmentEntity[] nearestSegments = abstractSegmentEntities.GetNearestSegments(coordinates, 3).ToArray();
                FilterTeeSegments(nearestSegments, coordinates, out IfcAbstractSegmentEntity[] branchPipes, out IfcAbstractSegmentEntity headPipe, out double angle);

                IPropertySet[] propertySets = tee.GetPropertySets().ToArray();
                double length = GetTeeLength(propertySets);
                double height = GetTeeHeight(propertySets);
                double branchDiameter = Math.Max(branchPipes[0].Diameter, branchPipes[1].Diameter);
                double headDiameter = headPipe.Diameter;
                
                IfcWeldedTeeEntity weldedTeeEntity = new IfcWeldedTeeEntity(name, tag, objectMatrix3D, length, branchDiameter, headDiameter, height, angle);
                weldedTeeEntity.ConnectedEntities.AddRange(nearestSegments);
                weldedTeeEntity.PropertySets.AddRange(propertySets);
                
                double headClipLength = height + branchDiameter / 2;
                double branchClipLength = length / 2;
                
                headPipe.Clip(weldedTeeEntity.NodeEntity, -headClipLength);
                IndexedResult<IfcNodeEntity> headPipeNearestNodeResult = headPipe.NodeEntities.GetNearestNode(weldedTeeEntity.NodeEntity);
                headPipe.NodeEntities[headPipeNearestNodeResult.Index] = weldedTeeEntity.NodeEntity;
                
                foreach (IfcAbstractSegmentEntity branchPipe in branchPipes)
                {
                    branchPipe.Clip(weldedTeeEntity.NodeEntity, -branchClipLength);
                    IndexedResult<IfcNodeEntity> branchPipeNearestNodeResult = branchPipe.NodeEntities.GetNearestNode(weldedTeeEntity.NodeEntity);
                    branchPipe.NodeEntities[branchPipeNearestNodeResult.Index] = weldedTeeEntity.NodeEntity;
                }

                weldedTeeEntities[i] = weldedTeeEntity;
            }

            return weldedTeeEntities;
        }

        private IfcVertexReducerConcentricEntity[] CreateConcentricReducers(IfcElement[] reducers, List<IfcPipeSegmentEntity> abstractSegmentEntities)
        {
            IfcVertexReducerConcentricEntity[] reducerConcentricEntities = new IfcVertexReducerConcentricEntity[reducers.Length];
            for (int i = 0; i < reducers.Length; i++)
            {
                IfcElement reducer = reducers[i];
                IfcLabel name = reducer.Name ?? new IfcLabel("");
                IfcIdentifier tag = reducer.Tag ?? new IfcIdentifier("");
                
                XbimMatrix3D objectMatrix3D = reducer.ObjectPlacement.ToObjectMatrix3D();
                XbimVector3D coordinates = objectMatrix3D.Translation;
                IfcAbstractSegmentEntity[] nearestSegments = abstractSegmentEntities.GetNearestSegments(coordinates, 2).ToArray();
                
                IPropertySet[] propertySets = reducer.GetPropertySets().ToArray();
                double length = GetPipeLength(propertySets);
                double smallDiameter = nearestSegments.Min(segment => segment.Diameter.Value);
                double largeDiameter = nearestSegments.Max(segment => segment.Diameter.Value);
                
                double[] diameters = nearestSegments
                    .Select(segment => segment.Diameter.Value)
                    .OrderBy(diameter => diameter)
                    .ToArray();
                
                IfcVertexReducerConcentricEntity reducerConcentricEntity = new IfcVertexReducerConcentricEntity(name, tag, objectMatrix3D, length, diameters, 16);
                reducerConcentricEntity.ConnectedEntities.AddRange(nearestSegments);
                reducerConcentricEntity.PropertySets.AddRange(propertySets);
                
                double clipLength = length / 2;
                foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in nearestSegments)
                {
                    ifcAbstractSegmentEntity.Clip(reducerConcentricEntity.NodeEntity, -clipLength);
                    IndexedResult<IfcNodeEntity> nearestNodeResult = ifcAbstractSegmentEntity.NodeEntities.GetNearestNode(reducerConcentricEntity.NodeEntity);
                    ifcAbstractSegmentEntity.NodeEntities[nearestNodeResult.Index] = reducerConcentricEntity.NodeEntity;
                }
            }

            return reducerConcentricEntities;
        }

        private static void FilterTeeSegments(IfcAbstractSegmentEntity[] segmentEntities, XbimVector3D coordinates, out IfcAbstractSegmentEntity[] branchPipes, out IfcAbstractSegmentEntity headPipe, out double angle)
        {
            branchPipes = new IfcAbstractSegmentEntity[2];
            headPipe = null;
            for (int i = 0; i < segmentEntities.Length; i++)
            {
                for (int j = i + 1; j < segmentEntities.Length; j++)
                {
                    XbimVector3D firstPipeDir = segmentEntities[i].ObjectMatrix3D.Value.Forward;
                    XbimVector3D secondPipeDir = segmentEntities[j].ObjectMatrix3D.Value.Forward;
                    
                    if (!firstPipeDir.IsParallel(secondPipeDir, 1e-3))
                        continue;
                    branchPipes[0] = segmentEntities[i];
                    branchPipes[1] = segmentEntities[j];
                    headPipe = segmentEntities[segmentEntities.Length - (i + j)];
                }
            }
            if (headPipe == null)
                throw new NullReferenceException("Cannot find head pipe");
            if (branchPipes == null)
                throw new NullReferenceException("Cannot find branch pipes");
            
            XbimVector3D branchDirection = IfcAxis.GetPipeDirectionFromNode(branchPipes[1], coordinates);
            XbimVector3D headDirection = IfcAxis.GetPipeDirectionFromNode(headPipe, coordinates).Normalized();
            
            angle = branchDirection.Angle(headDirection);
        }

        private static double GetTeeLength(IEnumerable<IPropertySet> propertySets)
        {
            Pset_Start psetStart = propertySets.OfType<Pset_Start>().First();
            return Pset_Start.GetDoublePropertyValue(psetStart.Data[nameof(StartTeeEntity.HeaderLength)]);
        }

        private static double GetTeeHeight(IEnumerable<IPropertySet> propertySets)
        {
            Pset_Start psetStart = propertySets.OfType<Pset_Start>().First();
            return Pset_Start.GetDoublePropertyValue(psetStart.Data[nameof(StartTeeEntity.CrotchHeight)]);
        }

        private static double GetBendAngle(IEnumerable<IPropertySet> propertySets)
        {
            Pset_PipeFittingTypeBend psetPipeFittingTypeBend = propertySets.OfType<Pset_PipeFittingTypeBend>().First();
            return psetPipeFittingTypeBend.BendAngle.Value;
        }

        private static double GetBendRadius(IEnumerable<IPropertySet> propertySets)
        {
            Pset_PipeFittingTypeBend psetPipeFittingTypeBend = propertySets.OfType<Pset_PipeFittingTypeBend>().First();
            return psetPipeFittingTypeBend.BendRadius.Value;
        }

        private static double GetPipeLength(IEnumerable<IPropertySet> propertySets)
        {
            Qto_PipeSegmentBaseQuantities qtoPipeSegmentBaseQuantities = propertySets.OfType<Qto_PipeSegmentBaseQuantities>().First();
            return qtoPipeSegmentBaseQuantities.Length.Value;
        }

        private static double GetPipeDiameter(IEnumerable<IPropertySet> propertySets)
        {
            Pset_PipeSegmentTypeCommon psetPipeSegmentTypeCommon = propertySets.OfType<Pset_PipeSegmentTypeCommon>().First();
            return psetPipeSegmentTypeCommon.OuterDiameter.Value;
        }
    }
}