using Time_Records.Models;

namespace Time_Records.Services;

public interface IGoogleAccountService {
    Task<AppUser> RegisterNewUserFromGoogleAsync(string idToken, int? monthTimeGoal = null);
}