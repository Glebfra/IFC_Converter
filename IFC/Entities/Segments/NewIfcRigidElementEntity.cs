using System;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.PropertySets;
using IFC.Tools;
using Start.Entities.Segments;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Segments
{
    public sealed class NewIfcRigidElementEntity : NewIfcAbstractRigidElementEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<double> Diameter { get; }

        public NewIfcRigidElementEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double diameter)
        {
            Colour.Value = IFC.Tools.Colour.FromHEX("009249");
            
            Name = new ActionProperty<IfcLabel>(name);
            Tag = new ActionProperty<IfcIdentifier>(tag);
            ObjectMatrix3D = new ActionProperty<XbimMatrix3D>(objectMatrix3D);
            Length = new ActionProperty<double>(length);
            Diameter = new ActionProperty<double>(diameter);
        }

        public static NewIfcRigidElementEntity CreateFromStart(StartRigidElementEntity rigidElement, IfcNodeEntity[] nodeEntities, NewIfcAbstractSegmentEntity[] segmentEntities)
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

            NewIfcRigidElementEntity ifcRigidElementEntity = new NewIfcRigidElementEntity(
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
}