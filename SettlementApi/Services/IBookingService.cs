using SettlementApi.Helpers;

namespace SettlementApi.Services;

public interface IBookingService
{
    Result<bool> IsTimeValid(string time);
    Result<bool> CanBook(string time);
    Result<string> BookTime(string time, string name);
}
