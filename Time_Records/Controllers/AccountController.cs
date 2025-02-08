using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Time_Records.DTO;
using Time_Records.Models;

namespace Time_Records.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase {
    private UserManager<AppUser> userManager;
    private RoleManager<IdentityRole> roleManager;
    private SignInManager<AppUser> signInManager;
    private IConfiguration configuration;


    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        IConfiguration configuration) {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl) {
        LoginDto loginDto = new LoginDto();
        loginDto.ReturnUrl = returnUrl;
        return Ok(loginDto);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto) {
        if (ModelState.IsValid) {
            AppUser appUser = await userManager.FindByNameAsync(loginDto.UserName);
            if (appUser != null) {
                await signInManager.SignOutAsync();
                Microsoft.AspNetCore.Identity.SignInResult result
                    = await signInManager.PasswordSignInAsync(appUser, loginDto.Password, false, false);
                if (result.Succeeded) {
                    return Ok(new { message = "Login successful", returnUrl = loginDto.ReturnUrl ?? "/" });
                }
            }
        }

        return Unauthorized();
    }

    [HttpPost("Jwt-login")]
    public async Task<IActionResult> JwtLogin([FromBody] LoginDto loginDto) {
        if (ModelState.IsValid) {
            AppUser appUser = await userManager.FindByNameAsync(loginDto.UserName);
            if (appUser != null) {
                var signInResult = await signInManager.PasswordSignInAsync(
                    appUser,
                    loginDto.Password,
                    isPersistent: false,
                    lockoutOnFailure: false
                    );
                if (signInResult.Succeeded) {
                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, appUser.UserName),
                        new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                        new Claim(ClaimTypes.Email, appUser.Email),
                        new Claim(ClaimTypes.MobilePhone, appUser.PhoneNumber ?? "")
                    };
                    var userRoles = await userManager.GetRolesAsync(appUser);
                    foreach (var role in userRoles) {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT:SecretKey not found")));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        issuer: configuration["Jwt:Issuer"],
                        audience: configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(10),
                        signingCredentials: credentials
                        );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(new {
                        token = tokenString,
                        message = "JWT Login successful",
                        returnUrl = loginDto.ReturnUrl ?? "/"
                    });
                }
                return Unauthorized(new { message = "Invalid credentials" });
            }
            return Unauthorized(new { message = "User not found" });
        }
        return BadRequest(new { message = "Invalid model state", errors = ModelState });
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout() {
        await signInManager.SignOutAsync();
        return Ok(new { message = "Logout successful", returnUrl = "/" });
    }

    [HttpGet("Access-denied")]
    public IActionResult AccessDenied() {
        return Unauthorized(new { message = "Access denied", returnUrl = "/forbidden" });
    }
}