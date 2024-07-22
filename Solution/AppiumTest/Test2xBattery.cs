using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Remote;
using System;

namespace AppiumTest
{
    public class Test2xBattery
    {
        private AndroidDriver<AndroidElement> _driver;

        [OneTimeSetUp]
        public void SetUp()
        {
            //var serverUri = new Uri(Environment.GetEnvironmentVariable("APPIUM_HOST") ?? "http://127.0.0.1:4723/");
            var serverUri = new Uri("http://127.0.0.1:4723/");
            var driverOptions = new AppiumOptions();
            driverOptions.AddAdditionalCapability("platformName", "Android");
            driverOptions.AddAdditionalCapability("appium:automationName", "UIAutomator2");
            driverOptions.AddAdditionalCapability("appium:udid", "emulator-5554");
            driverOptions.AddAdditionalCapability("appium:platformVersion", "11");
            driverOptions.AddAdditionalCapability("appium:appPackage", "com.android.settings");
            driverOptions.AddAdditionalCapability("appium:appActivity", ".Settings");
            // NoReset assumes the app com.google.android is preinstalled on the emulator
            driverOptions.AddAdditionalCapability("appium:noReset", true);

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
        public void TestBattery()
        {
            _driver.StartActivity("com.android.settings", ".Settings");
            _driver.FindElement(By.XPath("//*[@text='Battery']")).Click();
        }
    }
}