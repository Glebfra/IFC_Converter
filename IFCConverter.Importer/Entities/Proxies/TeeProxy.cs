using System.Collections.Generic;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.ConnectionAugmenters;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Interfaces;

namespace IFCConverter.Importer.Entities.Proxies
{
    [ProxyEntity(typeof(BoundPointConnectionResolver), 3, typeof(TeeConnectionAugmenter))]
    internal sealed class TeeProxy : IFittingProxy
    {
        public double HeadDiameter { get; }
        public double MainDiameter { get; }
        
        private IEnumerable<Vector<double>>? _boundary;

        public TeeProxy(
            Vector<double> position,
            Vector<double> mainProjection,
            Vector<double> headProjection,
            double mainDiameter,
            double headDiameter)
        {
            Position = position;
            MainProjection = mainProjection;
            HeadProjection = headProjection;
            MainDiameter = mainDiameter;
            HeadDiameter = headDiameter;
        }

        public Vector<double> MainProjection { get; }
        public Vector<double> HeadProjection { get; }

        public string? Name { get; set; }
        public Vector<double> Position { get; }

        public IEnumerable<Vector<double>> Boundary => _boundary ??= GetBoundaryPoints();

        public IStartEntity ToStartEntity()
        {
            StartWeldedTeeEntity teeEntity = new();
            teeEntity.Position = Position;

            double headLength = HeadProjection.L2Norm();
            double mainLength = MainProjection.L2Norm();
            teeEntity.CrotchHeight.CreateFromSI(headLength - MainDiameter / 2);
            teeEntity.HeaderLength.CreateFromSI(mainLength);

            if (Name != null)
                teeEntity.Name = Name;

            return teeEntity;
        }

        private IEnumerable<Vector<double>> GetBoundaryPoints()
        {
            return new[]
            {
                Position + HeadProjection,
                Position + MainProjection / 2,
                Position - MainProjection / 2
            };
        }
    }
}