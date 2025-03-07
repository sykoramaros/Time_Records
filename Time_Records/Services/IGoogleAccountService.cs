using Microsoft.AspNetCore.Mvc;
using Time_Records.DTO;
using Time_Records.Models;

namespace Time_Records.Services;

public interface IGoogleAccountService {
    Task<AppUser> RegisterNewUserFromGoogleAsync(string idToken, int? monthTimeGoal = null);
    Task<GoogleAuthLoginDto> GoogleLoginToken(string idToken);
}