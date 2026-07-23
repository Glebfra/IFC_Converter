using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Interfaces;
using Utils;
using MatrixExtensions = Utils.MatrixExtensions;

namespace Start.Augmenters
{
    internal sealed class StartReducerSegmentMoverAugmenter : StartAbstractAugmenter<StartReducerEccentricEntity>
    {
        public override void AugmentTyped(StartReducerEccentricEntity entity, IEnumerable<IStartEntity> otherEntities)
        {
            IStartSegmentEntity minDiameterSegmentEntity = entity.SegmentWithMinDiameter;
            IStartSegmentEntity maxDiameterSegmentEntity = entity.SegmentWithMaxDiameter;
            
            double angle = entity.AngleBetweenEccentricityVectorAndZmAxis.SIProperty;
            Matrix<double> minDiameterSegmentMatrix = minDiameterSegmentEntity.TransformationMatrix;
            Matrix<double> rotationMatrix = MatrixExtensions.CreateRotationAroundZ(angle);
            Matrix<double> rotatedMinDiameterSegmentMatrix = rotationMatrix * minDiameterSegmentMatrix;
            
            Vector<double> displacement = rotatedMinDiameterSegmentMatrix.GetUp() * (
                maxDiameterSegmentEntity.Diameter.SIProperty - minDiameterSegmentEntity.Diameter.SIProperty
            ) / 2;
            
            if (maxDiameterSegmentEntity.IsStartPosition(entity.Position))
                maxDiameterSegmentEntity.StartPosition += displacement;
        }
    }
}