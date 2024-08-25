using System.ComponentModel.DataAnnotations;

namespace SettlementApi.Configurations;

public class BookingConfiguration
{
    [Required]
    [Range(1, 59, ErrorMessage = "BookingDurationMinutes must be between 1 and 59.")]
    public int BookingDurationMinutes { get; set; }

    [Required]
    [Range(1, 10, ErrorMessage = "MaxSimultaneousBookings must be between 1 and 10.")]
    public int MaxSimultaneousBookings { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }
}
