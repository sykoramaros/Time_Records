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
    // private RoleManager<IdentityRole>? roleManager;
    // private SignInManager<AppUser> signInManager;
    private GoogleAccountService googleAccountService;


    // public GoogleAccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole>? roleManager, SignInManager<AppUser> signInManager, GoogleAccountService googleAccountService) {
    //     this.userManager = userManager;
    //     // this.roleManager = roleManager;
    //     // this.signInManager = signInManager;
    //     this.googleAccountService = googleAccountService;
    // }

    public GoogleAccountController(UserManager<AppUser> userManager, GoogleAccountService googleAccountService) {
        this.userManager = userManager;
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
}