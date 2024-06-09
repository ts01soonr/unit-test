using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace UnitTestProject
{
    public class TestChromeN
    {

        [SetUp]
        public void Setup()
        {
            //
        }
        [TearDown]
        public void TearDown()
        {
            //
        }

        [Test]
        public void goItalle()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            var driver = new ChromeDriver(chromeOptions);
            Console.WriteLine("open rw.italle.dk");
            driver.Navigate().GoToUrl("https://rw.italle.dk");
            Console.WriteLine(driver.Title);
            driver.FindElement(By.Name("username")).SendKeys("fang");
            Assert.AreEqual(driver.Title, "HB System Login");
            driver.Quit();
        }

        [Test]
        public void goSoonr()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            var driver = new ChromeDriver(chromeOptions);
            Console.WriteLine("open workplace");
            driver.Navigate().GoToUrl("https://us.workplace.datto.com/login");
            Console.WriteLine(driver.Title);
            driver.FindElement(By.Name("userName")).SendKeys("fang");
            Assert.AreEqual(driver.Title, "Workplace | Sign In");
            driver.Quit();
        }

    }
}
