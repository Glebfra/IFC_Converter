using System.Diagnostics.Contracts;
using IFCConverter.Exporter.Converters.Elements;
using IFCConverter.Exporter.Interfaces;
using Start.Entities.Anchors;
using Start.Entities.Fittings;
using Start.Entities.Joints;
using Start.Entities.Segments;
using Start.Interfaces;
using Xbim.Common;

namespace IFCConverter.Exporter.Converters
{
    internal static class ConverterFactory
    {
        [Pure]
        public static IIfcElementConverter? CreateConverter(IModel model, IStartEntity startEntity)
        {
            return startEntity switch
            {
                // Anchors
                StartFixedAnchorEntity => new FixedAnchorConverter(model),
                StartMomentFreeAnchorEntity => new MomentFreeAnchorConverter(model),
                StartRestingSupportAnchorEntity => new RestingSupportAnchorConverter(model),
                StartAbstractSpringAnchorEntity => new SpringAnchorConverter(model),
                StartRigidHangerAnchorEntity => new RigidHangerAnchorConverter(model),
                StartAbstractDirectionalGuideAnchorEntity => new DirectionalGuideAnchorConverter(model),
                StartAbstractConstantSpringAnchorEntity => new ConstantSpringAnchorConverter(model),
                StartNonstandardAnchorEntity => new NonstandardAnchorConverter(model),

                // Expansion Joint Entities
                StartAngularExpansionJointEntity => new SphericalPipesJointConverter(model),
                StartBallExpansionJointEntity => new SphericalPipesJointConverter(model),
                StartTorsionExpansionJointEntity => new TorsionExpansionJointConverter(model),
                StartAxialExpansionJointEntity => new AxialExpansionJointConverter(model),
                StartAxialExpansionSlipJointEntity => new AxialExpansionJointConverter(model),
                StartAxialCouplingJointEntity => new AxialCouplingJointConverter(model),
                StartLateralExpansionJointEntity => new LateralExpansionJointConverter(model),
                StartUniversalExpansionJointEntity => new SegmentedExpansionJointConverter(model),
                StartNonStandardExpansionJointEntity => new SegmentedExpansionJointConverter(model),

                // Fitting Entities
                StartValveEntity => new ValveConverter(model),
                StartAbstractReducerEntity => new ReducerConverter(model),
                StartAbstractBendEntity => new BendConverter(model),
                StartAbstractTeeEntity => new TeeConverter(model),

                // Segment Entities
                StartConeElementEntity => new ConeElementConverter(model),
                StartAbstractSegmentEntity => new PipeConverter(model),
                _ => null
            };
        }
    }
}