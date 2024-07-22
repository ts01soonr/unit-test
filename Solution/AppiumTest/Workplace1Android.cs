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
    public class TestAndroidWorkplace1x
    {
        private AndroidDriver<AndroidElement> _driver;

        [OneTimeSetUp]
        public void SetUp()
        {
            //var serverUri = new Uri(Environment.GetEnvironmentVariable("APPIUM_HOST") ?? "http://127.0.0.1:4723/");
            var serverUri = new Uri("http://127.0.0.1:4724/wd/hub");
            var driverOptions = new AppiumOptions();
            driverOptions.AddAdditionalCapability("platformName", "Android");
            driverOptions.AddAdditionalCapability("automationName", "UIAutomator2");
            driverOptions.AddAdditionalCapability("udid", "emulator-5554");
            driverOptions.AddAdditionalCapability("deviceName", "P2");
            driverOptions.AddAdditionalCapability("platformVersion", "10");
            driverOptions.AddAdditionalCapability("app", "C:\\Auto\\a\\VFSx.apk");
            driverOptions.AddAdditionalCapability("appPackage", "com.soonr.apps.go.production");
            driverOptions.AddAdditionalCapability("appActivity", "com.soonr.apps.workplace.ui.LogInActivity");
            // NoReset assumes the app com.google.android is preinstalled on the emulator
            driverOptions.AddAdditionalCapability("noReset", true);

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
        public void TestLogin1x()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            String PID = "com.soonr.apps.go.production:id/";
            //_driver.StartActivity("com.soonr.apps.go.production", "com.soonr.apps.workplace.ui.LogInActivity");
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id(PID + "editTextEmail")));
            _driver.FindElement(By.Id(PID+"editTextEmail")).SendKeys("52@test.soonr.com");
            _driver.FindElement(By.Id(PID + "loginButton")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id(PID + "password")));
            _driver.FindElement(By.Id(PID + "password")).SendKeys("password");
            //_driver.FindElement(By.Id(PID + "loginButton")).Click();

            //_driver.FindElement(By.XPath("//*[@text='Next']")).Click();
        }
    }
}