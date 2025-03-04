using System;
using IFC.Entities.Abstract;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgServiceElements;
using Xbim.Ifc4.TopologyResource;
using IfcObjectPlacement = IFC.Tools.IfcObjectPlacement;

namespace IFC.Entities
{
    public class IfcValveEntity : IfcAbstractArmatureEntity
    {
        private const int _numSegments = 32;
        private const double _angleStep = 2 * Math.PI / _numSegments;
    
        protected override IfcIdentifier Tag { get; set; } = "Valve";
        protected override IfcPipeFitting? _pipeFitting { get; set; }

        private readonly StartArmatureEntity _armatureEntity;

        public readonly double Length;
        public readonly double Diameter;

        public IfcValveEntity(StartArmatureEntity armatureEntity, IfcNodeEntity nodeEntity, IfcPipeEntity[] pipeEntities)
            : base(nodeEntity, pipeEntities)
        {
            _armatureEntity = armatureEntity;
            Length = _armatureEntity.Length;
            Diameter = Math.Max(_pipeEntities[0].Diameter, _pipeEntities[1].Diameter) * 1.5;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IfcCartesianPoint[] firstCircle = CreateCircle(model, Diameter / 2, -Length / 2);
            IfcCartesianPoint[] secondCircle = CreateCircle(model, Diameter / 2, Length / 2, Angle);
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, XbimVector3D.Zero);
            IfcFacetedBrep lowerBrep = CreateFacetedBrep(model, firstCircle, topPoint);
            IfcFacetedBrep upperBrep = CreateFacetedBrep(model, secondCircle, topPoint);
        
            IfcBooleanResult result = model.Instances.New<IfcBooleanResult>(booleanResult =>
            {
                booleanResult.Operator = IfcBooleanOperator.UNION;
                booleanResult.FirstOperand = lowerBrep;
                booleanResult.SecondOperand = upperBrep;
            });
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, result);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Name = _armatureEntity.Name;
                fitting.Representation = shape;
                fitting.Tag = Tag;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            IfcDistributionPort[] ports = IfcPortConnection.GetPipeClosestPorts(ObjectMatrix3D, _pipeEntities);
            IfcPortConnection.ConnectPorts(model, ports, _pipeFitting);
        
            _pipeEntities[0].Clip(_nodeEntity, Length / 2);
            _pipeEntities[1].Clip(_nodeEntity, Length / 2);
        
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
    
        private IfcCartesianPoint[] CreateCircle(IModel model, double radius, double height, double angle = 0)
        {
            IfcCartesianPoint[] points = new IfcCartesianPoint[_numSegments];
            XbimMatrix3D My = MatrixExtensions.My(angle);
            for (int i = 0; i < _numSegments; i++)
            {
                XbimVector3D point = new XbimVector3D(
                    radius * Math.Cos(_angleStep * i),
                    radius * Math.Sin(_angleStep * i),
                    height
                );
                if (angle != 0)
                    point = XbimVector3D.Multiply(point, My);
                points[i] = IfcAxis.CreatePoint(model, point);
            }

            return points;
        }

        private static IfcFacetedBrep CreateFacetedBrep(IModel model, IfcCartesianPoint[] points, IfcCartesianPoint topPoint)
        {
            IfcFace[] faces = new IfcFace[_numSegments + 1];
            int facesIndex = 0;
            for (int i = 0; i < _numSegments; i++)
            {
                IfcCartesianPoint p1 = points[i];
                IfcCartesianPoint p2 = points[(i + 1) % _numSegments];
                IfcCartesianPoint p3 = topPoint;
                faces[facesIndex++] = IfcGeometry.CreateTriangleFace(model, p1, p2, p3);
            }
            faces[facesIndex++] = IfcGeometry.CreatePolygonFace(model, points);

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

            #region VALVE DEBUG

#if DEBUG
            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "VALVE DEBUG";
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "Angle";
                        value.NominalValue = new IfcText(Angle.ToString("F5"));
                    }));
                });
            });
#endif

            #endregion
        }
    }
}