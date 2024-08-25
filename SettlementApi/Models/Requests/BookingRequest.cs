using System.ComponentModel.DataAnnotations;

namespace SettlementApi.Models.Requests;

public class BookingRequest
{
    [Required]
    [RegularExpression(@"^([01]\d|2[0-3]):([0-5]\d)$", ErrorMessage = "Invalid time format. Please use the format 00:00 (24 hour format)")]
    public string BookingTime{ get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Name cannot be empty")]
    public string Name{ get; set; }
}
