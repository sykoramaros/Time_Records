using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Time_Records.DTO;
using Time_Records.Models;

namespace Time_Records.Services;

public class GoogleAccountService : IGoogleAccountService {
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
    
    
    public async Task<AppUser> RegisterNewUserFromGoogleAsync(string idToken, int? monthTimeGoal = null) {
        GoogleJsonWebSignature.Payload payload;
        try {
            var settings = new GoogleJsonWebSignature.ValidationSettings {
                Audience = new[] { "680830179798-oquu7npstv9ofbpv781kq9usq7nfjqtg.apps.googleusercontent.com" }
            };
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        } catch (Exception ex) {
            throw new Exception($"Google token veryfication failed", ex);
        }

        var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);
        if (existingUser != null) {
            return existingUser;
        }
        var newUser = new AppUser {
            GoogleId = payload.Subject,
            UserName = payload.Name,
            Email = payload.Email,
            MonthTimeGoal = (monthTimeGoal == null || monthTimeGoal == 0) ? 15 : monthTimeGoal
        };
        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();
        return newUser;
    }
}