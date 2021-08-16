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

        public void Login()
        {
            String remote_url_chrome = "http://chrome.ukwest.azurecontainer.io:4444/";
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

        public IWebElement FindTacoShackMessageBox()
        {
            Login();
            
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

        public void BuySauce()
        {
            var messageBox = FindTacoShackMessageBox();
            messageBox.SendMessage("!sm buy salsa 1");
        }


        public void SellSauce()
        {
            var messageBox = FindTacoShackMessageBox();
            messageBox.SendMessage("!sm sell salsa 1");
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
