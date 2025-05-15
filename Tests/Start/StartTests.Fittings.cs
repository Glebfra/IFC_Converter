using System.Linq;
using NUnit.Framework;
using Start.API;
using Start.Entities.Fittings;

namespace Tests.Start
{
    [TestFixture]
    public partial class StartTests
    {
        [Test]
        public void ElbowTest()
        {
            StartBendEntity startBendEntity = _startDataArrayItems.FirstOrDefault(item => item.Type == StartElementType.ELBOW)?.Entity as StartBendEntity;
            
            Assert.NotNull(startBendEntity);
            Assert.True(startBendEntity.Type == StartElementType.ELBOW);
            Assert.True(startBendEntity.ManufacturingTechnologyEnum == StartManufacturingTechnologyEnum.SEAMLESS);
            Assert.AreEqual(0.152, startBendEntity.Radius.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startBendEntity.MillTolerance.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startBendEntity.MillToleranceOutside.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startBendEntity.OvalizationCoefficient.StartProperty, TOLERANCE);
            Assert.AreEqual(0.01, startBendEntity.WallThickness.StartProperty, TOLERANCE);
            Assert.AreEqual(0.005236, startBendEntity.Weight.StartProperty, TOLERANCE); 
        }

        [Test]
        public void PipeBendTest()
        {
            StartBendEntity startBendEntity = _startDataArrayItems.FirstOrDefault(item => item.Type == StartElementType.PIPE_BEND)?.Entity as StartBendEntity;
            
            Assert.NotNull(startBendEntity);
            Assert.True(startBendEntity.Type == StartElementType.PIPE_BEND);
            Assert.True(startBendEntity.ManufacturingTechnologyEnum == StartManufacturingTechnologyEnum.SEAMLESS);
            Assert.AreEqual(0, startBendEntity.MillTolerance.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startBendEntity.MillToleranceOutside.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startBendEntity.NumberOfMilters.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startBendEntity.OvalizationCoefficient.StartProperty, TOLERANCE);
            Assert.AreEqual(0.152, startBendEntity.Radius.StartProperty, TOLERANCE);
            Assert.AreEqual(0.01, startBendEntity.WallThickness.StartProperty, TOLERANCE);
            Assert.AreEqual(0.005236, startBendEntity.Weight.StartProperty, TOLERANCE);
        }
    }
}