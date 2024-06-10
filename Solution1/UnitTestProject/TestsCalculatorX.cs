using Xunit;
using Xunit.Abstractions;
using System;
using TestLibrary;
namespace UnitTestProject
{
    public class TestsCalculatorX
    {
        private readonly MyCalculator _calculator;
        private readonly ITestOutputHelper _output;

        public TestsCalculatorX(ITestOutputHelper output)
        {
            _calculator = new MyCalculator();
            _output = output;
        }
        [Fact]
        public void Add_SimpleValues_ShouldReturnCorrectSum()
        {
            // Arrange
            int a = 5;
            int b = 3;
            _output.WriteLine($"Adding {a} and {b}");

            // Act
            int result = _calculator.Add(a, b);
            _output.WriteLine($"Result of addition: {result}");

            // Assert
            Assert.Equal(8, result);
        }

        [Fact]
        public void Subtract_SimpleValues_ShouldReturnCorrectDifference()
        {
            // Arrange
            int a = 5;
            int b = 3;
            _output.WriteLine($"Subtracting {b} from {a}");

            // Act
            int result = _calculator.Subtract(a, b);
            _output.WriteLine($"Result of subtraction: {result}");

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void Multiply_SimpleValues_ShouldReturnCorrectProduct()
        {
            // Arrange
            int a = 5;
            int b = 3;
            _output.WriteLine($"Multiplying {a} and {b}");

            // Act
            int result = _calculator.Multiply(a, b);
            _output.WriteLine($"Result of multiplication: {result}");

            // Assert
            Assert.Equal(15, result);
        }

        [Fact]
        public void Divide_SimpleValues_ShouldReturnCorrectQuotient()
        {
            // Arrange
            int a = 6;
            int b = 3;
            _output.WriteLine($"Dividing {a} by {b}");

            // Act
            double result = _calculator.Divide(a, b);
            _output.WriteLine($"Result of division: {result}");

            // Assert
            Assert.Equal(2.0, result, 1);
        }

        [Fact]
        public void Divide_ByZero_ShouldThrowDivideByZeroException()
        {
            // Arrange
            int a = 6;
            int b = 0;
            _output.WriteLine($"Attempting to divide {a} by {b}");

            // Act & Assert
            var exception = Assert.Throws<DivideByZeroException>(() => _calculator.Divide(a, b));
            _output.WriteLine($"Caught exception: {exception.Message}");
        }
    }
}