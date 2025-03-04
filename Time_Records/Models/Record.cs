using Microsoft.AspNetCore.Identity;

namespace Time_Records.Models;

public class Record {
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeSpan RecordTime { get; set; }
    public int? RecordStudy { get; set; }
    public string? Description { get; set; }
    public Guid? IdentityUserId { get; set; }
}