using SettlementApi.Models;

namespace SettlementApi.DataRepositories;

public class BookingRepository : IBookingRepository
{
    private const int MaxSimultaneousBookings = 4;
    private readonly TimeSpan StartTime;
    private readonly TimeSpan EndTime;
    private const int BookingDurationMinutes = 59;

    private readonly List<Booking> _bookings = [];

    public BookingRepository()
    {
        StartTime = new(9, 0, 0);
        EndTime = new(16, 59, 0);
    }

    public bool IsTimeValid(TimeSpan time)
    {
        return time >= StartTime && time.Add(TimeSpan.FromMinutes(BookingDurationMinutes)) <= EndTime;
    }

    public bool CanBook(TimeSpan time)
    {
        var now = DateTime.Today.Add(time);
        var endTime = now.AddMinutes(BookingDurationMinutes);

        // Check if there are overlapping bookings
        return _bookings.Count(b => (b.StartTime < endTime && b.EndTime > now)) < MaxSimultaneousBookings;
    }

    public string BookTime(TimeSpan time, string name)
    {
        var now = DateTime.Today.Add(time);
        var endTime = now.AddMinutes(BookingDurationMinutes);

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

