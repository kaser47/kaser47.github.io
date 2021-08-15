using NUnit.Framework;

namespace Discord.Selenium.Tests
{
    public class AutomatedDiscordTests
    {
        private AutomatedDiscord systemUnderTest;

        [SetUp]
        public void Setup()
        {
            systemUnderTest = new AutomatedDiscord(@"C:\projects\RecentlyAddedShows\kaser47.github.io\RecentlyAddedShows.Web\Discord.Selenium.Web\bin\Debug\netcoreapp3.1\Resources");
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