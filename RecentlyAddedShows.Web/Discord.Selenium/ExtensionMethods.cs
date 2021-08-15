using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Discord.Selenium
{
    public static class ExtensionMethods
        {
            public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
            {
                if (timeoutInSeconds > 0)
                {
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                    return wait.Until(drv => drv.FindElement(by));
                }
                return driver.FindElement(by);
            }

            public static void SendMessage(this IWebElement element, string message)
            {
                element.SendKeys(message);
                element.SendKeys(Keys.Enter);
            }
        }
}
