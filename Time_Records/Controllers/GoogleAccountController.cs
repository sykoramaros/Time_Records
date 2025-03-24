using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
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

    // [HttpPost("VerifyGoogleToken")]
    // public async Task<IActionResult> VerifyGoogleToken([FromBody] string googleToken) {
    //     var payload = await googleAccountService.VerifyGoogleToken(googleToken);
    //     if (payload == null) {
    //         return Unauthorized("Token is not valid");
    //     }
    //     return Ok("Token is valid");
    // }
    
    // [AllowAnonymous]
    // [HttpPost("RegisterNewUserFromGoogleAsync")]
    // public async Task<IActionResult> RegisterNewUserFromGoogleAsync([FromBody] GoogleAuthDto googleAuthDto) {
    //     if (googleAuthDto == null || string.IsNullOrEmpty(googleAuthDto.IdToken)) {
    //         return BadRequest("Token is missing");
    //     }
    //     try {
    //         var user = await iGoogleAccountService.RegisterNewUserFromGoogleAsync(
    //             googleAuthDto.IdToken,
    //             googleAuthDto.MonthTimeGoal);
    //         return Ok(new {
    //             user.Id,
    //             user.UserName,
    //             user.Email,
    //             user.GoogleId,
    //             user.MonthTimeGoal
    //         });
    //     } catch (Exception ex) {
    //         return BadRequest(ex.Message);
    //     }
    // }
    
    [AllowAnonymous]
    [HttpPost("RegisterNewUserFromGoogleAsync")]
    public async Task<IActionResult> RegisterNewUserFromGoogleAsync([FromBody] GoogleLoginDto googleLoginDto) {
        if (googleLoginDto == null || string.IsNullOrEmpty(googleLoginDto.GoogleLoginToken)) {
            return BadRequest("Token is missing");
        }
        try {
            var user = await iGoogleAccountService.RegisterNewUserFromGoogleAsync(
                googleLoginDto.GoogleLoginToken);
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
    
    // [AllowAnonymous]
    // [HttpPost("GoogleLogin")]
    // public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthDto googleAuthDto) {
    //     if (googleAuthDto == null || string.IsNullOrEmpty(googleAuthDto.IdToken)) {
    //         return BadRequest("Token is missing");
    //     }
    //     var googleAuthLoginDto = await iGoogleAccountService.GoogleLoginToken(googleAuthDto.IdToken);
    //     if (googleAuthLoginDto == null) {
    //         return BadRequest("User not found or token creation failed");
    //     }
    //     return Ok(new {
    //         token = googleAuthLoginDto.Token,
    //         expiration = googleAuthLoginDto.Expiration
    //     });
    // }
    
    [AllowAnonymous]
    [HttpPost("GoogleLogin")]
    [SwaggerOperation(
        Summary = "Returns user data through Google Id Login Token",
        Description = "As input data is needed onlly importedGoogleLoginToken from OAuth2 google token"
    )]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto googleLoginDto) {
        if (googleLoginDto == null || string.IsNullOrEmpty(googleLoginDto.GoogleLoginToken)) {
            return BadRequest("Token is missing");
        }
        try {
            var authResult = await iGoogleAccountService.GoogleLoginToken(googleLoginDto.GoogleLoginToken);
            if (authResult == null) {
                return BadRequest("User not found or token creation failed");
            }
            authResult.Message = "Google login successful";
            return Ok(authResult);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}