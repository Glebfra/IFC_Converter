using System;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Interfaces;
using IFC.Extensions;
using IFC.PropertySets;
using IFC.Tools;
using Start.Entities.Segments;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Segments
{
    #if NEW
    
    public sealed class IfcRigidElementEntity : IfcAbstractRigidElementEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<double> Diameter { get; }

        public IfcRigidElementEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double diameter)
        {
            Colour.Value = IFC.Tools.Colour.FromHEX("009249");
            
            Name = new ActionProperty<IfcLabel>(name);
            Tag = new ActionProperty<IfcIdentifier>(tag);
            ObjectMatrix3D = new ActionProperty<XbimMatrix3D>(objectMatrix3D);
            Length = new ActionProperty<double>(length);
            Diameter = new ActionProperty<double>(diameter);
        }

        public static IfcRigidElementEntity CreateFromStart(StartRigidElementEntity rigidElement, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimVector3D coordinates = nodeEntities[0].ObjectMatrix3D.Translation;
            XbimVector3D direction = nodeEntities[1].ObjectMatrix3D.Translation - coordinates;
            
            double diameter = segmentEntities.Length switch
            {
                1 => segmentEntities[0].Diameter,
                2 => Math.Min(segmentEntities[0].Diameter, segmentEntities[1].Diameter),
                _ => 0.05
            };

            XbimVector3D forward = direction.Normalized();
            XbimMatrix3D objectMatrix3D = MatrixExtensions.CreateWorld(coordinates, forward);
            double length = direction.Length;

            IfcRigidElementEntity ifcRigidElementEntity = new IfcRigidElementEntity(
                rigidElement.Name,
                rigidElement.Type.ToString(),
                objectMatrix3D,
                length,
                diameter
            );
            ifcRigidElementEntity.PropertySets.Add(Pset_Start.CreateFromStart(rigidElement));
            ifcRigidElementEntity.PropertySets.Add(Pset_PipeSegmentTypeCommon.CreateFromStart(rigidElement));
            ifcRigidElementEntity.PropertySets.Add(Qto_PipeSegmentBaseQuantities.CreateFromStart(rigidElement));

            return ifcRigidElementEntity;
        }
    }
    
    #else
    
    public sealed class IfcRigidElementEntity : IfcAbstractRigidElementEntity, IIfcSegmentDependedEntity
    {
        public IfcAbstractSegmentEntity[] AbstractSegmentEntities { get; set; }
        
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public override Colour Colour { get; protected set; } = Colour.FromHEX("009249");
        public override double Length { get; protected set; }
        public override double Diameter { get; protected set; }
        public override ActionProperty<double> RealLength { get; protected set; }
        public override ActionProperty<double> OuterSurfaceArea { get; protected set; }
        public override ActionProperty<XbimVector3D> Coordinates { get; protected set; }
        public override XbimVector3D Direction { get; }

        public IfcRigidElementEntity(StartRigidElementEntity rigidElement, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(rigidElement, nodeEntities)
        {
            AbstractSegmentEntities = segmentEntities;
            
            Coordinates = new ActionProperty<XbimVector3D>(nodeEntities[0].ObjectMatrix3D.Translation);
            Direction = nodeEntities[1].ObjectMatrix3D.Translation - Coordinates.Value;
            RealLength = new ActionProperty<double>(Direction.Length);
            Length = Direction.Length;

            XbimVector3D forward = Direction.Normalized();
            ObjectMatrix3D = MatrixExtensions.CreateWorld(Coordinates.Value, forward);

            Diameter = segmentEntities.Length switch
            {
                1 => segmentEntities[0].Diameter,
                2 => Math.Min(segmentEntities[0].Diameter, segmentEntities[1].Diameter),
                _ => 0.05
            };
            if (Diameter > 0.05) Diameter = 0.05;
            OuterSurfaceArea = new ActionProperty<double>(MathExtensions.CalculateCylinderArea(Diameter / 2, RealLength.Value));
            
            RealLength.OnValueChange += () => OuterSurfaceArea.Value = MathExtensions.CalculateCylinderArea(Diameter / 2, RealLength.Value);
        }
    }

    #endif
}