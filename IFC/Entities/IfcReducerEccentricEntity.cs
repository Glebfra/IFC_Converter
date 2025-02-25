using System;
using System.Linq;
using IFC.Entities.Abstract;
using IFC.Tools;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Entities
{
    public class IfcReducerEccentricEntity : IfcAbstractEntity
    {
        protected override IfcIdentifier Tag { get; set; } = "Reducer Eccentric";
    
        private const int _numSegments = 32;
        private const double _angleStep = 2 * Math.PI / _numSegments;
        
        private readonly double _pipeDisplacement;
    
        private IfcPipeFitting? _pipeFitting { get; set; }
    
        private readonly StartReducerEntity _reducerEntity;
        private readonly IfcPipeEntity[] _pipeEntities;
        private readonly IfcNodeEntity _nodeEntity;

        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public double Length { get; }

        public IfcReducerEccentricEntity(StartReducerEntity reducerEntity, IfcNodeEntity nodeEntity, IfcPipeEntity[] pipeEntities)
        {
            _reducerEntity = reducerEntity;
            _nodeEntity = nodeEntity;
            _pipeEntities = pipeEntities.OrderBy(entity => entity.Diameter).ToArray();
            
            XbimVector3D coordinates = _nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = CalculateForwardVector();
            XbimVector3D up = CalculateUpVector();
            _pipeDisplacement = up.Length;
            up = up.Normalized();
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
            
            Length = _reducerEntity.LengthOfConicalPart;
        }
    
        public override IfcProduct CreateAndAdd(IModel model)
        {
            base.CreateAndAdd(model);

            IfcCartesianPoint[] lowerCircle = CreateCircle(model, _pipeEntities[0].Diameter / 2, 0, 0);
            IfcCartesianPoint[] upperCircle = CreateCircle(model, _pipeEntities[1].Diameter / 2, Length, _pipeDisplacement);
            IfcFacetedBrep facetedBrep = CreateFacetedBrep(model, lowerCircle, upperCircle);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, facetedBrep);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.ObjectPlacement = _localPlacement;
                fitting.Representation = shape;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.TRANSITION;
                fitting.Tag = Tag;
                fitting.Name = _reducerEntity.Name;
            });
            
            _pipeEntities[1].Clip(_nodeEntity, Length);
            
            MovePipe(_pipeEntities[1]);
            AddProperties(model, _pipeFitting);
            ConnectPorts(model);

            return _pipeFitting;
        }

        private XbimVector3D CalculateUpVector()
        {
            XbimVector3D coordinates = _nodeEntity.ObjectMatrix3D.Translation;
            IfcNodeEntity[] pipeNodeEntities = _pipeEntities[1].NodeEntities
                .OrderBy(entity => (entity.ObjectMatrix3D.Translation - coordinates).Length).ToArray();
            return (
                pipeNodeEntities[1].ObjectMatrix3D.Translation - 
                pipeNodeEntities[0].ObjectMatrix3D.Translation - 
                IfcAxis.GetDirectionToPipe(_pipeEntities[1], coordinates)
            );
        }

        private XbimVector3D CalculateForwardVector()
        {
            XbimVector3D coordinates = _nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D directionToPipe = IfcAxis.GetDirectionToPipe(_pipeEntities[1], coordinates);
            return directionToPipe.Normalized();
        }

        private IfcCartesianPoint[] CreateCircle(IModel model, double radius, double height, double displacement)
        {
            IfcCartesianPoint[] points = new IfcCartesianPoint[_numSegments];
            for (int i = 0; i < _numSegments; i++)
            {
                XbimVector3D point = new XbimVector3D(
                    radius * Math.Cos(_angleStep * i),
                    radius * Math.Sin(_angleStep * i) - displacement,
                    height
                );
                points[i] = IfcAxis.CreatePoint(model, point);
            }

            return points;
        }

        // TODO Кинуть в общий класс
        private static IfcFacetedBrep CreateFacetedBrep(IModel model, IfcCartesianPoint[] lowerPoints, IfcCartesianPoint[] upperPoints)
        {
            IfcFace[] faces = new IfcFace[_numSegments + 2];
            int facesIndex = 0;
            for (int i = 0; i < _numSegments; i++)
            {
                IfcCartesianPoint p1 = lowerPoints[i];
                IfcCartesianPoint p2 = lowerPoints[(i + 1) % _numSegments];
                IfcCartesianPoint p3 = upperPoints[(i + 1) % _numSegments];
                IfcCartesianPoint p4 = upperPoints[i];
                faces[facesIndex++] = IfcGeometry.CreateRectangleFace(model, p1, p2, p3, p4);
            }
            faces[facesIndex++] = IfcGeometry.CreatePolygonFace(model, lowerPoints);
            faces[facesIndex++] = IfcGeometry.CreatePolygonFace(model, upperPoints);

            return model.Instances.New<IfcFacetedBrep>(brep =>
            {
                brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
            });
        }
        
        private void MovePipe(IfcPipeEntity pipeEntity)
        {
            // Check if the first node is closer to the node entity than the second node
            // TODO Как-то назвать метод
            if ((pipeEntity.NodeEntities[0].ObjectMatrix3D.Translation - _nodeEntity.ObjectMatrix3D.Translation).Length <
                (pipeEntity.NodeEntities[1].ObjectMatrix3D.Translation - _nodeEntity.ObjectMatrix3D.Translation).Length)
            {
                pipeEntity.Coordinates += ObjectMatrix3D.Up * _pipeDisplacement;
            }
        }

        private IfcRelConnectsPorts ConnectPorts(IModel model)
        {
            var closestPorts = (
                from port in _pipeEntities.SelectMany(pipe => pipe.Ports)
                let distance = (port.ObjectPlacement.ToMatrix3D().Translation - ObjectMatrix3D.Translation).Length
                orderby distance
                select port
            ).Take(2).ToArray();

            return model.Instances.New<IfcRelConnectsPorts>(ports =>
            {
                ports.Name = $"{closestPorts[0].GlobalId}|{closestPorts[1].GlobalId}";
                ports.Description = "Flow";
                ports.RelatingPort = closestPorts[0];
                ports.RelatedPort = closestPorts[1];
                ports.RealizingElement = _pipeFitting;
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
                    foreach (var kvp in _reducerEntity.GetData())
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