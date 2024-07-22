//******************************************************************************
//
// Copyright (c) 2017 Microsoft Corporation. All rights reserved.
//
// This code is licensed under the MIT License (MIT).
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//******************************************************************************

using NUnit.Framework;
using OpenQA.Selenium.Appium.Windows;
using System.Threading;
using System;

namespace AppiumTest
{
    public class WinAppTest : WinAppSetup
    {
        private static WindowsElement header;
        private static WindowsElement calculatorResult;

       

        [OneTimeSetUp]
        public void ClassInitialize()
        {

            // Create session to launch a Calculator window
            Setup();

            // Identify calculator mode by locating the header
            try
            {
                header = session.FindElementByAccessibilityId("Header");
            }
            catch
            {
                header = session.FindElementByAccessibilityId("ContentPresenter");
            }            

            // Ensure that calculator is in standard mode
            if (!header.Text.Equals("Standard", StringComparison.OrdinalIgnoreCase))
            {
                session.FindElementByAccessibilityId("TogglePaneButton").Click();
                Thread.Sleep(TimeSpan.FromSeconds(1));
                var splitViewPane = session.FindElementByClassName("SplitViewPane");
                splitViewPane.FindElementByName("Standard Calculator").Click();
                Thread.Sleep(TimeSpan.FromSeconds(1));
                Assert.IsTrue(header.Text.Equals("Standard", StringComparison.OrdinalIgnoreCase));
            }

            // Locate the calculatorResult element
            calculatorResult = session.FindElementByAccessibilityId("CalculatorResults");
            Assert.IsNotNull(calculatorResult);
        }

        [OneTimeTearDown]
        public void ClassCleanup()
        {
            TearDown();
        }

        [Test]
        public void test1Addition()
        {
            // Find the buttons by their names and click them in sequence to perform 1 + 7 = 8
            session.FindElementByName("One").Click();
            session.FindElementByName("Plus").Click();
            session.FindElementByName("Seven").Click();
            session.FindElementByName("Equals").Click();
            Assert.AreEqual("8", GetCalculatorResultText());
        }

        [Test]
        public void test2Division()
        {
            // Find the buttons by their accessibility ids and click them in sequence to perform 88 / 11 = 8
            //session.FindElementByName("Clear").Click();
            session.FindElementByAccessibilityId("num8Button").Click();
            session.FindElementByAccessibilityId("num8Button").Click();
            session.FindElementByAccessibilityId("divideButton").Click();
            session.FindElementByAccessibilityId("num1Button").Click();
            session.FindElementByAccessibilityId("num1Button").Click();
            session.FindElementByAccessibilityId("equalButton").Click();
            Assert.AreEqual("8", GetCalculatorResultText());
        }

        [Test]
        public void test3Multiplication()
        {
            // Find the buttons by their names using XPath and click them in sequence to perform 9 x 9 = 81
            //session.FindElementByName("Clear").Click();
            session.FindElementByXPath("//Button[@Name='Nine']").Click();
            session.FindElementByXPath("//Button[@Name='Multiply by']").Click();
            session.FindElementByXPath("//Button[@Name='Nine']").Click();
            session.FindElementByXPath("//Button[@Name='Equals']").Click();
            Assert.AreEqual("81", GetCalculatorResultText());
        }

        [Test]
        public void test4Subtraction()
        {
            // Find the buttons by their accessibility ids using XPath and click them in sequence to perform 9 - 1 = 8
            //session.FindElementByName("Clear").Click();
            session.FindElementByXPath("//Button[@AutomationId=\"num9Button\"]").Click();
            session.FindElementByXPath("//Button[@AutomationId=\"minusButton\"]").Click();
            session.FindElementByXPath("//Button[@AutomationId=\"num1Button\"]").Click();
            session.FindElementByXPath("//Button[@AutomationId=\"equalButton\"]").Click();
            Assert.AreEqual("8", GetCalculatorResultText());
        }

        private string GetCalculatorResultText()
        {
            return calculatorResult.Text.Replace("Display is", string.Empty).Trim();
        }
    }
}
