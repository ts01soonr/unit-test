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
using System.IO;
using System.Linq;

namespace AppiumTest
{
    public class AndersenMobileApp
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
        private By getBy(String txt)
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
        private Boolean wait4(String txt, int n)
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
        private Boolean has(String txt)
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
            options.AddAdditionalCapability("appium:deviceName", "iPhone 16");
            options.AddAdditionalCapability("appium:udid", "C62EB759-90C1-4EE5-BD11-C8B9BE7916D8");
            options.AddAdditionalCapability("appium:app", "/Users/customer/Documents/code/test/andersen/src/MobileApp/bin/Debug/net9.0-ios/iossimulator-arm64/Configit.Andersen.MobileApp.app");
            options.AddAdditionalCapability("appium:automationName", "XCUITest");
            options.AddAdditionalCapability("appium:platformVersion", "18.4");
            //options.AddAdditionalCapability("appium:noReset", "true");

            _driver = new IOSDriver<IOSElement>(new Uri("http://167.235.122.75:4723"), options);

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
        public void TestEmptyConfigure()
        {
            /* Target Appium 2.x version.
            * URI: http://167.235.122.75:4723
            * 
            */
            Console.WriteLine("testing ios");
            Console.WriteLine(_driver.SessionId);
            Dictionary<string, object> result = (Dictionary<string, object>) _driver.ExecuteScript("mobile: deviceInfo");
            Console.WriteLine(result.GetType());
            Console.WriteLine("timeZone="+result["timeZone"].ToString());
            Console.WriteLine("isSimulator=" + result["isSimulator"].ToString());
            
            const string responseText = "@Your response will be showed here...";
            Console.WriteLine("hasConfigure-Response=" + has(responseText));

            const string enterRequest = "@Enter your configuration request here";
            //Console.WriteLine("hasConfigure-Request=" + has(enterRequest));
            const string requestJson = """
                                        {
                                            "packagePath": "Internal/master/pwu",
                                            "date": "2025-01-01",
                                            "viewId": "IPAD",
                                            "line": {
                                                "productId": "pwu",
                                                "variableAssignments": [
                                                    {
                                                        "variableId": "DIM_BUILDDATE",
                                                        "value": "2025-01-01"
                                                    }
                                                ]
                                            }
                                        }
                                        """;

            if(has(enterRequest)) WE.SendKeys(requestJson);
            
            const string cfgButton = "@Configure";
            //Console.WriteLine("hasConfigure-Button=" + has(cfgButton));
            if( has (cfgButton)) WE.Click();
            Thread.Sleep(1000);
            //string pageSource = _driver.PageSource;
            //File.WriteAllText("appium_page.xml", pageSource);
            const string responseJson = "//XCUIElementTypeTextView[2]";
            Console.WriteLine("hasConfigure-Button=" + has(responseJson));
            string text = WE.GetAttribute("value");
            Console.WriteLine("respone text: \n" + text);
            Assert.IsTrue(text.Contains("packagePath"));
            Assert.IsTrue(text.Contains("phases"));
        }


    }
}