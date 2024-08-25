using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SettlementApi.Controllers;
using SettlementApi.Helpers;
using SettlementApi.Models.Requests;
using SettlementApi.Services;

namespace SettlementApi.Tests.UnitTests.Controllers;

public class BookingControllerTests
{
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly Mock<ILogger<BookingController>> _mockLogger;
    private readonly BookingController _controller;

    public BookingControllerTests()
    {
        _mockBookingService = new Mock<IBookingService>();
        _mockLogger = new Mock<ILogger<BookingController>>();
        _controller = new BookingController(_mockBookingService.Object, _mockLogger.Object);
    }

    [Theory]
    [InlineData("10:00", "Cristiano Ronaldo", true, true, "new-guid", typeof(OkObjectResult))]
    [InlineData("10:00", "Kimi Raikkonen", true, false, null, typeof(ConflictObjectResult))]
    [InlineData("10:00", "Lewis Hamilton", false, true, null, typeof(BadRequestObjectResult))]
    [InlineData("invalid-time", "Alexis Sanchez", true, true, null, typeof(BadRequestObjectResult))]
    public void Post_ShouldReturnExpectedResult(string bookingTime, string name, bool isTimeValid, bool canBook, string bookingId, Type expectedResultType)
    {
        // Arrange
        _mockBookingService.Setup(service => service.IsTimeValid(bookingTime))
            .Returns(new Result<bool>(isTimeValid, isTimeValid, "Out of business hours"));

        if (isTimeValid)
        {
            _mockBookingService.Setup(service => service.CanBook(bookingTime))
                .Returns(new Result<bool>(canBook, canBook, "Max number of bookings reached"));

            if (canBook)
            {
                _mockBookingService.Setup(service => service.BookTime(bookingTime, name))
                    .Returns(new Result<string>(bookingId, bookingId == null ? false : true, bookingId == null ? "Booking failed" : null));
            }
        }

        var request = new BookingRequest { BookingTime = bookingTime, Name = name };

        // Act
        var result = _controller.Post(request);

        // Assert
        if (expectedResultType == typeof(OkObjectResult))
        {
            var okResult = result as OkObjectResult;
            Assert.Equivalent(new { bookingId }, okResult?.Value);
        }
        else if (expectedResultType == typeof(BadRequestObjectResult))
        {
            var badRequestResult = result as BadRequestObjectResult;
            var content = badRequestResult?.Value;
            if (bookingTime == "invalid-time")
            {
                Assert.Contains("Error processing the booking", content.ToString());
            }
            else
            {
                Assert.Contains("Out of business hours", content.ToString());
            }
        }
        else if (expectedResultType == typeof(ConflictObjectResult))
        {
            var conflictResult = result as ConflictObjectResult;
            Assert.Equal("Time slot fully booked", conflictResult?.Value);
        }
    }

    [Fact]
    public void Post_ShouldHandleUnexpectedException()
    {
        // Arrange
        _mockBookingService.Setup(service => service.IsTimeValid(It.IsAny<string>()))
            .Throws(new Exception("Unexpected error"));

        var request = new BookingRequest { BookingTime = "10:00", Name = "Virat Kohli" };

        // Act
        var result = _controller.Post(request);
        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}
