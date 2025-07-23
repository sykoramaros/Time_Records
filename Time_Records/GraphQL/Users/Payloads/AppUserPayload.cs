using Time_Records.DTO;
using Time_Records.Models;

namespace Time_Records.GraphQL.Users.Payloads;

public class AppUserPayload {
    public AppUser? User { get; set; }
    public List<string> Errors { get; set; } = new();
}