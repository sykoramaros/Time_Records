using System.ComponentModel.DataAnnotations;

namespace Time_Records.GraphQL.Types.Inputs;

[InputObjectType]
public class CreateAppUserInput {
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(3)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    [Phone]
    public string? PhoneNumber { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Value for must be at least 1")]
    public int? MonthTimeGoal { get; set; }
}