using SettlementApi.Models;

namespace SettlementApi.DataRepositories;

public interface IBookingRepository
{
    bool IsTimeValid(TimeSpan time);
    bool CanBook(TimeSpan time);
    string BookTime(TimeSpan time, string name);
    IList<Booking> GetBookings();
}
