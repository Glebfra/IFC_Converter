using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using IFCConverter.Interfaces;
using IFCConverter.Utils;
using MathNet.Numerics.LinearAlgebra;
using Start.API;
using Start.Entities.Fittings;
using Start.Interfaces;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using IfcProject = Ifc.API.IfcProject;
using MatrixExtensions = Utils.MatrixExtensions;

namespace IFCConverter.Converters
{
    internal class StartToIfcConverter
    {
        private readonly ExportDataContainer _exportDataContainer;
        private readonly Logger _logger = Logger.GetInstance();

        public StartToIfcConverter(ExportDataContainer exportDataContainer)
        {
            _exportDataContainer = exportDataContainer;
        }

        public void Convert(StartDocument startDocument)
        {
            _logger.System($"STARTtoIFC converter v.{Assembly.GetExecutingAssembly().GetName().Version}");

            using (IStartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                IStartEntity[] startEntities = startProject.GetStartEntities();
                _logger.Info($"Found {startEntities.Count()} objects");

                IStartClippableEntity[] clippableEntities = startEntities.OfType<IStartClippableEntity>().ToArray();
                _logger.Info($"Clippable entities found {clippableEntities.Length} objects");
                ClipEntities(clippableEntities);

                IEnumerable<StartReducerEccentricEntity> reducerEccentricEntities =
                    startEntities.OfType<StartReducerEccentricEntity>();
                MoveSegmentsWithReducers(reducerEccentricEntities);

                using (IfcProject ifcProject = IfcProject.CreateProject(startDocument.GetTitle()))
                {
                    IModel model = ifcProject.Model;
                    foreach (IStartEntity startEntity in startEntities)
                        try
                        {
                            IfcProduct? ifcProduct = CreateIfcEntity(model, startEntity);
                            if (ifcProduct == null)
                                continue;
                            ifcProject.AddEntityRaw(ifcProduct);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(
                                $"Error while converting entity {startEntity.GetType().FullName} with id {startEntity.ID}: {ex}");
                        }

                    ifcProject.SaveAs(_exportDataContainer.OutputFilePath);
                }
            }
        }

        private void ClipEntities(IEnumerable<IStartClippableEntity> clippableEntities)
        {
            foreach (IStartClippableEntity clippableEntity in clippableEntities)
            {
                IEnumerable<IStartClippingEntity> clippingEntities =
                    clippableEntity.ConnectedEntities.OfType<IStartClippingEntity>();
                foreach (IStartClippingEntity clippingEntity in clippingEntities)
                {
                    clippingEntity.ClipEntity(clippableEntity);
                    _logger.Info(
                        $"Clipping entity {clippableEntity.GetType().FullName} by {clippingEntity.GetType().FullName}");
                }
            }
        }

        [Pure]
        private IfcProduct? CreateIfcEntity(IModel model, IStartEntity startEntity)
        {
            IIfcElementConverter? converter = ConverterFactory.CreateConverter(model, startEntity);
            if (converter == null)
                return null;

            _logger.Info($"Created converter {converter?.GetType().FullName}");

            IfcProduct? ifcProduct = converter?.BuildIfc(startEntity) as IfcProduct;
            _logger.Info($"Created product {ifcProduct?.GetType().FullName} (global ifc id: {ifcProduct?.GlobalId})");

            return ifcProduct;
        }

        private static void MoveSegmentsWithReducers(IEnumerable<StartReducerEccentricEntity> reducerEccentricEntities)
        {
            foreach (StartReducerEccentricEntity startReducerEccentricEntity in reducerEccentricEntities)
            {
                IStartSegmentEntity minDiameterSegmentEntity = startReducerEccentricEntity.SegmentWithMinDiameter;
                IStartSegmentEntity maxDiameterSegmentEntity = startReducerEccentricEntity.SegmentWithMaxDiameter;

                double angle = startReducerEccentricEntity.AngleBetweenEccentricityVectorAndZmAxis.SIProperty;
                Matrix<double> minDiameterSegmentMatrix = minDiameterSegmentEntity.TransformationMatrix;
                Matrix<double> rotationMatrix = MatrixExtensions.CreateRotationAroundZ(angle);
                Matrix<double> rotatedMinDiameterSegmentMatrix = rotationMatrix * minDiameterSegmentMatrix;

                Vector<double> displacement = rotatedMinDiameterSegmentMatrix.GetUp() * (
                    maxDiameterSegmentEntity.Diameter.SIProperty - minDiameterSegmentEntity.Diameter.SIProperty
                ) / 2;

                if (maxDiameterSegmentEntity.IsStartPosition(startReducerEccentricEntity.Position))
                    maxDiameterSegmentEntity.StartPosition += displacement;
            }
        }
    }
}