using System.ComponentModel.DataAnnotations;

namespace Time_Records.GraphQL.Types.Inputs;

// Pro registraci přes Google OAuth (GoogleId je neměnný sub z token payload)

[InputObjectType]
public class CreateAppUserFromGoogleInput {
    [Required]
    [MaxLength(200)]
    public string GoogleId { get; set; } = string.Empty;    // sub z Google tokenu
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string UserName { get; set; } = string.Empty;
    
    [Phone]
    public string? PhoneNumber { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue, ErrorMessage = "Month time goal must be at least 1")]
    public int MonthTimeGoal { get; set; }
}