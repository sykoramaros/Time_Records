using System.ComponentModel.DataAnnotations;

namespace Time_Records.DTO;

public class RecordDto {
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    [Range(typeof(TimeSpan), "00:00:00", "23:59:59", ErrorMessage = "Time must be between 00:00:00 and 23:59:59")]
    public TimeSpan RecordTime { get; set; } = TimeSpan.Zero;
    [Range(typeof(TimeSpan), "00:00:00", "23:59:59", ErrorMessage = "RecordCreditTime must be between 00:00:00 and 23:59:59")]
    public TimeSpan? RecordCreditTime { get; set; } = TimeSpan.Zero;
    public int? RecordStudy { get; set; }
    public string? Description { get; set; }
    public Guid? IdentityUserId { get; set; }
}