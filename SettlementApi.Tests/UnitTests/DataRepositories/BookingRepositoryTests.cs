using SettlementApi.DataRepositories;

namespace SettlementApi.Tests.UnitTests.DataRepositories;

public class BookingRepositoryTests
{
    private BookingRepository CreateRepository => new();
    private BookingRepository CreateRepositoryWithSampleValues
    {
        get
        {
            var repository = new BookingRepository();
            for (int i = 0; i < 4; i++)
            {
                repository.BookTime(new TimeSpan(10, 0, 0), $"Cristiano {i}");
            }
            return repository;
        }
    }

    [Theory]
    [InlineData(10, 0, true)] // 10:00 AM
    [InlineData(8, 0, false)]  // 8:00 AM, before opening hours
    [InlineData(16, 0, true)] // 4:00 PM, ends at 4:59 PM 
    [InlineData(16, 1, false)] // 4:01 PM, ends at 5:00 PM since the latest booking time is 4 PM
    public void IsTimeValid_ShouldReturnExpectedResult(int hours, int minutes, bool expected)
    {
        // Arrange
        var repository = CreateRepository;
        var time = new TimeSpan(hours, minutes, 0);

        // Act
        var result = repository.IsTimeValid(time);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10, 0, true)] // 10:00 AM
    [InlineData(11, 0, false)] // 11:00 AM when the slot is full
    public void CanBook_ShouldReturnExpectedResult(int hours, int minutes, bool expected)
    {
        // Arrange
        var repository = CreateRepository;
        var time = new TimeSpan(hours, minutes, 0);

        if (expected)
        {
            repository.BookTime(time, "Cristiano Ronaldo");
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                repository.BookTime(time, $"Person {i}");
            }
        }

        // Act
        var result = repository.CanBook(time);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10, 0, typeof(InvalidOperationException), "Time slot fully booked or overlaps with another booking")] // 12:00 PM slot full
    [InlineData(8, 0, typeof(ArgumentException), "Invalid booking time")] // Invalid time
    public void BookTime_ShouldThrowExpectedException(int hours, int minutes, Type expectedExceptionType, string expectedMessage)
    {
        // Arrange
        var repository = CreateRepositoryWithSampleValues;
        var time = new TimeSpan(hours, minutes, 0);

        // Act & Assert
        var ex = Assert.Throws(expectedExceptionType, () => repository.BookTime(time, "Cristiano Ronaldo"));

        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public void GetBookings_ShouldReturnAllBookings()
    {
        // Arrange
        var repository = CreateRepository;
        var time = new TimeSpan(14, 0, 0); // 2:00 PM
        repository.BookTime(time, "Bukayo Saka");
        repository.BookTime(time, "Aaron Ramsey");

        // Act
        var bookings = repository.GetBookings();

        // Assert
        Assert.NotEmpty(bookings);
        Assert.Equal(2, bookings.Count(b => b.StartTime.TimeOfDay == time));
    }
}