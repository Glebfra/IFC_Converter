using System;
using System.Linq;
using IFC.Entities.Abstract;
using IFC.Tools;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Entities
{
    public class IfcFlangeEntity : IfcAbstractArmatureEntity
    {
        protected override IfcIdentifier Tag { get; set; } = "Flange";
    
        private const int _numSegments = 32;
        private const double _angleStep = 2 * Math.PI / _numSegments;
    
        private readonly StartArmatureEntity _armatureEntity;

        public readonly double Length;
        public readonly double[] Radiuses;

        protected override IfcPipeFitting? _pipeFitting { get; set; }

        public IfcFlangeEntity(StartArmatureEntity armatureEntity, IfcNodeEntity ifcNodeEntity, IfcPipeEntity[] ifcPipeEntities)
            : base(ifcNodeEntity, ifcPipeEntities)
        {
            _armatureEntity = armatureEntity;
            Length = _armatureEntity.Length;
            Radiuses = _pipeEntities.Select(entity => entity.Diameter / 2).ToArray();
        }
    
        public override IfcProduct CreateAndAdd(IModel model)
        {
            base.CreateAndAdd(model);
        
            IfcCartesianPoint[] firstCircleConnection = CreateCircle(model, Radiuses[0], -0.5 * Length);
            IfcCartesianPoint[] firstCircleExtension = CreateCircle(model, Radiuses[0] * 1.1, -0.3 * Length);
            IfcCartesianPoint[] firstCircleStartFlange = CreateCircle(model, Radiuses[0] * 1.5, -0.3 * Length);
            IfcCartesianPoint[] firstCircleEndFlange = CreateCircle(model, Radiuses[0] * 1.5, -0.1 * Length);
        
            IfcCartesianPoint[] secondCircleConnection = CreateCircle(model, Radiuses[1], 0.5 * Length);
            IfcCartesianPoint[] secondCircleExtension = CreateCircle(model, Radiuses[1] * 1.1, 0.3 * Length);
            IfcCartesianPoint[] secondCircleStartFlange = CreateCircle(model, Radiuses[1] * 1.5, 0.3 * Length);
            IfcCartesianPoint[] secondCircleEndFlange = CreateCircle(model, Radiuses[1] * 1.5, 0.1 * Length);

            IfcFacetedBrep[] facetedBreps = new IfcFacetedBrep[6];
            facetedBreps[0] = CreateFacetedBrep(model, firstCircleConnection, firstCircleExtension);
            facetedBreps[1] = CreateFacetedBrep(model, firstCircleExtension, firstCircleStartFlange);
            facetedBreps[2] = CreateFacetedBrep(model, firstCircleStartFlange, firstCircleEndFlange);
            facetedBreps[3] = CreateFacetedBrep(model, secondCircleConnection, secondCircleExtension);
            facetedBreps[4] = CreateFacetedBrep(model, secondCircleExtension, secondCircleStartFlange);
            facetedBreps[5] = CreateFacetedBrep(model, secondCircleStartFlange, secondCircleEndFlange);
        
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, facetedBreps);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Name = _armatureEntity.Name;
                fitting.Tag = Tag;
                fitting.ObjectPlacement = _localPlacement;
                fitting.Representation = shape;
            });
            AddProperties(model, _pipeFitting);
            _pipeEntities[0].Clip(_nodeEntity, 0.5 * Length);
            _pipeEntities[1].Clip(_nodeEntity, 0.5 * Length);
        
            return _pipeFitting;
        }
    
        private IfcCartesianPoint[] CreateCircle(IModel model, double radius, double height)
        {
            IfcCartesianPoint[] points = new IfcCartesianPoint[_numSegments];
            for (int i = 0; i < _numSegments; i++)
            {
                XbimVector3D point = new XbimVector3D(
                    radius * Math.Cos(_angleStep * i),
                    radius * Math.Sin(_angleStep * i),
                    height
                );
                points[i] = IfcAxis.CreatePoint(model, point);
            }

            return points;
        }
    
        private static IfcFacetedBrep CreateFacetedBrep(IModel model, IfcCartesianPoint[] firstPoints, IfcCartesianPoint[] secondPoints)
        {
            IfcFace[] faces = new IfcFace[_numSegments + 2];
            int facesIndex = 0;
            for (int i = 0; i < _numSegments; i++)
            {
                IfcCartesianPoint p1 = firstPoints[i];
                IfcCartesianPoint p2 = firstPoints[(i + 1) % _numSegments];
                IfcCartesianPoint p3 = secondPoints[(i + 1) % _numSegments];
                IfcCartesianPoint p4 = secondPoints[i];
                faces[facesIndex++] = IfcGeometry.CreateRectangleFace(model, p1, p2, p3, p4);
            }
            faces[facesIndex++] = IfcGeometry.CreatePolygonFace(model, firstPoints);
            faces[facesIndex++] = IfcGeometry.CreatePolygonFace(model, secondPoints);

            return model.Instances.New<IfcFacetedBrep>(brep =>
            {
                brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
            });
        }

        protected override void AddProperties(IModel model, IfcProduct product)
        {
            base.AddProperties(model, product);
        
            #region Pset_PipeFittingTypeStart
        
            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "Pset_PipeFittingTypeStart";
                    foreach (var kvp in _armatureEntity.GetData())
                    {
                        set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                        {
                            value.Name = kvp.Key;
                            value.NominalValue = new IfcText(kvp.Value);
                        }));
                    }
                });
            });

            #endregion
        }
    }
}