using Microsoft.Extensions.Options;
using SettlementApi.Configurations;
using SettlementApi.Models;

namespace SettlementApi.DataRepositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingConfiguration _bookingConfiguration;
    private readonly List<Booking> _bookings = [];

    public BookingRepository(IOptions<BookingConfiguration> options)
    {
        _bookingConfiguration = options.Value;
    }

    public bool IsTimeValid(TimeSpan time)
    {
        return time >= _bookingConfiguration.StartTime && time.Add(TimeSpan.FromMinutes(_bookingConfiguration.BookingDurationMinutes)) <= _bookingConfiguration.EndTime;
    }

    public bool CanBook(TimeSpan time)
    {
        var now = DateTime.Today.Add(time);
        var endTime = now.AddMinutes(_bookingConfiguration.BookingDurationMinutes);

        // Check if there are overlapping bookings
        return _bookings.Count(b => (b.StartTime < endTime && b.EndTime > now)) < _bookingConfiguration.MaxSimultaneousBookings;
    }

    public string BookTime(TimeSpan time, string name)
    {
        var now = DateTime.Today.Add(time);
        var endTime = now.AddMinutes(_bookingConfiguration.BookingDurationMinutes);

        if (!IsTimeValid(time))
        {
            throw new ArgumentException("Invalid booking time");
        }

        if (!CanBook(time))
        {
            throw new InvalidOperationException("Time slot fully booked or overlaps with another booking");
        }

        var bookingId = Guid.NewGuid().ToString();

        _bookings.Add(new Booking{
            BookingId = bookingId,
            StartTime = now,
            EndTime = endTime
        });

        return bookingId;
    }

    public IList<Booking> GetBookings()
    {
        return _bookings;
    }
}

