using Moq;
using SettlementApi.DataRepositories;
using SettlementApi.Services;

namespace SettlementApi.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockRepository;
        private readonly BookingService _service;

        public BookingServiceTests()
        {
            _mockRepository = new Mock<IBookingRepository>();
            _service = new BookingService(_mockRepository.Object);
        }

        [Theory]
        [InlineData("09:00", true)] // Exact start time
        [InlineData("16:00", true)] // Exact end time
        [InlineData("17:00", false)] // Outside business hours
        [InlineData("08:59", false)] // Just before opening
        [InlineData("16:59", false)] // Just before closing
        public void IsTimeValid_ShouldHandleAllTimes(string time, bool expectedValid)
        {
            // Arrange
            _mockRepository.Setup(repo => repo.IsTimeValid(It.IsAny<TimeSpan>()))
                .Returns(expectedValid);

            // Act
            var result = _service.IsTimeValid(time);

            // Assert
            Assert.Equal(expectedValid, result.Success);
            Assert.Equal(expectedValid, result.Value);
            if (!expectedValid)
            {
                Assert.Equal("Booking is out of business hours.", result.ErrorMessage);
            }
        }

        [Theory]
        [InlineData("09:00", true)] // Available slot
        [InlineData("10:00", false)] // Fully booked slot
        [InlineData("12:00", false)] // Slot overbooked
        public void CanBook_ShouldHandleAvailability(string time, bool expectedCanBook)
        {
            // Arrange
            _mockRepository.Setup(repo => repo.CanBook(It.IsAny<TimeSpan>()))
                .Returns(expectedCanBook);

            // Act
            var result = _service.CanBook(time);

            // Assert
            Assert.Equal(expectedCanBook, result.Success);
            Assert.Equal(expectedCanBook, result.Value);
            if (!expectedCanBook)
            {
                Assert.Equal("Max number of bookings reached for that hour.", result.ErrorMessage);
            }
        }

        [Theory]
        [InlineData("09:00", "Cristiano Ronaldo", "8e68d12b-98d5-4f2f-841b-f646aefe354b", true)] // Successful booking
        [InlineData("08:00", "Cristiano Ronaldo", null, false)] // Invalid time format
        [InlineData("09:00", "Cristiano Ronaldo", null, false)] // Booking failed
        public void BookTime_ShouldHandleAllScenarios(string time, string name, string bookingId, bool shouldSucceed)
        {
            // Arrange
            if (shouldSucceed)
            {
                _mockRepository.Setup(repo => repo.BookTime(It.IsAny<TimeSpan>(), name))
                    .Returns(bookingId);
            }
            else
            {
                _mockRepository.Setup(repo => repo.BookTime(It.IsAny<TimeSpan>(), name))
                    .Throws(new Exception("Unknown error")); // Simulate an error
            }

            // Act
            var result = _service.BookTime(time, name);

            // Assert
            if (shouldSucceed)
            {
                Assert.True(result.Success);
                Assert.Equal(bookingId, result.Value);
            }
            else
            {
                Assert.False(result.Success);
                Assert.Equal("An error occurred: Unknown error", result.ErrorMessage);
            }
        }

        [Fact]
        public void IsTimeValid_ShouldHandleInvalidTimeFormat()
        {
            // Arrange
            var invalidTime = "invalid-time";

            // Act
            var result = _service.IsTimeValid(invalidTime);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid time format.", result.ErrorMessage);
        }

        [Fact]
        public void BookTime_ShouldHandleInvalidTimeFormat()
        {
            // Arrange
            var invalidTime = "invalid-time";

            // Act
            var result = _service.BookTime(invalidTime, "Cristiano Ronaldo");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid time format.", result.ErrorMessage);
        }
    }
}
