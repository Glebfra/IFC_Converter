using NUnit.Framework;
using Start;
using Start.API;

namespace Tests.Start
{
    [TestFixture]
    public partial class StartTests
    {
        private const double TOLERANCE = 1e-5;
        
        private StartDataArrayItem[] _startDataArrayItems;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (StartProject startProject = StartProject.OpenProject(@"C:\Users\glebf\source\repos\IFC_Converter\Tests\Start\Test.ctp"))
            {
                _startDataArrayItems = startProject.GetDataArrayItems();
            }
        }
    }
}