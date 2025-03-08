using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Time_Records.DTO;
using Time_Records.Models;

namespace Time_Records.Services;

public class GoogleAccountService : IGoogleAccountService {
    private ApplicationDbContext dbContext;
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
    
    public (string loginToken, DateTime expiration) GenerateLoginToken(AppUser user, int expirationMinutes = 30) {
        try {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT:SecretKey not found"));

            // Kontrola délky klíče pro HMAC-SHA256
            if (key.Length < 32) {
                throw new ArgumentException("Secret key must be at least 32 bytes long for HMAC-SHA256");
            }
            var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                }),
                Audience = "680830179798-oquu7npstv9ofbpv781kq9usq7nfjqtg.apps.googleusercontent.com",
                Expires = expiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), expiration);
        }
        catch (Exception ex) {
            throw new InvalidOperationException("Failed to generate login token", ex);
        }
    }

    public async Task<AppUser> RegisterNewUserFromGoogleAsync(string idToken, int? monthTimeGoal = null) {
        var payload = await VerifyGoogleToken(idToken);
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
            MonthTimeGoal = (monthTimeGoal == null || monthTimeGoal == 0) ? 15 : monthTimeGoal
        };
        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();
        return newUser;
    }

    public async Task<GoogleAuthLoginDto> GoogleLoginToken(string idToken) {
        var payload = await VerifyGoogleToken(idToken);
        if (payload == null) {
            throw new Exception("Google token veryfication failed");
        }

        var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);
        if (existingUser == null) {
            var newUser = await RegisterNewUserFromGoogleAsync(idToken, 15);
            existingUser = newUser;
            // return GenerateLoginToken(newUser);
        }

        var claims = new List<Claim> {
            new Claim("Id", existingUser.Id.ToString()),    // Převod Guid na string
            new Claim(ClaimTypes.Name, existingUser.UserName),
            new Claim(ClaimTypes.Email, existingUser.Email),
            new Claim("GoogleId", existingUser.GoogleId),
            new Claim("PhoneNumber", existingUser.PhoneNumber ?? ""),
            new Claim("MonthTimeGoal", existingUser.MonthTimeGoal.ToString())
        };
        
        var secretKey = configuration["Jwt:SecretKey"];
        if (secretKey == null) {
            throw new Exception("JWT:SecretKey not found");
        }
        if (string.IsNullOrEmpty(secretKey)) {
            throw new Exception("JWT:SecretKey is empty");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(30);
        var token = new JwtSecurityToken(issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials);
        
        var googleAuthLoginDto = new GoogleAuthLoginDto {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expires
        };
        Console.WriteLine($"Token: {googleAuthLoginDto.Token}, Expiration: {googleAuthLoginDto.Expiration}");

        return googleAuthLoginDto;
    }
    // Logout na backendu neni nutny ale na frontendu byt muze
}