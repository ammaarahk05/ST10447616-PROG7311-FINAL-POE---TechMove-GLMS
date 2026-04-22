using Xunit;
using TechMoveLogisticSystem.Services;

namespace TechMoveLogisticSystem.Tests
{
    public class CurrencyServiceTests
    {
        // test 1: happy path; normal conversion 
        [Fact]
        public void ConvertUsdToZar_ValidInput_ReturnsCorrectValue()
        {
            
            var service = new CurrencyService(null); // HttpClient isnt needed here
            decimal usd = 10;
            decimal rate = 16.3566m;

           
            var result = service.ConvertUsdToZar(usd, rate);

            
            Assert.Equal(163.566m, result);
        }

        // test 2: the zero value edge case
        [Fact]
        public void ConvertUsdToZar_ZeroUsd_ReturnsZero()
        {
            var service = new CurrencyService(null);
            decimal usd = 0;
            decimal rate = 16.3566m;

            var result = service.ConvertUsdToZar(usd, rate);

            Assert.Equal(0, result);
        }

        //test 3: large value test
        [Fact]
        public void ConvertUsdToZar_LargeAmount_ReturnsCorrectValue()
        {
            var service = new CurrencyService(null);
            decimal usd = 1000;
            decimal rate = 16.3566m;

           
            var result = service.ConvertUsdToZar(usd, rate);

            
            Assert.Equal(16356.6m, result);
        }

        //test 4: edge case - negative valu
        [Fact]
        public void ConvertUsdToZar_NegativeUsd_ReturnsNegativeValue()
        {
            // Arrange
            var service = new CurrencyService(null);
            decimal usd = -5;
            decimal rate = 16.3566m;

            // Act
            var result = service.ConvertUsdToZar(usd, rate);

            // Assert
            Assert.Equal(-81.783m, result);
        }
    }
}