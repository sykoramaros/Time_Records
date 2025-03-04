using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Time_Records.DTO;
using Time_Records.Models;

namespace Time_Records.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase {
    private  UserManager<AppUser> userManager;
    private RoleManager<IdentityRole>? roleManager;
    private SignInManager<AppUser> signInManager;
    private IConfiguration configuration;



    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        IConfiguration configuration) {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.configuration = configuration;
    }

    // [HttpGet]
    // public IActionResult Login(string returnUrl) {
    //     LoginDto loginDto = new LoginDto();
    //     loginDto.ReturnUrl = returnUrl;
    //     return Ok(loginDto);
    // }
    //
    // [HttpPost("Login")]
    // public async Task<IActionResult> Login([FromBody] LoginDto loginDto) {
    //     if (ModelState.IsValid) {
    //         AppUser appUser = await userManager.FindByNameAsync(loginDto.UserName);
    //         if (appUser != null) {
    //             await signInManager.SignOutAsync();
    //             Microsoft.AspNetCore.Identity.SignInResult result
    //                 = await signInManager.PasswordSignInAsync(appUser, loginDto.Password, false, false);
    //             if (result.Succeeded) {
    //                 return Ok(new { message = "Login successful", returnUrl = loginDto.ReturnUrl ?? "/" });
    //             }
    //         }
    //     }
    //
    //     return Unauthorized();
    // }

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
    
    // [HttpGet("Google-login")]
    // public IActionResult GoogleLogin() 
    // {
    //     // Explicitně nastavte absolute URL
    //     var redirectUrl = Url.Action(
    //         action: "GoogleLoginCallback", 
    //         controller: "Account", 
    //         values: null, 
    //         protocol: Request.Scheme,
    //         host: Request.Host.Value);
    //
    //     if (string.IsNullOrEmpty(redirectUrl))
    //     {
    //         return BadRequest("Redirect URL is null or empty");
    //     }
    //
    //     // Pro debug vypište URL
    //     Console.WriteLine($"Redirect URL: {redirectUrl}");
    //
    //     return Challenge(
    //         new AuthenticationProperties { RedirectUri = redirectUrl }, 
    //         GoogleDefaults.AuthenticationScheme);
    // }
    
    // [HttpGet("Google-login")]
    // public IActionResult GoogleLogin() 
    // {
    //     // Použijte přesně stejnou redirect URL jako v Google Console
    //     var redirectUrl = "https://localhost:7081/api/Account/Google-login-callback";
    //
    //     return Challenge(
    //         new AuthenticationProperties { RedirectUri = redirectUrl }, 
    //         GoogleDefaults.AuthenticationScheme);
    // }
    
    // [HttpGet("Google-login")]
    // public IActionResult GoogleLogin()
    // {
    //     var properties = new AuthenticationProperties
    //     {
    //         RedirectUri = "https://localhost:7081/api/Account/Google-login-callback",
    //         // Přidejte tyto řádky pro řešení problému se stavem
    //         Items =
    //         {
    //             { ".xsrf", "true" },
    //             { "scheme", GoogleDefaults.AuthenticationScheme },
    //         },
    //         IsPersistent = true
    //     };
    //
    //     return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    // }
    //
    // [HttpGet("Google-login-callback")]
    // public async Task<IActionResult> GoogleLoginCallback() {
    //     var authenticationResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //     if (!authenticationResult.Succeeded) {
    //         return Unauthorized();
    //     }
    //     var claims = authenticationResult.Principal.Identities.FirstOrDefault()?.Claims;
    //     var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    //     var userName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
    //     var user = await userManager.FindByEmailAsync(email);
    //     if (user == null) {
    //         user = new AppUser { UserName = userName, Email = email };
    //         await userManager.CreateAsync(user);
    //     }
    //     await signInManager.SignInAsync(user, false);
    //     return Ok(new { message = "Login successful", returnUrl = "/login" });
    // }
    

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