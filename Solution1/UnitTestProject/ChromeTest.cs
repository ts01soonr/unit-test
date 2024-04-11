using System;
using ClassLibrary1;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace UnitTestProject
{
    public class ChromeTest
    {
        Calculator calculator; 

        [SetUp]
        public void Setup()
        {
            calculator = new Calculator();
        }

        [Test]
        public void startBrowser()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            var driver = new ChromeDriver(chromeOptions);
            Console.WriteLine("start2");
            driver.Navigate().GoToUrl("https://rw.italle.dk");
            Console.WriteLine(driver.Title);
            driver.FindElement(By.Name("username")).SendKeys("fang");
            driver.Quit();
        }

       
    }
}
