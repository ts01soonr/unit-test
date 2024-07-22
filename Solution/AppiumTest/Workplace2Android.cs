using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;

namespace AppiumTest
{
    public class TestWorkplaceAndroid2
    {
        private AndroidDriver<AndroidElement> _driver;
        private AndroidElement WE;
        private static String XID = "//android.widget.";
        private static String PID = "com.soonr.apps.go.production:id/";
        private static String AID = "android:id/";
        public By getBy(String txt)
        {
            if (txt.StartsWith("//")) return By.XPath(txt);
            By by = By.XPath(XID + txt); // default xpath
            if (txt.StartsWith("#")) return By.Id(PID + txt.Substring(1));
            if (txt.StartsWith("$")) return By.Id(AID + txt.Substring(1));
            if (txt.StartsWith("@")) return By.XPath(XID + "TextView[@text='" + txt.Substring(1) + "']"); //@Projects
            if (txt.StartsWith("&"))
            {
                if (txt.Equals("&+")) return By.Id(PID + "fab_button");
                if (txt.Equals("&UP")) return By.XPath(XID + "ImageButton[@content-desc='Navigate up']");
                if (txt.Equals("&Allow")) return By.Id("com.android.permissioncontroller:id/permission_allow_button");
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
            catch (Exception t)
            {
                return false;
            }
        }
        public String getList()
        {
            //return text under id:list
            System.Threading.Thread.Sleep(2000);
            if (!has("$list")) return "";
            String list = "";
            if (has("#emptyText")) return WE.GetAttribute("text");
            if (wait4("#label", 10))
            {
                foreach (AndroidElement w in _driver.FindElements(getBy("#label")))
                {
                    String name = w.GetAttribute("text");
                    if (list.Length == 0) list = name;
                    else list = list + ":" + name;
                }
            }
            Console.WriteLine(list);
            return list;
        }
        [OneTimeSetUp]
        public void SetUp()
        {
            //var serverUri = new Uri(Environment.GetEnvironmentVariable("APPIUM_HOST") ?? "http://127.0.0.1:4723/");
            var serverUri = new Uri("http://127.0.0.1:4723/");
            var driverOptions = new AppiumOptions();
            driverOptions.AddAdditionalCapability("platformName", "Android");
            driverOptions.AddAdditionalCapability("appium:automationName", "UIAutomator2");
            driverOptions.AddAdditionalCapability("appium:udid", "emulator-5554");
            driverOptions.AddAdditionalCapability("appium:deviceName", "P2");
            driverOptions.AddAdditionalCapability("appium:platformVersion", "10");
            driverOptions.AddAdditionalCapability("appium:app", "C:\\Auto\\a\\VFSx.apk");
            driverOptions.AddAdditionalCapability("appium:appPackage", "com.soonr.apps.go.production");
            driverOptions.AddAdditionalCapability("appium:appActivity", "com.soonr.apps.workplace.ui.LogInActivity");
            // NoReset assumes the app com.google.android is preinstalled on the emulator
            driverOptions.AddAdditionalCapability("appium:noReset", false);

            _driver = new AndroidDriver<AndroidElement>(serverUri, driverOptions, TimeSpan.FromSeconds(180));
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _driver.Dispose();
            _driver.Quit();
        }

        [Test]
        public void TestLogin()
        {
            System.Threading.Thread.Sleep(5000);
            wait4("#editTextEmail", 10);
            if (has("#editTextEmail")) WE.SendKeys("52@test.soonr.com");
            if (has("#loginButton")) WE.Click();
            wait4("#password", 10);
            if (has("#password")) WE.SendKeys("password");
            Console.WriteLine("Done");
            System.Threading.Thread.Sleep(5000);

        }

        [Test]
        public void TestLogin2()
        {

            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            String PID = "com.soonr.apps.go.production:id/";
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id(PID + "editTextEmail")));
            _driver.FindElement(By.Id(PID+"editTextEmail")).SendKeys("52@test.soonr.com");
            _driver.FindElement(By.Id(PID + "loginButton")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id(PID + "password")));
            _driver.FindElement(By.Id(PID + "password")).SendKeys("password");
            Console.WriteLine("Done2");
            System.Threading.Thread.Sleep(5000);
            //_driver.FindElement(By.Id(PID + "loginButton")).Click();

            //_driver.FindElement(By.XPath("//*[@text='Next']")).Click();
        }
    }
}