using Time_Records.DTO;
using Time_Records.Models;

namespace Time_Records.Services;

public interface IGoogleAccountService {
    Task<AppUser> RegisterNewUserFromGoogleAsync(string importedGoogleLoginToken);
    Task<GoogleLoginDto> GoogleLoginToken(string importedGoogleLoginToken);
}