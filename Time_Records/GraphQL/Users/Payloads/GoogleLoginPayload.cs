using Time_Records.DTO;

namespace Time_Records.GraphQL.Users.Payloads;

public class GoogleLoginPayload {
    public GoogleLoginDto? GoogleLoginDto { get; set; }
    public List<string> Errors { get; set; } = new();
}
