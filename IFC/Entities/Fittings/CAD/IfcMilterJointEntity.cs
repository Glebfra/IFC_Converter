using System;
using IFC.Entities.Abstract;
using IFC.Tools;
using Start.API;
using Start.Entities;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.QuantityResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcMilterJointEntity : IfcAbstractFittingEntity
    {
        public double Length => 2 * Depth;
        public double Depth { get; }
        
        private readonly StartBendEntity _bendEntity;
        private IfcPipeFitting _pipeFitting;

        public IfcMilterJointEntity(StartBendEntity bendEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegments)
            : base(bendEntity, ifcNodeEntity, ifcAbstractSegments)
        {
            _bendEntity = bendEntity;
            Depth = Math.Min(ifcAbstractSegments[0].Length, ifcAbstractSegments[1].Length) * 0.1;
        }
    
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointObjectPlacement(model, ObjectMatrix3D);

            IfcRepresentationItem[] ifcRepresentationItems = new IfcRepresentationItem[_IfcAbstractSegmentEntities.Length + 1];
            for (int i = 0; i < _IfcAbstractSegmentEntities.Length; i++)
            {
                ifcRepresentationItems[i] = CreateExtrudedAreaSolid(model, _IfcAbstractSegmentEntities[i], 0);
                _IfcAbstractSegmentEntities[i].Clip(NodeEntity, Depth);
            }

            ifcRepresentationItems[_IfcAbstractSegmentEntities.Length] = model.Instances.New<IfcBooleanResult>(result =>
            {
                result.Operator = IfcBooleanOperator.INTERSECTION;
                result.FirstOperand = CreateExtrudedAreaSolid(model, _IfcAbstractSegmentEntities[0], Depth);
                result.SecondOperand = CreateExtrudedAreaSolid(model, _IfcAbstractSegmentEntities[1], Depth);
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
            
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
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
                Diameter / 2,
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