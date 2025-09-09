using IFC.PropertySets;
using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.QuantityResource;

namespace IFCConverter.Extensions.PropertySets
{
    internal static class Qto_PipeSegmentBaseQuantitiesExtensions
    {
        public static Qto_PipeSegmentBaseQuantities CreateFromStart(StartAbstractSegmentEntity abstractSegmentEntity)
        {
            Qto_PipeSegmentBaseQuantities qto = new Qto_PipeSegmentBaseQuantities();
            qto.NetWeight = new ActionProperty<IfcMassMeasure>(abstractSegmentEntity.PipeUnitWeight.SIProperty);
            
            XbimVector3D projection = new XbimVector3D(
                abstractSegmentEntity.ProjectionAlongOXAxis.SIProperty,
                abstractSegmentEntity.ProjectionAlongOYAxis.SIProperty,
                abstractSegmentEntity.ProjectionAlongOZAxis.SIProperty
            );
            qto.Length = new ActionProperty<IfcLengthMeasure>(projection.Length);
            return qto;
        }
        
        public static Qto_PipeSegmentBaseQuantities CreateFromQuantitySet(IIfcElementQuantity elementQuantity)
        {
            Qto_PipeSegmentBaseQuantities qto = new Qto_PipeSegmentBaseQuantities();
            foreach (IIfcPhysicalQuantity quantity in elementQuantity.Quantities)
            {
                switch (quantity.Name)
                {
                    case nameof(qto.Length):
                        qto.Length = ((IfcQuantityLength)quantity).LengthValue;
                        break;
                    case nameof(qto.GrossCrossSectionArea):
                        qto.GrossCrossSectionArea = ((IfcQuantityArea)quantity).AreaValue;
                        break;
                    case nameof(qto.NetCrossSectionArea):
                        qto.NetCrossSectionArea = ((IfcQuantityArea)quantity).AreaValue;
                        break;
                    case nameof(qto.OuterSurfaceArea):
                        qto.OuterSurfaceArea = ((IfcQuantityArea)quantity).AreaValue;
                        break;
                    case nameof(qto.GrossWeight):
                        qto.GrossWeight = ((IfcQuantityWeight)quantity).WeightValue;
                        break;
                    case nameof(qto.NetWeight):
                        qto.NetWeight = ((IfcQuantityWeight)quantity).WeightValue;
                        break;
                }
            }

            return qto;
        }
    }
}