using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Time_Records.DTO;
using Time_Records.Models;
using Time_Records.Services;

namespace Time_Records.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GoogleAccountController : ControllerBase {
    private UserManager<AppUser> userManager;
    
    private readonly IGoogleAccountService iGoogleAccountService;
    // private RoleManager<IdentityRole>? roleManager;
    // private SignInManager<AppUser> signInManager;
    private GoogleAccountService googleAccountService;

    // public GoogleAccountController(UserManager<AppUser> userManager, GoogleAccountService googleAccountService) {
    //     this.userManager = userManager;
    //     this.googleAccountService = googleAccountService;
    // }

    public GoogleAccountController(UserManager<AppUser> userManager, IGoogleAccountService iGoogleAccountService, GoogleAccountService googleAccountService) {
        this.userManager = userManager;
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

    [HttpPost("RegisterUser")]
    public async Task<IActionResult> RegisterUser([FromBody] AppUserDto userDto) {
        var user = await googleAccountService.RegisterUser(userDto);
        if (user == null) {
            return BadRequest("Registration is not successfull");
        }
        return Ok(user);
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
}