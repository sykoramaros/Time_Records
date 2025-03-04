using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Time_Records.DTO;
using Time_Records.Models;

namespace Time_Records.Services;

public class GoogleAccountService {
    private ApplicationDbContext dbContext;
    // private UserManager<AppUser> userManager;


    public GoogleAccountService(ApplicationDbContext dbContext, UserManager<AppUser> userManager) {
        this.dbContext = dbContext;
        // this.userManager = userManager;
    }

    public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string googleToken) {
        try {
            var settings = new GoogleJsonWebSignature.ValidationSettings {
                Audience = new[] { "680830179798-oquu7npstv9ofbpv781kq9usq7nfjqtg.apps.googleusercontent.com" }
            };
            return await GoogleJsonWebSignature.ValidateAsync(googleToken, settings);
        } catch (Exception ex) {
            Console.WriteLine($"Google toen veryfication failed: {ex.Message}");
            return null;
        }
    }

    public async Task<AppUser> RegisterUser(AppUserDto userDto) {
        var user = new AppUser {
            UserName = userDto.UserName,
            Email = userDto.Email,
            PhoneNumber = userDto.PhoneNumber,
            MonthTimeGoal = (userDto.MonthTimeGoal == null || userDto.MonthTimeGoal == 0) ? 15 : userDto.MonthTimeGoal
        };
        if (!string.IsNullOrEmpty(userDto.Password)) {
            var hasher = new PasswordHasher<AppUser>();
            user.PasswordHash = hasher.HashPassword(user, userDto.Password);
        }

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }
}