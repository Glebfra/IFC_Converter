using System.Diagnostics.Contracts;
using IFCConverter.Exporter.Converters.Elements;
using IFCConverter.Exporter.Interfaces;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Entities.Fittings;
using IFCConverter.Start.Entities.Joints;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;
using Xbim.Common;

namespace IFCConverter.Exporter.Converters
{
    internal static class ConverterFactory
    {
        [Pure]
        public static IIfcElementConverter CreateConverter(IModel model, IStartEntity startEntity)
        {
            switch (startEntity)
            {
                // Anchors
                case StartFixedAnchorEntity _:
                    return new FixedAnchorConverter(model);
                case StartMomentFreeAnchorEntity _:
                    return new MomentFreeAnchorConverter(model);
                case StartRestingSupportAnchorEntity _:
                    return new RestingSupportAnchorConverter(model);
                case StartAbstractSpringAnchorEntity _:
                    return new SpringAnchorConverter(model);
                case StartRigidHangerAnchorEntity _:
                    return new RigidHangerAnchorConverter(model);
                case StartAbstractDirectionalGuideAnchorEntity _:
                    return new DirectionalGuideAnchorConverter(model);
                case StartAbstractConstantSpringAnchorEntity _:
                    return new ConstantSpringAnchorConverter(model);
                case StartNonstandardAnchorEntity _:
                    return new NonstandardAnchorConverter(model);
                // Expansion Joint Entities
                case StartAngularExpansionJointEntity _:
                case StartBallExpansionJointEntity _:
                    return new SphericalPipesJointConverter(model);
                case StartTorsionExpansionJointEntity _:
                    return new TorsionExpansionJointConverter(model);
                case StartAxialExpansionJointEntity _:
                case StartAxialExpansionSlipJointEntity _:
                    return new AxialExpansionJointConverter(model);
                case StartAxialCouplingJointEntity _:
                    return new AxialCouplingJointConverter(model);
                case StartLateralExpansionJointEntity _:
                    return new LateralExpansionJointConverter(model);
                case StartUniversalExpansionJointEntity _:
                case StartNonStandardExpansionJointEntity _:
                    return new SegmentedExpansionJointConverter(model);
                // Fitting Entities
                case StartValveEntity _:
                    return new ValveConverter(model);
                case StartAbstractReducerEntity _:
                    return new ReducerConverter(model);
                case StartAbstractBendEntity _:
                    return new BendConverter(model);
                case StartAbstractTeeEntity _:
                    return new TeeConverter(model);
                // Segment Entities
                case StartBeamEntity _:
                    return new BeamConverter(model);
                case StartConeElementEntity _:
                    return new ConeElementConverter(model);
                case StartAbstractSegmentEntity _:
                    return new PipeConverter(model);
                default:
                    return null;
            }
        }
    }
}