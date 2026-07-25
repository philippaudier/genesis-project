using NUnit.Framework;

namespace Genesis.Tests
{
    /// <summary>
    /// Genesis-001 (Foundation) smoke test. It proves the test assembly is wired correctly —
    /// NUnit and the Unity Test Framework resolve, and <c>Genesis.Simulation</c> /
    /// <c>Genesis.Core</c> are referenceable. It asserts nothing about the world and contains no
    /// simulation logic. Replace it when the first real tests arrive.
    /// </summary>
    public sealed class FoundationSmokeTests
    {
        [Test]
        public void Architecture_Compiles_And_TestHarness_Runs()
        {
            Assert.Pass();
        }
    }
}
