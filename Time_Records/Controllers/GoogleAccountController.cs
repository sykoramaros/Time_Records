using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Time_Records.DTO;
using Time_Records.Models;
using Time_Records.Services;

namespace Time_Records.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GoogleAccountController : ControllerBase {
    private UserManager<AppUser> userManager;
    private readonly IConfiguration configuration;
    private readonly IGoogleAccountService iGoogleAccountService;
    // private RoleManager<IdentityRole>? roleManager;
    // private SignInManager<AppUser> signInManager;
    private GoogleAccountService googleAccountService;

    // public GoogleAccountController(UserManager<AppUser> userManager, GoogleAccountService googleAccountService) {
    //     this.userManager = userManager;
    //     this.googleAccountService = googleAccountService;
    // }

    // public GoogleAccountController(UserManager<AppUser> userManager, IGoogleAccountService iGoogleAccountService, GoogleAccountService googleAccountService) {
    //     this.userManager = userManager;
    //     this.iGoogleAccountService = iGoogleAccountService;
    //     this.googleAccountService = googleAccountService;
    // }

    public GoogleAccountController(UserManager<AppUser> userManager, IConfiguration configuration, IGoogleAccountService iGoogleAccountService, GoogleAccountService googleAccountService) {
        this.userManager = userManager;
        this.configuration = configuration;
        this.iGoogleAccountService = iGoogleAccountService;
        this.googleAccountService = googleAccountService;
    }

    [HttpPost("VerifyGoogleToken")]
    public async Task<IActionResult> VerifyGoogleToken([FromBody] string googleToken) {
        var payload = await googleAccountService.VerifyGoogleToken(googleToken);
        if (payload == null) {
            return Unauthorized("Token is not valid");
        }
        return Ok("Token is valid");
    }
    
    [HttpPost("RegisterNewUserFromGoogleAsync")]
    public async Task<IActionResult> RegisterNewUserFromGoogleAsync([FromBody] GoogleAuthDto googleAuthDto) {
        if (googleAuthDto == null || string.IsNullOrEmpty(googleAuthDto.IdToken)) {
            return BadRequest("Token is missing");
        }
        try {
            var user = await iGoogleAccountService.RegisterNewUserFromGoogleAsync(
                googleAuthDto.IdToken,
                googleAuthDto.MonthTimeGoal);
            return Ok(new {
                user.Id,
                user.UserName,
                user.Email,
                user.GoogleId,
                user.MonthTimeGoal
            });
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("GoogleLogin")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthDto googleAuthDto) {
        if (googleAuthDto == null || string.IsNullOrEmpty(googleAuthDto.IdToken)) {
            return BadRequest("Token is missing");
        }
        var googleAuthLoginDto = await iGoogleAccountService.GoogleLoginToken(googleAuthDto.IdToken);
        if (googleAuthLoginDto == null) {
            return BadRequest("User not found or token creation failed");
        }
        return Ok(new {
            token = googleAuthLoginDto.Token,
            expiration = googleAuthLoginDto.Expiration
        });
    }
}