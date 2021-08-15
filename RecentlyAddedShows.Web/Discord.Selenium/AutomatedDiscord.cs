using System;
using System.Diagnostics;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Remote;

namespace Discord.Selenium
{
    public class AutomatedDiscord : IDisposable
    {
        private IWebDriver driver;
        private readonly string webDriverLocation;

        public AutomatedDiscord(string webDriverLocation)
        {
            this.webDriverLocation = webDriverLocation;
        }

        public void Login()
        {
            //var options = new ChromeOptions();
            //options.AddArgument("--no-sandbox");
            //options.AddArguments("headless");


            //String remote_url_chrome = "http://0.0.0.0:4444/wd/hub";
            //ChromeOptions options = new ChromeOptions();
            //driver = new RemoteWebDriver(new Uri(remote_url_chrome), options);
            //driver.Url = Consts.discordWeb;

            //driver = new ChromeDriver(webDriverLocation, options) {Url = Consts.discordWeb};

            String remote_url_chrome = "http://chrome.ukwest.azurecontainer.io:4444/";
            //ChromeOptions options = new ChromeOptions();
            //options.AddArgument("no-sandbox");
            //options.AddArgument("headless");
            //options.AddArgument("--privileged");
            //options.AddArgument("window-size=1200x800");
            driver = new RemoteWebDriver(new Uri(remote_url_chrome), new ChromeOptions());
            driver.Url = Consts.discordWeb;
            var email = driver.FindElement(By.Name("email"), Consts.timeout);
            email.SendKeys(Consts.username);

            var password = driver.FindElement(By.Name("password"), Consts.timeout);
            password.SendKeys(Consts.password);

            IWebElement closeButton = null;

            do
            {
                var submitButton = driver
                    .FindElement(By.XPath("//div[text()='Login']"), Consts.timeout);

                submitButton.Click();

                closeButton = driver
                    .FindElement(By.CssSelector("button[aria-label='Close']"), Consts.timeout);
            } while (closeButton == null);

            closeButton?.Click();
        }

        public string Display()
        {
            return webDriverLocation;
        }

        public IWebElement FindTacoShackMessageBox()
        {
            try
            {
                Login();
            }
            catch (Exception e)
            {
               return FindTacoShackMessageBox();
            }

            var tacoServerIcon = driver.FindElement(By.CssSelector("div[aria-label='  Taco server']"), Consts.timeout);
            tacoServerIcon.Click();

            HandleSayHelloToThreads();
            HandleChangeServerIdentity();

            var generalChannel = driver
                .FindElements(By.TagName("div"))
                .FirstOrDefault(x => x.Text.Contains("general"));
            generalChannel?.Click();

            var messageBox = driver.FindElement(By.CssSelector("div[aria-label='Message #general']"), Consts.timeout);

            return messageBox;
        }

        public void Work()
        {
            var messageBox = FindTacoShackMessageBox();
            messageBox.SendMessage("!work");
        }

        public void Overtime()
        {
            var messageBox = FindTacoShackMessageBox();
            messageBox.SendMessage("!ot");
        }

        public void Tips()
        {
            var messageBox = FindTacoShackMessageBox();
            messageBox.SendMessage("!tips");
        }

        public void Clean()
        {
            var messageBox = FindTacoShackMessageBox();
            messageBox.SendMessage("!clean");
        }

        public void Claim()
        {
            var messageBox = FindTacoShackMessageBox();
            messageBox.SendMessage("!task claim");
        }

        public void Daily()
        {
            var messageBox = FindTacoShackMessageBox();
            messageBox.SendMessage("!daily");
        }

        private void HandleChangeServerIdentity()
        {
            var gotIt = driver.FindElement(By.XPath("//div[text()='Got it']"), Consts.timeout);
            gotIt?.Click();
        }

        private void HandleSayHelloToThreads()
        {
            var gotIt = driver.FindElement(By.XPath("//div[text()='Got it!']"), Consts.timeout);
            gotIt?.Click();
        }

        public void Dispose()
        {
            driver.Quit();
            driver?.Dispose();
        }
    }
}
