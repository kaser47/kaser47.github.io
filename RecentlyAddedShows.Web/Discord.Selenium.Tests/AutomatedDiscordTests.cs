using NUnit.Framework;

namespace Discord.Selenium.Tests
{
    public class AutomatedDiscordTests
    {
        private AutomatedDiscord systemUnderTest;

        [SetUp]
        public void Setup()
        {
            systemUnderTest = new AutomatedDiscord();
        }

        [TearDown]
        public void Teardown()
        {
            systemUnderTest.Dispose();
        }

        [Test]
        public void WorkTest()
        {
            systemUnderTest.Work();
        }
    }
}