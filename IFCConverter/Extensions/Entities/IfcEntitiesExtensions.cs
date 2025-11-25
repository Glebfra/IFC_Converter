using System;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Fittings.Vertex;
using IFC.Entities.Segments;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Anchors;
using Start.Entities.Fittings;
using Start.Entities.Segments;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities
{
    internal static partial class IfcEntitiesExtensions
    {
        public static IfcRigidElementEntity CreateRigidElementFromStart(StartRigidElementEntity rigidElement, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreatePipeObjectMatrix(rigidElement, nodeEntities, out double length);
            
            double diameter = segmentEntities.Length switch
            {
                1 => segmentEntities[0].Diameter,
                2 => Math.Min(segmentEntities[0].Diameter, segmentEntities[1].Diameter),
                _ => 0.05
            };
            
            string name = rigidElement.Name;
            string type = rigidElement.Type.ToString();

            IfcRigidElementEntity ifcRigidElementEntity = new IfcRigidElementEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntities),
                type,
                objectMatrix3D,
                length,
                diameter
            );
            
            ifcRigidElementEntity.PropertySets.Add(Pset_Start.CreateFromStart(rigidElement));
            ifcRigidElementEntity.PropertySets.Add(Pset_PipeSegmentTypeCommon.CreateFromStart(rigidElement));
            ifcRigidElementEntity.PropertySets.Add(Qto_PipeSegmentBaseQuantities.CreateFromStart(rigidElement));

            return ifcRigidElementEntity;
        }
        
        public static IfcPipeSegmentEntity CreatePipeSegmentFromStart(StartPipeEntity pipeEntity, IfcNodeEntity[] nodeEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreatePipeObjectMatrix(pipeEntity, nodeEntities, out double length);
            
            string name = pipeEntity.Name;
            string type = pipeEntity.Type.ToString();
            
            IfcPipeSegmentEntity pipeSegment = new IfcPipeSegmentEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntities),
                type,
                objectMatrix3D,
                length,
                pipeEntity.Diameter.SIProperty,
                nodeEntities
            );

            pipeSegment.PropertySets.Add(Pset_Start.CreateFromStart(pipeEntity));
            pipeSegment.PropertySets.Add(Pset_PipeSegmentTypeCommon.CreateFromStart(pipeEntity));
            pipeSegment.PropertySets.Add(Qto_PipeSegmentBaseQuantities.CreateFromStart(pipeEntity));

            return pipeSegment;
        }
        
        public static IfcConeElementEntity CreateConeElementFromStart(StartConeElementEntity coneElement, IfcNodeEntity[] nodeEntities)
        {
            int numSegments = 16;
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreatePipeObjectMatrix(coneElement, nodeEntities, out double length);

            double diameter = coneElement.Diameter.SIProperty;
            double secondDiameter = coneElement.SecondDiameter.SIProperty;
                
            string name = coneElement.Name;
            string type = coneElement.Type.ToString();

            IfcConeElementEntity coneElementEntity = new IfcConeElementEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntities),
                type,
                objectMatrix3D,
                length,
                diameter,
                secondDiameter,
                numSegments
            );
            
            coneElementEntity.PropertySets.Add(Pset_Start.CreateFromStart(coneElement));
            coneElementEntity.PropertySets.Add(Pset_PipeSegmentTypeCommon.CreateFromStart(coneElement));
            coneElementEntity.PropertySets.Add(Qto_PipeSegmentBaseQuantities.CreateFromStart(coneElement));

            return coneElementEntity;
        }
        
        public static IfcCylindricalShellEntity CreateCylindricalShellFromStart(StartPipeEntity pipeEntity, IfcNodeEntity[] nodeEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreatePipeObjectMatrix(pipeEntity, nodeEntities, out double length);
            double diameter = pipeEntity.Diameter.SIProperty;
            
            string name = pipeEntity.Name;
            string type = pipeEntity.Type.ToString();

            IfcCylindricalShellEntity cylindricalShellEntity = new IfcCylindricalShellEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntities),
                type,
                objectMatrix3D,
                length,
                diameter
            );
            
            cylindricalShellEntity.PropertySets.Add(Pset_Start.CreateFromStart(pipeEntity));
            cylindricalShellEntity.PropertySets.Add(Pset_PipeSegmentTypeCommon.CreateFromStart(pipeEntity));
            cylindricalShellEntity.PropertySets.Add(Qto_PipeSegmentBaseQuantities.CreateFromStart(pipeEntity));

            return cylindricalShellEntity;
        }
        
        public static IfcFlexibleSegmentEntity CreateFlexibleSegmentFromStart(StartFlexibleElementEntity flexibleElement, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreatePipeObjectMatrix(flexibleElement, nodeEntities, out double length);

            double diameter = segmentEntities.Length switch
            {
                1 => segmentEntities[0].Diameter,
                2 => Math.Min(segmentEntities[0].Diameter, segmentEntities[1].Diameter),
                _ => 0.05
            };
            
            string name = flexibleElement.Name;
            string type = flexibleElement.Type.ToString();

            IfcFlexibleSegmentEntity flexibleSegmentEntity = new IfcFlexibleSegmentEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntities),
                type,
                objectMatrix3D,
                length,
                diameter
            );
            
            flexibleSegmentEntity.PropertySets.Add(Pset_Start.CreateFromStart(flexibleElement));
            flexibleSegmentEntity.PropertySets.Add(Pset_PipeSegmentTypeCommon.CreateFromStart(flexibleElement));
            flexibleSegmentEntity.PropertySets.Add(Qto_PipeSegmentBaseQuantities.CreateFromStart(flexibleElement));

            return flexibleSegmentEntity;
        }
        
        public static IfcConstantForceSupportEntity CreateConstantForceSupportFromStart(StartConstantForceSupportEntity constantForceSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);

            string name = constantForceSupport.Name;
            string type = constantForceSupport.Type.ToString();

            IfcConstantForceSupportEntity constantForceSupportEntity = new IfcConstantForceSupportEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                segmentEntities[0].Diameter,
                segmentEntities[0].Diameter * 2,
                numSegments
            );

            constantForceSupportEntity.ConnectedEntities.AddRange(segmentEntities);
            constantForceSupportEntity.PropertySets.Add(Pset_Start.CreateFromStart(constantForceSupport));

            return constantForceSupportEntity;
        }
        
        public static IfcConstantForceSupportHangerEntity CreateConstantForceHangerFromStart(StartConstantForceSupportHangerEntity constantForceSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            string name = constantForceSupport.Name;
            string type = constantForceSupport.Type.ToString();

            IfcConstantForceSupportHangerEntity constantForceSupportHangerEntity = new IfcConstantForceSupportHangerEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                segmentEntities[0].Diameter,
                segmentEntities[0].Diameter * 2,
                numSegments
            );
            
            constantForceSupportHangerEntity.ConnectedEntities.AddRange(segmentEntities);
            constantForceSupportHangerEntity.PropertySets.Add(Pset_Start.CreateFromStart(constantForceSupport));
            
            return constantForceSupportHangerEntity;
        }
        
        public static IfcDamperEntity CreateDamperFromStart(StartDamperEntity damperEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;
            
            string name = damperEntity.Name;
            string type = damperEntity.Type.ToString();
            
            IfcDamperEntity ifcDamperEntity = new IfcDamperEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                damperEntity,
                diameter,
                height,
                numSegments
            );
            
            ifcDamperEntity.ConnectedEntities.AddRange(segmentEntities);
            ifcDamperEntity.PropertySets.Add(Pset_Start.CreateFromStart(damperEntity));

            return ifcDamperEntity;
        }
        
        public static IfcFixedAnchorEntity CreateFixedAnchorFromStart(StartAnchorEntity anchorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFixedAnchorObjectMatrix(nodeEntity, abstractSegmentEntities);
            
            string name = anchorEntity.Name;
            string type = anchorEntity.Type.ToString();
            
            IfcFixedAnchorEntity fixedAnchorEntity = new IfcFixedAnchorEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                abstractSegmentEntities[0].Diameter * 2,
                abstractSegmentEntities[0].Diameter * 2
            );
            
            fixedAnchorEntity.ConnectedEntities.AddRange(abstractSegmentEntities);
            fixedAnchorEntity.PropertySets.Add(Pset_Start.CreateFromStart(anchorEntity));
            
            return fixedAnchorEntity;
        }
        
        public static IfcGuideDoubleDirectionSupportEntity CreateGuideDoubleDirectionFromStart(StartGuideDoubleDirectionSupportEntity doubleDirectionSupportEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateDirectionSupportObjectMatrix(nodeEntity, segmentEntities);
            
            string name = doubleDirectionSupportEntity.Name;
            string type = doubleDirectionSupportEntity.Type.ToString();
            
            IfcGuideDoubleDirectionSupportEntity guideDoubleDirectionSupportEntity = new IfcGuideDoubleDirectionSupportEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                segmentEntities[0].Diameter,
                segmentEntities[0].Diameter * 2,
                numSegments
            );
            
            guideDoubleDirectionSupportEntity.ConnectedEntities.AddRange(segmentEntities);
            guideDoubleDirectionSupportEntity.PropertySets.Add(Pset_Start.CreateFromStart(doubleDirectionSupportEntity));
            
            return guideDoubleDirectionSupportEntity;
        }
        
        public static IfcGuideSingleDirectionSupportEntity CreateGuideSingleDirectionFromStart(StartGuideSingleDirectionSupportEntity guideSingleDirectionSupportEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateDirectionSupportObjectMatrix(nodeEntity, segmentEntities);
            
            string name = guideSingleDirectionSupportEntity.Name;
            string type = guideSingleDirectionSupportEntity.Type.ToString();
            
            IfcGuideSingleDirectionSupportEntity guideSingleDirectionSupportEntityIfc = new IfcGuideSingleDirectionSupportEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                segmentEntities[0].Diameter,
                segmentEntities[0].Diameter * 2,
                numSegments
            );
            
            guideSingleDirectionSupportEntityIfc.ConnectedEntities.AddRange(segmentEntities);
            guideSingleDirectionSupportEntityIfc.PropertySets.Add(Pset_Start.CreateFromStart(guideSingleDirectionSupportEntity));
            
            return guideSingleDirectionSupportEntityIfc;
        }
        
        public static IfcAbstractHingedAnchorEntity CreateHingedAnchorFromStart(StartHingedAnchorEntity hingedAnchor, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;
            
            string name = hingedAnchor.Name;
            string type = hingedAnchor.Type.ToString();

            IfcAbstractHingedAnchorEntity hingedAnchorEntity = new IfcHingedAnchorEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                diameter,
                height,
                numSegments
            );
            
            hingedAnchorEntity.ConnectedEntities.AddRange(segmentEntities);
            hingedAnchorEntity.PropertySets.Add(Pset_Start.CreateFromStart(hingedAnchor));

            return hingedAnchorEntity;
        }
        
        public static IfcNonStandardRestraintEntity CreateNonStandardRestraintFromStart(StartNonStandardRestraint nonStandardRestraint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;
            
            string name = nonStandardRestraint.Name;
            string type = nonStandardRestraint.Type.ToString();
            
            IfcNonStandardRestraintEntity restraintEntity = new IfcNonStandardRestraintEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                nonStandardRestraint,
                diameter,
                height,
                numSegments
            );
            
            restraintEntity.ConnectedEntities.AddRange(segmentEntities);
            restraintEntity.PropertySets.Add(Pset_Start.CreateFromStart(nonStandardRestraint));
            
            return restraintEntity;
        }
        
        public static IfcRigidHangerEntity CreateRigidHangerFromStart(StartRigidHangerEntity rigidHanger, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;
            
            string name = rigidHanger.Name;
            string type = rigidHanger.Type.ToString();

            IfcRigidHangerEntity rigidHangerEntity = new IfcRigidHangerEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                diameter,
                height,
                numSegments
            );
            
            rigidHangerEntity.ConnectedEntities.AddRange(segmentEntities);
            rigidHangerEntity.PropertySets.Add(Pset_Start.CreateFromStart(rigidHanger));

            return rigidHangerEntity;
        }
        
        public static IfcSlidingSupportEntity CreateSlidingSupportFromStart(StartSlidingSupportEntity slidingSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;
            
            string name = slidingSupport.Name;
            string type = slidingSupport.Type.ToString();

            IfcSlidingSupportEntity slidingSupportEntity = new IfcSlidingSupportEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                diameter,
                height,
                numSegments
            );
            
            slidingSupportEntity.ConnectedEntities.AddRange(segmentEntities);
            slidingSupportEntity.PropertySets.Add(Pset_Start.CreateFromStart(slidingSupport));

            return slidingSupportEntity;
        }
        
        public static IfcSpringHangerEntity CreateSpringHangerFromStart(StartSpringSupportEntity springSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;
            
            string name = springSupport.Name;
            string type = springSupport.Type.ToString();

            IfcSpringHangerEntity springHangerEntity = new IfcSpringHangerEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                diameter,
                height,
                numSegments
            );
            
            springHangerEntity.ConnectedEntities.AddRange(segmentEntities);
            springHangerEntity.PropertySets.Add(Pset_Start.CreateFromStart(springSupport));

            return springHangerEntity;
        }
        
        public static IfcSpringSupportEntity CreateSpringSupportFromStart(StartSpringSupportEntity springSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;
            
            string name = springSupport.Name;
            string type = springSupport.Type.ToString();
            
            IfcSpringSupportEntity springSupportEntity = new IfcSpringSupportEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                diameter,
                height,
                numSegments
            );
            
            springSupportEntity.ConnectedEntities.AddRange(segmentEntities);
            springSupportEntity.PropertySets.Add(Pset_Start.CreateFromStart(springSupport));

            return springSupportEntity;
        }
        
        public static IfcAxialExpansionJointEntity CreateAxisExpansionFromStart(StartAxialExpansionJointEntity axialExpansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double length = axialExpansionJoint.Length.SIProperty;
            double diameter = segmentEntities[0].Diameter;
            
            string name = axialExpansionJoint.Name;
            string type = axialExpansionJoint.Type.ToString();

            IfcAxialExpansionJointEntity ifcAxialExpansionJointEntity = new IfcAxialExpansionJointEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );

            ifcAxialExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            ifcAxialExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(axialExpansionJoint));
            ifcAxialExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(axialExpansionJoint));
            
            return ifcAxialExpansionJointEntity;
        }
        
        public static IfcCadBendEntity CreateCadBendFromStart(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double bendRadius = bendEntity.Radius.SIProperty;
            double pipeRadius = Math.Min(segmentEntities[0].Diameter / 2, segmentEntities[1].Diameter / 2);
            
            double length = angle * bendRadius;
            
            string name = bendEntity.Name;
            string type = bendEntity.Type.ToString();

            IfcCadBendEntity cadBendEntity = new IfcCadBendEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length, 
                angle,
                bendRadius,
                pipeRadius
            );
            
            cadBendEntity.ConnectedEntities.AddRange(segmentEntities);
            cadBendEntity.PropertySets.Add(Pset_Start.CreateFromStart(bendEntity));
            cadBendEntity.PropertySets.Add(Pset_PipeFittingTypeBend.CreateFromStart(bendEntity));
            cadBendEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(bendEntity));

            return cadBendEntity;
        }
        
        public static IfcCapEntity CreateCapFromStart(StartCapEntity capEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double diameter = segmentEntities[0].Diameter;
            double length = diameter / 2;
            
            string name = capEntity.Name;
            string type = capEntity.Type.ToString();
            
            IfcCapEntity ifcCapEntity = new IfcCapEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter
            );
            
            ifcCapEntity.ConnectedEntities.AddRange(segmentEntities);
            ifcCapEntity.PropertySets.Add(Pset_Start.CreateFromStart(capEntity));
            ifcCapEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(capEntity));

            return ifcCapEntity;
        }
        
        public static IfcConnectorEntity CreateConnectorFromStart(StartConnectorEntity connectorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double diameter = segmentEntities[0].Diameter;
            double length = diameter / 4;
            
            string name = connectorEntity.Name;
            string type = connectorEntity.Type.ToString();

            IfcConnectorEntity ifcConnectorEntity = new IfcConnectorEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter
            );
            
            ifcConnectorEntity.ConnectedEntities.AddRange(segmentEntities);
            ifcConnectorEntity.PropertySets.Add(Pset_Start.CreateFromStart(connectorEntity));
            ifcConnectorEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(connectorEntity));

            return ifcConnectorEntity;
        }
        
        public static IfcFabricatedTeeEntity CreateFabricatedTeeFromStart(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);

            double length = teeEntity.HeaderLength.SIProperty;
            double height = teeEntity.BranchHeight.SIProperty + branchPipes[0].Diameter / 2;
            
            string name = teeEntity.Name;
            string type = teeEntity.Type.ToString();

            IfcFabricatedTeeEntity fabricatedTeeEntity = new IfcFabricatedTeeEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            fabricatedTeeEntity.ConnectedEntities.AddRange(segmentEntities);
            fabricatedTeeEntity.PropertySets.Add(Pset_Start.CreateFromStart(teeEntity));
            fabricatedTeeEntity.PropertySets.Add(Pset_PipeFittingTypeJunction.CreateFromStart(teeEntity));
            fabricatedTeeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(teeEntity));

            return fabricatedTeeEntity;
        }
        
        public static IfcMilterJointEntity CreateMilterJointFromStart(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double length = 2 * Math.Min(segmentEntities[0].Length.Value, segmentEntities[1].Length.Value) * 0.1;
            
            string name = bendEntity.Name;
            string type = bendEntity.Type.ToString();

            IfcMilterJointEntity milterJointEntity = new IfcMilterJointEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter
            );
            
            milterJointEntity.ConnectedEntities.AddRange(segmentEntities);
            milterJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(bendEntity));
            milterJointEntity.PropertySets.Add(Pset_PipeFittingTypeBend.CreateFromStart(bendEntity));
            milterJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(bendEntity));

            return milterJointEntity;
        }
        
        public static IfcNonstandardExpansionJointEntity CreateNonstandardExpansionFromStart(StartNonstandardExpansionJointEntity nonstandardExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double length = nonstandardExpansion.Length.SIProperty;
            double radius = segmentEntities[0].Diameter / 2;
            
            string name = nonstandardExpansion.Name;
            string type = nonstandardExpansion.Type.ToString();

            IfcNonstandardExpansionJointEntity nonstandardExpansionJointEntity = new IfcNonstandardExpansionJointEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                radius
            );
            
            nonstandardExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            nonstandardExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(nonstandardExpansion));
            nonstandardExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(nonstandardExpansion));

            return nonstandardExpansionJointEntity;
        }
        
        public static IfcNonStandardTeeEntity CreateNonstandardTeeFromStart(StartNonstandardTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);
            
            double length = teeEntity.HeaderLength.SIProperty;
            double height = branchPipes[0].Diameter / 2 + teeEntity.BranchHeight.SIProperty;
            
            string name = teeEntity.Name;
            string type = teeEntity.Type.ToString();

            IfcNonStandardTeeEntity nonstandardTeeEntity = new IfcNonStandardTeeEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            nonstandardTeeEntity.ConnectedEntities.AddRange(segmentEntities);
            nonstandardTeeEntity.PropertySets.Add(Pset_Start.CreateFromStart(teeEntity));
            nonstandardTeeEntity.PropertySets.Add(Pset_PipeFittingTypeJunction.CreateFromStart(teeEntity));
            nonstandardTeeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(teeEntity));

            return nonstandardTeeEntity;
        }
        
        public static IfcStandTeeEntity CreateStandTeeFromStart(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);
            
            double length = teeEntity.HeaderLength.SIProperty;
            double height = branchPipes[0].Diameter / 2 + teeEntity.BranchHeight.SIProperty;
            
            string name = teeEntity.Name;
            string type = teeEntity.Type.ToString();

            IfcStandTeeEntity standTeeEntity = new IfcStandTeeEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            standTeeEntity.ConnectedEntities.AddRange(segmentEntities);
            standTeeEntity.PropertySets.Add(Pset_Start.CreateFromStart(teeEntity));
            standTeeEntity.PropertySets.Add(Pset_PipeFittingTypeJunction.CreateFromStart(teeEntity));
            standTeeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(teeEntity));

            return standTeeEntity;
        }
        
        public static IfcStubInEntity CreateStubInFromStart(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);

            double length = headPipe.Diameter;
            double height = branchPipes[0].Diameter / 2;
            
            string name = teeEntity.Name;
            string type = teeEntity.Type.ToString();

            IfcStubInEntity stubInEntity = new IfcStubInEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            stubInEntity.ConnectedEntities.AddRange(segmentEntities);
            stubInEntity.PropertySets.Add(Pset_Start.CreateFromStart(teeEntity));
            stubInEntity.PropertySets.Add(Pset_PipeFittingTypeJunction.CreateFromStart(teeEntity));
            stubInEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(teeEntity));

            return stubInEntity;
        }
        
        public static IfcSweepoletEntity CreateSweepoletFromStart(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);

            double length = headPipe.Diameter;
            double height = branchPipes[0].Diameter / 2;
            
            string name = teeEntity.Name;
            string type = teeEntity.Type.ToString();
            
            IfcSweepoletEntity sweepoletEntity = new IfcSweepoletEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            sweepoletEntity.ConnectedEntities.AddRange(segmentEntities);
            sweepoletEntity.PropertySets.Add(Pset_Start.CreateFromStart(teeEntity));
            sweepoletEntity.PropertySets.Add(Pset_PipeFittingTypeJunction.CreateFromStart(teeEntity));
            sweepoletEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(teeEntity));

            return sweepoletEntity;
        }
        
        public static IfcUniversalExpansionJointEntity CreateUniversalExpansionJointFromStart(StartUniversalExpansionJointEntity universalExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);

            double length = universalExpansion.Length.SIProperty;
            double radius = segmentEntities[0].Diameter / 2;
            
            string name = universalExpansion.Name;
            string type = universalExpansion.Type.ToString();

            IfcUniversalExpansionJointEntity universalExpansionJointEntity = new IfcUniversalExpansionJointEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                radius
            );
            
            universalExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            universalExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(universalExpansion));
            universalExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(universalExpansion));

            return universalExpansionJointEntity;
        }
        
        public static IfcVertexAngularExpansionJointEntity CreateAngularExpansionJointFromStart(StartAngularExpansionJointEntity angularExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double length = angularExpansion.Length.SIProperty;
            double diameter = length;
            
            string name = angularExpansion.Name;
            string type = angularExpansion.Type.ToString();

            IfcVertexAngularExpansionJointEntity angularExpansionJointEntity = new IfcVertexAngularExpansionJointEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                angle,
                diameter,
                numSegments
            );
            
            angularExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            angularExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(angularExpansion));
            angularExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(angularExpansion));

            return angularExpansionJointEntity;
        }
        
        public static IfcVertexAxialExpansionJointEntity CreateAxialExpansionJointFromStart(StartAxialExpansionJointEntity expansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);

            double length = expansionJoint.Length.SIProperty;
            double diameter = segmentEntities[0].Diameter;
            
            string name = expansionJoint.Name;
            string type = expansionJoint.Type.ToString();

            IfcVertexAxialExpansionJointEntity axialExpansionJointEntity = new IfcVertexAxialExpansionJointEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );
            
            axialExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            axialExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(expansionJoint));
            axialExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(expansionJoint));

            return axialExpansionJointEntity;
        }
        
        public static IfcVertexBallExpansionJointEntity CreateBallExpansionJointFromStart(StartBallExpansionJointEntity ballExpansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);

            double length = ballExpansionJoint.Length.SIProperty;
            double diameter = length * 2;
            
            string name = ballExpansionJoint.Name;
            string type = ballExpansionJoint.Type.ToString();

            IfcVertexBallExpansionJointEntity ballExpansionJointEntity = new IfcVertexBallExpansionJointEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );
            
            ballExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            ballExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(ballExpansionJoint));
            ballExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(ballExpansionJoint));

            return ballExpansionJointEntity;
        }
        
        public static IfcVertexBendEntity CreateVertexBendFromStart(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double bendRadius = bendEntity.Radius.SIProperty;
            double pipeRadius = Math.Min(segmentEntities[0].Diameter / 2, segmentEntities[1].Diameter / 2);
            
            string name = bendEntity.Name;
            string type = bendEntity.Type.ToString();

            IfcVertexBendEntity ifcBendEntity = new IfcVertexBendEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                0,
                angle,
                bendRadius,
                pipeRadius,
                numSegments
            );
            
            ifcBendEntity.ConnectedEntities.AddRange(segmentEntities);
            ifcBendEntity.PropertySets.Add(Pset_Start.CreateFromStart(bendEntity));
            ifcBendEntity.PropertySets.Add(Pset_PipeFittingTypeBend.CreateFromStart(bendEntity));
            ifcBendEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(bendEntity));

            return ifcBendEntity;
        }
        
        public static IfcVertexFlangeEntity CreateFlangeFromStart(StartArmatureEntity armature, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double length = armature.Length.SIProperty;
            double[] diameters = segmentEntities.Select(segment => segment.Diameter.Value).ToArray();
            
            string name = armature.Name;
            string type = armature.Type.ToString();

            IfcVertexFlangeEntity flangeEntity = new IfcVertexFlangeEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameters,
                numSegments
            );
            
            flangeEntity.ConnectedEntities.AddRange(segmentEntities);
            flangeEntity.PropertySets.Add(Pset_Start.CreateFromStart(armature));
            flangeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(armature));

            return flangeEntity;
        }
        
        public static IfcVertexLateralExpansionJointEntity CreateLateralExpansionJointFromStart(StartLateralExpansionJointEntity lateralExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double length = lateralExpansion.Length.SIProperty;
            double diameter = length * 2;
            
            string name = lateralExpansion.Name;
            string type = lateralExpansion.Type.ToString();

            IfcVertexLateralExpansionJointEntity lateralExpansionJointEntity = new IfcVertexLateralExpansionJointEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter,
                angle,
                numSegments
            );
            
            lateralExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            lateralExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(lateralExpansion));
            lateralExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(lateralExpansion));

            return lateralExpansionJointEntity;
        }
        
        public static IfcVertexReducerConcentricEntity CreateReducerConcentricFromStart(StartReducerEntity reducer, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            segmentEntities = segmentEntities.OrderBy(segment => segment.Diameter.Value).ToArray();
            
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateReducerConcentricObjectMatrix(nodeEntity, segmentEntities);

            double length = reducer.LengthOfConicalPart.SIProperty;
            double[] diameters = segmentEntities.Select(segment => segment.Diameter.Value).ToArray();
            
            string name = reducer.Name;
            string type = reducer.Type.ToString();

            IfcVertexReducerConcentricEntity reducerConcentricEntity = new IfcVertexReducerConcentricEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameters,
                numSegments
            );
            
            reducerConcentricEntity.ConnectedEntities.AddRange(segmentEntities);
            reducerConcentricEntity.PropertySets.Add(Pset_Start.CreateFromStart(reducer));
            reducerConcentricEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(reducer));

            return reducerConcentricEntity;
        }
        
        public static IfcVertexReducerEccentricEntity CreateReducerEccentricFromStart(StartReducerEntity reducer, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            segmentEntities = segmentEntities
                .OrderBy(segment => segment.Diameter.Value)
                .ToArray();
            
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateReducerEccentricObjectMatrix(nodeEntity, segmentEntities, out double displacementLength);

            if ((segmentEntities[1].StartNode.GetDistanceToNode(nodeEntity) < segmentEntities[1].EndNode.GetDistanceToNode(nodeEntity)))
            {
                segmentEntities[1].MovePipe(objectMatrix3D.Up * displacementLength);
            }

            double length = reducer.LengthOfConicalPart.SIProperty;
            double[] diameters = segmentEntities.Select(segment => segment.Diameter.Value).ToArray();
            
            string name = reducer.Name;
            string type = reducer.Type.ToString();

            IfcVertexReducerEccentricEntity reducerEntity = new IfcVertexReducerEccentricEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                displacementLength,
                diameters,
                numSegments
            );

            reducerEntity.ConnectedEntities.AddRange(segmentEntities);
            reducerEntity.PropertySets.Add(Pset_Start.CreateFromStart(reducer));
            reducerEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(reducer));

            return reducerEntity;
        }
        
        public static IfcVertexSaddleBendEntity CreateSaddleBendFromStart(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateSaddleBendObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity[] branchPipes, out IfcAbstractSegmentEntity headPipe);
            
            double bendRadius = bendEntity.Radius.SIProperty;
            double pipeRadius = Math.Min(headPipe.Diameter / 2, branchPipes[0].Diameter / 2);
            double branchPipeRadius = branchPipes[1].Diameter / 2;

            double length = angle * bendRadius;
            double branchHeight = length / 2;
            
            string name = bendEntity.Name;
            string type = bendEntity.Type.ToString();

            IfcVertexSaddleBendEntity saddleBendEntity = new IfcVertexSaddleBendEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                angle,
                bendRadius,
                pipeRadius,
                numSegments,
                branchHeight,
                branchPipeRadius
            );
            
            saddleBendEntity.ConnectedEntities.AddRange(segmentEntities);
            saddleBendEntity.PropertySets.Add(Pset_Start.CreateFromStart(bendEntity));
            saddleBendEntity.PropertySets.Add(Pset_PipeFittingTypeBend.CreateFromStart(bendEntity));
            saddleBendEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(bendEntity));

            return saddleBendEntity;
        }
        
        public static IfcVertexSingleFlangeEntity CreateSingleFlangeFromStart(StartArmatureEntity armatureEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double length = armatureEntity.Length.SIProperty;
            double diameter = segmentEntities[0].Diameter;
            
            string name = armatureEntity.Name;
            string type = armatureEntity.Type.ToString();

            IfcVertexSingleFlangeEntity singleFlangeEntity = new IfcVertexSingleFlangeEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );
            
            singleFlangeEntity.ConnectedEntities.AddRange(segmentEntities);
            singleFlangeEntity.PropertySets.Add(Pset_Start.CreateFromStart(armatureEntity));
            singleFlangeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(armatureEntity));

            return singleFlangeEntity;
        }
        
        public static IfcVertexTorsionExpansionJointEntity CreateTorsionExpansionJointFromStart(StartTorsionExpansionJointEntity torsionExpansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double diameter = segmentEntities[0].Diameter;
            double length = torsionExpansionJoint.Length.SIProperty;
            
            string name = torsionExpansionJoint.Name;
            string type = torsionExpansionJoint.Type.ToString();

            IfcVertexTorsionExpansionJointEntity vertexTorsionExpansionJointEntity = new IfcVertexTorsionExpansionJointEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );
            
            vertexTorsionExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            vertexTorsionExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(torsionExpansionJoint));
            vertexTorsionExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(torsionExpansionJoint));

            return vertexTorsionExpansionJointEntity;
        }
        
        public static IfcVertexValveEntity CreateValveFromStart(StartArmatureEntity armatureEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);

            double diameter = Math.Max(segmentEntities[0].Diameter, segmentEntities[1].Diameter);
            double length = armatureEntity.Length.SIProperty;
            
            string name = armatureEntity.Name;
            string type = armatureEntity.Type.ToString();

            IfcVertexValveEntity valveEntity = new IfcVertexValveEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter,
                angle,
                numSegments
            );
            
            valveEntity.ConnectedEntities.AddRange(segmentEntities);
            valveEntity.PropertySets.Add(Pset_Start.CreateFromStart(armatureEntity));
            valveEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(armatureEntity));

            return valveEntity;
        }
        
        public static IfcWeldedTeeEntity CreateWeldedTeeFromStart(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);
            
            double length = teeEntity.HeaderLength.SIProperty;
            if (length == 0) 
                length = headPipe.Diameter;
            double height = teeEntity.CrotchHeight.SIProperty + branchPipes[0].Diameter / 2;
            
            string name = teeEntity.Name;
            string type = teeEntity.Type.ToString();
            
            IfcWeldedTeeEntity weldedTeeEntity = new IfcWeldedTeeEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            weldedTeeEntity.ConnectedEntities.AddRange(segmentEntities);
            weldedTeeEntity.PropertySets.Add(Pset_Start.CreateFromStart(teeEntity));
            weldedTeeEntity.PropertySets.Add(Pset_PipeFittingTypeJunction.CreateFromStart(teeEntity));
            weldedTeeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(teeEntity));

            return weldedTeeEntity;
        }
        
        public static IfcWeldoletEntity CreateWeldoletFromStart(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);
            
            double length = headPipe.Diameter;
            double height = branchPipes[0].Diameter / 2 + teeEntity.BranchHeight.SIProperty;
            
            string name = teeEntity.Name;
            string type = teeEntity.Type.ToString();

            IfcWeldoletEntity weldoletEntity = new IfcWeldoletEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            weldoletEntity.ConnectedEntities.AddRange(segmentEntities);
            weldoletEntity.PropertySets.Add(Pset_Start.CreateFromStart(teeEntity));
            weldoletEntity.PropertySets.Add(Pset_PipeFittingTypeJunction.CreateFromStart(teeEntity));
            weldoletEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(teeEntity));

            return weldoletEntity;
        }
    }
}