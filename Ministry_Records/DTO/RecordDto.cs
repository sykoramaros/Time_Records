using System.ComponentModel.DataAnnotations;

namespace Ministry_Records.DTO;

public class RecordDto {
    public int Id { get; set; }
    
    public DateOnly Date { get; set; }
    
    [Range(typeof(TimeSpan), "00:01:00", "23:59:59", ErrorMessage = "Time must be between 00:01:00 and 23:59:59")]
    public TimeSpan RecordTime { get; set; }
    
    public int? RecordStudy { get; set; }
    
    public string? Description { get; set; }
}