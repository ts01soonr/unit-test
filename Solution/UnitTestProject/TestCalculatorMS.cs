//MS-Test Example
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestLibrary;
using System;

namespace UnitTestProject
{
    [TestClass]
    public class TestCalculatorMS
    {
        private MyCalculator _calculator;

        [TestInitialize]
        public void Setup()
        {
            _calculator = new MyCalculator();
        }

        [TestMethod]
        public void Add_ShouldReturnCorrectSum()
        {
            // Arrange
            int a = 5;
            int b = 3;

            // Act
            int result = _calculator.Add(a, b);

            // Assert
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public void Subtract_ShouldReturnCorrectDifference()
        {
            // Arrange
            int a = 5;
            int b = 3;

            // Act
            int result = _calculator.Subtract(a, b);

            // Assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void Multiply_ShouldReturnCorrectProduct()
        {
            // Arrange
            int a = 5;
            int b = 3;

            // Act
            int result = _calculator.Multiply(a, b);

            // Assert
            Assert.AreEqual(15, result);
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void Divide_ShouldThrowDivideByZeroException_WhenDividingByZero()
        {
            // Arrange
            int a = 5;
            int b = 0;

            // Act
            _calculator.Divide(a, b);

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        public void Divide_ShouldReturnCorrectQuotient()
        {
            // Arrange
            int a = 6;
            int b = 3;

            // Act
            int result = _calculator.Divide(a, b);

            // Assert
            Assert.AreEqual(2, result);
        }
    }
}

