using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Time_Records.DTO;
using Time_Records.Models;

namespace Time_Records.Services;

public class GoogleAccountService : IGoogleAccountService {
    private readonly ApplicationDbContext dbContext;
    private readonly IConfiguration configuration;
    private UserManager<AppUser> userManager;

    // public GoogleAccountService(ApplicationDbContext dbContext, UserManager<AppUser> userManager) {
    //     this.dbContext = dbContext;
    //     // this.userManager = userManager;
    // }

    public GoogleAccountService(ApplicationDbContext dbContext, IConfiguration configuration, UserManager<AppUser> userManager) {
        this.dbContext = dbContext;
        this.configuration = configuration;
        this.userManager = userManager;
    }

    // verifikace google tokenu
    private static async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string importedGoogleLoginToken) {
        try {
            var settings = new GoogleJsonWebSignature.ValidationSettings {
                Audience = ["680830179798-oquu7npstv9ofbpv781kq9usq7nfjqtg.apps.googleusercontent.com"]
            };
            Console.WriteLine("Starting Google token validation...");
            var payload = await GoogleJsonWebSignature.ValidateAsync(importedGoogleLoginToken, settings);
            Console.WriteLine(settings);
            Console.WriteLine(payload);
            if (payload == null) {
                Console.WriteLine("Google token validation failed.");
                throw new Exception("Payload is null");
            }
            // if (payload.NotBeforeTimeSeconds.HasValue) {
            //     var notBefore = DateTimeOffset.FromUnixTimeSeconds(payload.NotBeforeTimeSeconds.Value);
            //     var now = DateTimeOffset.UtcNow;
            //     if (notBefore > now) {
            //         var waitTime = (int)(notBefore - now).TotalMilliseconds + 1000;
            //         Console.WriteLine($"Waiting {waitTime} milliseconds...");
            //         await Task.Delay(waitTime);
            //     }
            // }
            Console.WriteLine("Google token validation completed.");
            return payload;
        } catch (Exception ex) {
            // Console.WriteLine($"Google token veryfication failed: {ex.Message}");
            Console.WriteLine($"Google token veryfication failed: {ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }
    
    // public (string loginToken, DateTime expiration) GenerateLoginToken(AppUser user, int expirationMinutes = 30) {
    //     try {
    //         var tokenHandler = new JwtSecurityTokenHandler();
    //         var key = Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT:SecretKey not found"));
    //
    //         // Kontrola délky klíče pro HMAC-SHA256
    //         if (key.Length < 32) {
    //             throw new ArgumentException("Secret key must be at least 32 bytes long for HMAC-SHA256");
    //         }
    //         var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);
    //         var tokenDescriptor = new SecurityTokenDescriptor {
    //             Subject = new ClaimsIdentity(new[] {
    //                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    //                 new Claim(ClaimTypes.Email, user.Email),
    //                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //                 new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
    //             }),
    //             Audience = "680830179798-oquu7npstv9ofbpv781kq9usq7nfjqtg.apps.googleusercontent.com",
    //             Expires = expiration,
    //             SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    //         };
    //         var token = tokenHandler.CreateToken(tokenDescriptor);
    //         return (tokenHandler.WriteToken(token), expiration);
    //     }
    //     catch (Exception ex) {
    //         throw new InvalidOperationException("Failed to generate login token", ex);
    //     }
    // }

    public async Task<AppUser> RegisterNewUserFromGoogleAsync(string importedGoogleLoginToken) {
        var payload = await VerifyGoogleToken(importedGoogleLoginToken);
        if (payload == null) {
            throw new Exception("Google token veryfication failed");
        }
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);
        if (existingUser != null) {
            return existingUser;
        }   
        var newUser = new AppUser {
            GoogleId = payload.Subject,
            UserName = payload.Name,
            Email = payload.Email,
            // MonthTimeGoal = (monthTimeGoal == null || monthTimeGoal == 0) ? 15 : monthTimeGoal,
            MonthTimeGoal = 15,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();
        return newUser;
    }

    public async Task<GoogleLoginDto> GoogleLoginToken(string importedGoogleLoginToken) {
        var payload = await VerifyGoogleToken(importedGoogleLoginToken);
        Console.WriteLine("payload:" + payload);
        if (payload == null) {
            throw new Exception("Google token veryfication failed");
        }
        // google id se oznacuje jako Sub nebo Subject
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);
        if (existingUser == null) {
            var newUser = await RegisterNewUserFromGoogleAsync(importedGoogleLoginToken);
            existingUser = newUser;
            // return GenerateLoginToken(newUser);
        }
        if (existingUser.Email == null) {
            throw new InvalidOperationException("User email is null");
        }
        if (existingUser.UserName == null) {
            throw new InvalidOperationException("User username is null");
        }
        if (existingUser.MonthTimeGoal == 0) {
            throw new InvalidOperationException("User monthTimeGoal is zero");
        }
        var claims = new List<Claim> {
            new ("Id", existingUser.Id.ToString()),    // Převod Guid na string
            new (ClaimTypes.Name, existingUser.UserName),
            new (ClaimTypes.Email, existingUser.Email),
            new ("GoogleId", existingUser.GoogleId ?? ""),
            new ("PhoneNumber", existingUser.PhoneNumber ?? ""),
            new ("MonthTimeGoal", existingUser.MonthTimeGoal.ToString() ?? "")
        };
        
        var secretKey = configuration["Jwt:SecretKey"];
        if (secretKey == null) {
            throw new Exception("JWT:SecretKey not found");
        }
        if (string.IsNullOrEmpty(secretKey)) {
            throw new Exception("JWT:SecretKey is empty");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) ?? throw new InvalidOperationException("JWT:SecretKey not found");
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(30);
        var token = new JwtSecurityToken(issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials);
        
        var googleLoginDto = new GoogleLoginDto {
            GoogleLoginToken = new JwtSecurityTokenHandler().WriteToken(token),
            GoogleLoginExpiration = expires
        };
        Console.WriteLine($"Token: {googleLoginDto.GoogleLoginToken}, Expiration: {googleLoginDto.GoogleLoginExpiration}");
        
        return googleLoginDto;
    }
    // Logout na backendu neni nutny ale na frontendu byt muze
}