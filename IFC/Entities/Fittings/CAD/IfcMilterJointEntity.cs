using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract;
using IFC.Tools;
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
        public override double Length { get; protected set; }
        public double Depth { get; }
        
        private readonly StartBendEntity _bendEntity;
        private IfcPipeFitting _pipeFitting;

        public IfcMilterJointEntity(StartBendEntity bendEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegments)
            : base(bendEntity, ifcNodeEntity, abstractSegments)
        {
            _bendEntity = bendEntity;
            Depth = Math.Min(abstractSegments[0].Length, abstractSegments[1].Length) * 0.1;
            Length = 2 * Depth;
        }
    
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointObjectPlacement(model, ObjectMatrix3D);
            
            List<IfcRepresentationItem> ifcRepresentationItems = new List<IfcRepresentationItem>();
            for (int i = 0; i < AbstractSegmentEntities.Length; i++)
            {
                ifcRepresentationItems.Add(CreateExtrudedAreaSolid(model, AbstractSegmentEntities[i], 0));
                AbstractSegmentEntities[i].Clip(NodeEntity, Depth);
            }

            IfcExtrudedAreaSolid[] segments = AbstractSegmentEntities.Select(item => CreateExtrudedAreaSolid(model, item, Depth)).ToArray();
            ifcRepresentationItems.Add(IfcGeometry.CreateBooleanResult(model, segments[0], segments[1], IfcBooleanOperator.INTERSECTION));

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
                        weight.WeightValue = _bendEntity.Weight.SIProperty;
                    }));
                });
            });

            #endregion
        }
    }
}