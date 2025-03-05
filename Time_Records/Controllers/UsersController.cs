using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Time_Records.DTO;
using Time_Records.Models;
using System.IdentityModel.Tokens.Jwt;
using Google.Apis.Auth;

namespace Time_Records.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase {
    private UserManager<AppUser> userManager;
    private IPasswordHasher<AppUser> passwordHasher;
    private IPasswordValidator<AppUser> passwordValidator;
    private IConfiguration configuration;

    public UsersController(UserManager<AppUser> userManager, IPasswordHasher<AppUser> passwordHasher, IPasswordValidator<AppUser> passwordValidator) {
        this.userManager = userManager;
        this.passwordHasher = passwordHasher;
        this.passwordValidator = passwordValidator;
    }

    // public UsersController(UserManager<AppUser> userManager, IPasswordHasher<AppUser> passwordHasher, IPasswordValidator<AppUser> passwordValidator, IConfiguration configuration) {
    //     this.userManager = userManager;
    //     this.passwordHasher = passwordHasher;
    //     this.passwordValidator = passwordValidator;
    //     this.configuration = configuration;
    // }

    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers() {
        var users = await userManager.Users.ToListAsync();
        return Ok(users);
    }
    
    [HttpGet("GetUserById/{id}")]
    public async Task<IActionResult> GetUserById(string id) {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) {
            return NotFound("User not found");
        }
        return Ok(user);
    }

    // [HttpGet("GetUserByIdQuery")]
    // public async Task<IActionResult> GetUserByIdQueryAsync([FromQuery] string userId) {
    //     if (string.IsNullOrEmpty(userId)) {
    //         throw new UnauthorizedAccessException("User not found");
    //     }
    //     var user = await userManager.Users
    //         .Where (us => us.Id == userId)
    //         .FirstOrDefaultAsync();
    //     if (user == null) {
    //         return NotFound("User ID was not found");
    //     }
    //     return Ok(user);
    // }
    
    [HttpGet("GetUserByIdQuery")]
    public async Task<IActionResult> GetUserByIdQueryAsync([FromQuery] Guid userId) {
        if (userId == Guid.Empty) {
            throw new UnauthorizedAccessException("User not found");
        }
        var user = await userManager.Users
            .Where(us => us.Id == userId)
            .FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User ID was not found");
        }
        return Ok(user);
    }
    
    [HttpGet("GetByUserEmail/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email) {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) {
            return NotFound("User not found");
        }
        return Ok(user);
    }
    
    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] AppUserDto newUser) {
        if (ModelState.IsValid) {
            var existingUser = await userManager.FindByEmailAsync(newUser.Email);
            if (existingUser != null) {
                return BadRequest("User with this email already exists");
            }
            AppUser appUser = new AppUser {
                UserName = newUser.UserName,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber,
                MonthTimeGoal = (newUser.MonthTimeGoal == null || newUser.MonthTimeGoal == 0) ? 15 : newUser.MonthTimeGoal
            };
            IdentityResult result = await userManager.CreateAsync(appUser, newUser.Password);
            if (result.Succeeded) {
                return Ok(new {success = true, user = new {appUser.Id, appUser.UserName, appUser.Email, appUser.PhoneNumber, appUser.MonthTimeGoal}});
            }
            return BadRequest(result.Errors);
        } else {
            return BadRequest(ModelState);
        }
    }
    
    // [HttpPut("EditUser/{id}")]
    // public async Task<IActionResult> EditUser(string id, [FromBody] AppUserDto editedUser) {
    //     var userToEdit = await userManager.FindByIdAsync(id);
    //     if (userToEdit == null) {
    //         return NotFound("User not found");
    //     }
    //     userToEdit.UserName = editedUser.UserName;
    //     userToEdit.Email = editedUser.Email;
    //     userToEdit.PhoneNumber = editedUser.PhoneNumber;
    //     userToEdit.MonthTimeGoal = (editedUser.MonthTimeGoal == null || editedUser.MonthTimeGoal == 0) ? 15 : editedUser.MonthTimeGoal;
    //     IdentityResult result = await userManager.UpdateAsync(userToEdit);
    //     if (result.Succeeded) {
    //         return Ok();
    //     } else {
    //         return BadRequest(result.Errors);
    //     }
    // }
    
    [HttpPut("EditUser/{id}")]
    public async Task<IActionResult> EditUser(Guid id, [FromBody] AppUserDto editedUser) {
        var userToEdit = await userManager.FindByIdAsync(id.ToString());  // Převedení Guid na string
        if (userToEdit == null) {
            return NotFound("User not found");
        }
        userToEdit.UserName = editedUser.UserName;
        userToEdit.Email = editedUser.Email;
        userToEdit.PhoneNumber = editedUser.PhoneNumber;
        userToEdit.MonthTimeGoal = (editedUser.MonthTimeGoal == null || editedUser.MonthTimeGoal == 0) ? 15 : editedUser.MonthTimeGoal;
        IdentityResult result = await userManager.UpdateAsync(userToEdit);
        if (result.Succeeded) {
            return Ok();
        } else {
            return BadRequest(result.Errors);
        }
    }
    
    // [HttpPut("EditUserByIdQuery")]
    // public async Task<IActionResult> EditUsrByIdQueryAsync([FromQuery] string userId, [FromBody] AppUserDto editedUser) {
    //     if (string.IsNullOrEmpty(userId)) {
    //         throw new UnauthorizedAccessException("No user is found");
    //     }
    //     var userToEdit = await userManager.Users
    //         .Where(user => user.Id == userId)
    //         .FirstOrDefaultAsync();
    //     if (userToEdit == null) {
    //         return NotFound("User ID was not found");
    //     }
    //     userToEdit.UserName = editedUser.UserName;
    //     userToEdit.Email = editedUser.Email;
    //     userToEdit.PhoneNumber = editedUser.PhoneNumber;
    //     userToEdit.MonthTimeGoal = editedUser.MonthTimeGoal;
    //     IdentityResult result = await userManager.UpdateAsync(userToEdit);
    //     if (result.Succeeded) {
    //         return Ok();
    //     } else {
    //         return BadRequest(result.Errors);
    //     }
    // }
    
    // [HttpPut("EditUserByIdQuery")]
    // public async Task<IActionResult> EditUsrByIdQueryAsync([FromQuery] Guid userId, [FromBody] AppUserDto editedUser) {
    //     if (userId == Guid.Empty) {
    //         throw new UnauthorizedAccessException("No user is found");
    //     }
    //     var userToEdit = await userManager.Users
    //         .Where(user => user.Id == userId)
    //         .FirstOrDefaultAsync();
    //     if (userToEdit == null) {
    //         return NotFound("User ID was not found");
    //     }
    //     userToEdit.UserName = editedUser.UserName;
    //     userToEdit.Email = editedUser.Email;
    //     userToEdit.PhoneNumber = editedUser.PhoneNumber;
    //     userToEdit.MonthTimeGoal = editedUser.MonthTimeGoal;
    //     IdentityResult result = await userManager.UpdateAsync(userToEdit);
    //     if (result.Succeeded) {
    //         return Ok();
    //     } else {
    //         return BadRequest(result.Errors);
    //     }
    // }
    
    [HttpPut("EditUserByIdQuery")]
    public async Task<IActionResult> EditUsrByIdQueryAsync([FromQuery] Guid userId, [FromBody] AppUserDto editedUser) {
        if (userId == Guid.Empty) {
            throw new UnauthorizedAccessException("No user is found");
        }
        var userToEdit = await userManager.Users
            .Where(user => user.Id == userId)
            .FirstOrDefaultAsync();
        if (userToEdit == null) {
            return NotFound("User ID was not found");
        }
        userToEdit.UserName = editedUser.UserName;
        userToEdit.Email = editedUser.Email;
        userToEdit.PhoneNumber = editedUser.PhoneNumber;
        userToEdit.MonthTimeGoal = editedUser.MonthTimeGoal;
        IdentityResult result = await userManager.UpdateAsync(userToEdit);
        if (result.Succeeded) {
            return Ok();
        } else {
            return BadRequest(result.Errors);
        }
    }
    
    [HttpDelete("DeleteUser/{id}")]
    public async Task<IActionResult> DeleteUser(string id) {
        var userToDelete = await userManager.FindByIdAsync(id);
        if (userToDelete == null) {
            return NotFound("User not found");
        }
        IdentityResult result = await userManager.DeleteAsync(userToDelete);
        if (result.Succeeded) {
            return Ok();
        } else {
            return BadRequest(result.Errors);
        }
    }

    // [HttpPut("EditMonthTimeGoal")]
    // public async Task<IActionResult> EditMonthTimeGoalAsyncQuery([FromQuery] string userId, [FromBody] AppUserDto editedUser) {
    //     if (string.IsNullOrEmpty(userId)) {
    //         throw new UnauthorizedAccessException("User not found");
    //     }
    //     var monthTimeGoalToEdit = await userManager.Users
    //         .FirstOrDefaultAsync(record => record.Id == userId);
    //     monthTimeGoalToEdit.MonthTimeGoal = editedUser.MonthTimeGoal;
    //     IdentityResult result = await userManager.UpdateAsync(monthTimeGoalToEdit);
    //     if (result.Succeeded) {
    //         return Ok();
    //     } else {
    //         return BadRequest(result.Errors);
    //     }
    // }
    
    [HttpPut("EditMonthTimeGoal")]
    public async Task<IActionResult> EditMonthTimeGoalAsyncQuery([FromQuery] Guid userId, [FromBody] AppUserDto editedUser) {
        if (userId == Guid.Empty) {
            throw new UnauthorizedAccessException("User not found");
        }
        var userToEdit = await userManager.Users
            .FirstOrDefaultAsync(record => record.Id == userId);
        if (userToEdit == null) {
            return NotFound("User ID was not found");
        }
        userToEdit.MonthTimeGoal = editedUser.MonthTimeGoal;
        IdentityResult result = await userManager.UpdateAsync(userToEdit);
        if (result.Succeeded) {
            return Ok();
        } else {
            return BadRequest(result.Errors);
        }
    }

}