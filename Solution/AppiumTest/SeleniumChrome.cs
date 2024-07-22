using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace SeleniumTest
{
    [TestFixture]
    public class BasicTests
    {
        private IWebDriver driver;

        [SetUp]
        public void SetUp()
        {
            // Initialize ChromeDriver
            driver = new ChromeDriver(@"C:\AutoTest\res");
        }

        [Test]
        public void TestGoogleHomePageTitle()
        {
            // Navigate to Google
            driver.Navigate().GoToUrl("https://www.google.com");

            // Assert the title
            Assert.AreEqual("Google", driver.Title, "The title of the page is not as expected.");
        }

        [TearDown]
        public void TearDown()
        {
            // Close the browser
            driver.Quit();
        }
    }
}
