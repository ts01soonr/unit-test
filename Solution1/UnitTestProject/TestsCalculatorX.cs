using Xunit;
using System;
using TestLibrary;
namespace UnitTestProject
{
    public class CalculatorTests
    {
        private readonly MyCalculator _calculator;

        public CalculatorTests()
        {
            _calculator = new MyCalculator();
        }

        [Fact]
        public void Add_SimpleValues_ShouldReturnCorrectSum()
        {
            // Arrange
            int a = 5;
            int b = 3;

            // Act
            int result = _calculator.Add(a, b);

            // Assert
            Assert.Equal(8, result);
        }

        [Fact]
        public void Subtract_SimpleValues_ShouldReturnCorrectDifference()
        {
            // Arrange
            int a = 5;
            int b = 3;

            // Act
            int result = _calculator.Subtract(a, b);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void Multiply_SimpleValues_ShouldReturnCorrectProduct()
        {
            // Arrange
            int a = 5;
            int b = 3;

            // Act
            int result = _calculator.Multiply(a, b);

            // Assert
            Assert.Equal(15, result);
        }

        [Fact]
        public void Divide_SimpleValues_ShouldReturnCorrectQuotient()
        {
            // Arrange
            int a = 6;
            int b = 3;

            // Act
            double result = _calculator.Divide(a, b);

            // Assert
            Assert.Equal(2.0, result, 1);
        }

        [Fact]
        public void Divide_ByZero_ShouldThrowDivideByZeroException()
        {
            // Arrange
            int a = 6;
            int b = 0;

            // Act & Assert
            Assert.Throws<DivideByZeroException>(() => _calculator.Divide(a, b));
        }
    }
}