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
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.QuantityResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgServiceElements;
using IfcObjectPlacement = IFC.Tools.IfcObjectPlacement;

namespace IFC.Entities
{
    public class IfcMilterJointEntity : IfcAbstractEntity
    {
        protected override IfcIdentifier Tag { get; set; } = "Milter Joint";
    
        private readonly StartBendEntity _bendEntity;
        private readonly IfcNodeEntity _ifcNodeEntity;
        private readonly IfcAbstractSegmentEntity[] _ifcAbstractSegments;

        private IfcPipeFitting _pipeFitting;

        private double _pipeAngle;

        public double Length => 2 * Depth;

        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public double Depth { get; }

        public IfcMilterJointEntity(StartBendEntity bendEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegments)
        {
            _bendEntity = bendEntity;
            _ifcNodeEntity = ifcNodeEntity;
            _ifcAbstractSegments = ifcAbstractSegments;

            XbimVector3D[] directionToPipes = CalculateDirectionToPipes();
            ObjectMatrix3D = CreateObjectMatrix(directionToPipes);
            
            _pipeAngle = CalculateBendAngle();
            Depth = Math.Min(ifcAbstractSegments[0].Length, ifcAbstractSegments[1].Length) * 0.1;
        }
    
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointObjectPlacement(model, ObjectMatrix3D);

            IfcRepresentationItem[] ifcRepresentationItems = new IfcRepresentationItem[_ifcAbstractSegments.Length + 1];
            for (int i = 0; i < _ifcAbstractSegments.Length; i++)
            {
                ifcRepresentationItems[i] = CreateExtrudedAreaSolid(model, _ifcAbstractSegments[i], 0);
                _ifcAbstractSegments[i].Clip(_ifcNodeEntity, Depth);
            }

            ifcRepresentationItems[_ifcAbstractSegments.Length] = model.Instances.New<IfcBooleanResult>(result =>
            {
                result.Operator = IfcBooleanOperator.INTERSECTION;
                result.FirstOperand = CreateExtrudedAreaSolid(model, _ifcAbstractSegments[0], Depth);
                result.SecondOperand = CreateExtrudedAreaSolid(model, _ifcAbstractSegments[1], Depth);
            });

            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, ifcRepresentationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _bendEntity.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
            });

            IfcDistributionPort[] ports = IfcPortConnection.GetPipeClosestPorts(ObjectMatrix3D, _ifcAbstractSegments);
            IfcPortConnection.ConnectPorts(model, ports, _pipeFitting);
            
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
        
        private XbimMatrix3D CreateObjectMatrix(XbimVector3D[] directionToPipes)
        {
            XbimVector3D coordinates = _ifcNodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D up = XbimVector3D.CrossProduct(directionToPipes[0] * -1, directionToPipes[1]).Normalized();
            XbimVector3D forward = directionToPipes[0] * -1;
            
            return XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }
        
        private XbimVector3D[] CalculateDirectionToPipes()
        {
            XbimVector3D coordinates = _ifcNodeEntity.ObjectMatrix3D.Translation;
            return _ifcAbstractSegments.Select(pipe => IfcAxis.GetDirectionToPipe(pipe, coordinates)).ToArray();
        }
        
        private double CalculateBendAngle()
        {
            XbimVector3D[] pipesDirection = _ifcAbstractSegments.Select(pipe => pipe.ObjectMatrix3D.Forward).ToArray();
            return pipesDirection[0].Angle(pipesDirection[1]);
        }

        private IfcExtrudedAreaSolid CreateExtrudedAreaSolid(IModel model, IfcAbstractSegmentEntity ifcAbstractSegment, double displacement)
        {
            XbimVector3D directionToPipe = IfcAxis.GetDirectionToPipe(ifcAbstractSegment, ObjectMatrix3D.Translation).Normalized();
            XbimVector3D localUp = ObjectMatrix3D.Up;
        
            IfcCartesianPoint point = IfcAxis.CreatePoint(model, XbimVector3D.Zero - directionToPipe * displacement);
            IfcDirection extrudedDirection = IfcAxis.CreateDirection(model, new XbimVector3D(0, 0, 1));

            IfcDirection ifcDirectionToPipe = IfcAxis.CreateDirection(model, directionToPipe);
            IfcDirection ifcLocalUp = IfcAxis.CreateDirection(model, localUp);
            IfcAxis2Placement3D placement3D = IfcAxis.CreateAxis2Placement3D(model, point, ifcDirectionToPipe, ifcLocalUp);
        
            IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(
                model,
                ifcAbstractSegment.Diameter / 2,
                XbimVector3D.Zero,
                new XbimVector3D(1, 0, 0)
            );

            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = extrudedDirection;
                solid.Depth = Depth;
                solid.Position = placement3D;
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
                    foreach (var kvp in _bendEntity.GetData())
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

            #region Qto_PipeFittingBaseQuantities

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcElementQuantity>(quantity =>
                {
                    quantity.Name = "Qto_PipeFittingBaseQuantities";
                    quantity.Quantities.Add(model.Instances.New<IfcQuantityLength>(length =>
                    {
                        length.Name = "Length";
                        length.LengthValue = Length;
                        length.Formula = "radius*angle; [angle]=rad, [radius]=metre";
                    }));
                    quantity.Quantities.Add(model.Instances.New<IfcQuantityWeight>(weight =>
                    {
                        weight.Name = "NetWeight";
                        weight.WeightValue = ValueConverter.ValueConverter.TfToKg(_bendEntity.Weight) * Length;
                    }));
                });
            });

            #endregion
        }
    }
}