using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System;
using System.Collections.Generic;

namespace AppiumTest
{
    public class TestWorkplaceIOS2
    {
        /* Target Appium 2.x version without Workplace app setup.
         * URI: http://10.16.70.27:4723
         * 
         */
        private IOSDriver<IOSElement> _driver;
        private IOSElement WE;
        private static String XID = "//XCUIElementType";
        private static String PID = "";
        private static String AID = "";
        public By getBy(String txt)
        {
            if (txt.StartsWith("//")) return By.XPath(txt);
            By by = By.XPath(XID + txt); // default xpath
            if (txt.StartsWith("#")) return By.Id(PID + txt.Substring(1));
            if (txt.StartsWith("$")) return By.Id(AID + txt.Substring(1));
            if (txt.StartsWith("@"))
            {
                String xpath = XID + "StaticText[@name='" + txt.Substring(1) + "']";
                if (txt.EndsWith("@"))
                { // @Photo@
                    txt = txt.Replace("@", "");
                    xpath = "//XCUIElementTypeStaticText[starts-with(@name,'" + txt + "')]";
                }
                return By.XPath(xpath); //@Projects
            }
            if (txt.StartsWith("&"))
            {
                if (txt.Equals("&+")) return By.Id(PID + "fab_button");
                if (txt.Equals("&UP")) return By.XPath(XID + "ImageButton[@content-desc='Navigate up']");
                if (txt.Equals("&Strike")) return By.XPath(XID + "ImageButton[@content-desc='Strike Out']");
            }

            return by;
        }
        public Boolean wait4(String txt, int n)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(n));
                wait.Until(ExpectedConditions.ElementIsVisible(getBy(txt)));
                return true;
            }
            catch (Exception t)
            {
                Console.WriteLine("can not find element " + txt);
                return false;
            }
        }
        public Boolean has(String txt)
        {
            try
            {
                WE = _driver.FindElement(getBy(txt));
                return WE != null;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        [OneTimeSetUp]
        public void SetUp()
        {
            AppiumOptions options = new AppiumOptions();
            options.AddAdditionalCapability("platformName", "iOS");
            options.AddAdditionalCapability("appium:deviceName", "iPhone 8");
            options.AddAdditionalCapability("appium:udid", "50E2E429-F2D4-46CF-97CF-6D1DEEAFF3AB");
            options.AddAdditionalCapability("appium:app", "/Users/Auto/Auto/a/Workplace.app");
            options.AddAdditionalCapability("appium:automationName", "XCUITest");
            options.AddAdditionalCapability("appium:platformVersion", "15.2");

            _driver = new IOSDriver<IOSElement>(new Uri("http://10.16.70.27:4723"), options);

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            //xcrun simctl list | egrep '(Booted)'
            //driver.Dispose();
            //driver.Quit();
        }

        [Test]
        public void TestLogin()
        {
            /* Target Appium 2.x version without Workplace app setup.
            * URI: http://10.16.70.27:4723
            * 
            */
            Console.WriteLine("testing ios");
            Console.WriteLine(_driver.SessionId);
            Dictionary<string, object> result = (Dictionary<string, object>) _driver.ExecuteScript("mobile: deviceInfo");
            Console.WriteLine(result.GetType());
            Console.WriteLine("timeZone="+result["timeZone"].ToString());
            Console.WriteLine("isSimulator=" + result["isSimulator"].ToString());
            String ele = "#SNRLoginPageViewController_textfield_email";
            Console.WriteLine("hasEmail-input=" + has(ele));
            if(has(ele)) WE.SendKeys("fang");
            Thread.Sleep(5000);

        }


    }
}