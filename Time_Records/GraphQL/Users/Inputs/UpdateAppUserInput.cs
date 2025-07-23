using System.ComponentModel.DataAnnotations;

namespace Time_Records.GraphQL.Types.Inputs;

[InputObjectType]
public class UpdateAppUserInput {
    [Required]
    public Guid Id { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
    
    [MinLength(3)]
    public string? UserName { get; set; }
    
    [Phone]
    public string? PhoneNumber { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "MonthTimeGoal must be at least 1")]
    public int? MonthTimeGoal { get; set; }
    
    // GoogleId se nemění - je neměnný
}