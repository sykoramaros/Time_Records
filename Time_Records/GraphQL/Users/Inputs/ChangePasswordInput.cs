using System.ComponentModel.DataAnnotations;

namespace Time_Records.GraphQL.Types.Inputs;

[InputObjectType]
public class ChangePasswordInput {
    [Required] public Guid UserId { get; set; }

    [Required] public string CurrentPassword { get; set; } = string.Empty;

    [Required] [MinLength(6)] public string NewPassword { get; set; } = string.Empty;
}