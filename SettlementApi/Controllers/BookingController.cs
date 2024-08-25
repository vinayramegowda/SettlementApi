using Microsoft.AspNetCore.Mvc;
using SettlementApi.Models.Requests;
using SettlementApi.Services;

namespace SettlementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController(IBookingService bookingService, ILogger<BookingController> logger) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;
    private readonly ILogger<BookingController> _logger = logger;

    [HttpPost]
    public IActionResult Post([FromBody] BookingRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Invalid input format error/s {errors}", errors);
                return BadRequest(new { Errors = errors });
            }

            var timeValidityResult = _bookingService.IsTimeValid(request.BookingTime);
            if (!timeValidityResult.Success)
            {
                _logger.LogWarning("Invalid booking time: {BookingTime} with error: {error}", request.BookingTime, timeValidityResult.ErrorMessage);
                return BadRequest("Out of business hours");
            }

            var canBookResult = _bookingService.CanBook(request.BookingTime);
            if (!canBookResult.Success)
            {
                _logger.LogWarning("Time slot fully booked: {BookingTime} with error: {error}", request.BookingTime, canBookResult.ErrorMessage);
                return Conflict("Time slot fully booked");
            }

            var bookingResult = _bookingService.BookTime(request.BookingTime, request.Name);

            if (!bookingResult.Success)
            {
                _logger.LogError("Error processing the booking for time: {BookingTime} with error: {error}", request.BookingTime, bookingResult.ErrorMessage);
                return BadRequest("Error processing the booking");
            }

            return Ok(new { bookingId = bookingResult.Value });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing the booking");
            return StatusCode(500, "An unexpected error occurred");
        }
    }
}
