using SettlementApi.DataRepositories;
using SettlementApi.Helpers;

namespace SettlementApi.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;

    public Result<bool> IsTimeValid(string time)
    {
        if(TimeSpan.TryParse(time, out var bookingTime))
        {
            var result = _bookingRepository.IsTimeValid(bookingTime);
            return result ? Result<bool>.SuccessResult(result) : Result<bool>.Failure("Booking is out of business hours.");
        }
        return Result<bool>.Failure("Invalid time format.");
    }

    public Result<bool> CanBook(string time)
    {
        _ = TimeSpan.TryParse(time, out var bookingTime);

        var result = _bookingRepository.CanBook(bookingTime);
        return result ? Result<bool>.SuccessResult(result) : Result<bool>.Failure("Max number of bookings reached for that hour.");
    }

    public Result<string> BookTime(string time, string name)
    {
        try
        {
            if (TimeSpan.TryParse(time, out var bookingTime))
            {
                var bookingId = _bookingRepository.BookTime(bookingTime, name);
                if (bookingId != null)
                {
                    return Result<string>.SuccessResult(bookingId);
                }
                return Result<string>.Failure("Booking failed due to unknown reasons.");
            }
            return Result<string>.Failure("Invalid time format.");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"An error occurred: {ex.Message}");
        }
    }
}
