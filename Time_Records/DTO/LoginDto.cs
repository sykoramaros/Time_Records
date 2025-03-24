namespace Time_Records.DTO;

public class LoginDto {
    public required string? UserName { get; set; }
    public required string? Email { get; set; }
    public required string Password { get; set; }
    // public string? ReturnUrl { get; set; }
}