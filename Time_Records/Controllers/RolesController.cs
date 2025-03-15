using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Time_Records.Models;

namespace Time_Records.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase {
    private RoleManager<IdentityRole> roleManager;
    private UserManager<AppUser> userManager;

    public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager) {
        this.roleManager = roleManager;
        this.userManager = userManager;
    }
    
    [HttpGet("GetAllRoles")]
    public async Task<IActionResult> GetAllRoles() {
        var roles = await roleManager.Roles.ToListAsync();
        return Ok(roles);
    }
    
    [HttpGet("GetRoleById/{id}")]
    public async Task<IActionResult> GetRoleById(string id) {
        var roleToEdit = await roleManager.FindByIdAsync(id);
        if (roleToEdit == null) {
            return NotFound("Role not found");
        }
        var users = await userManager.Users.AsNoTracking().ToListAsync();
        var members = new List<AppUser>();
        var nonMembers = new List<AppUser>();
        foreach (var user in users) {
            var isInRole = await userManager.IsInRoleAsync(user, roleToEdit.Name);
            if (isInRole) {
                members.Add(user);
            } else {
                nonMembers.Add(user);
            }
        }
        return Ok(new RoleMembersModel {
            Role = roleToEdit,
            Members = members,
            NonMembers = nonMembers
        });
    }
    
    [HttpPost("CreateRole")]
    public async Task<IActionResult> CreateRole(string roleName) {
        var existingRole = await roleManager.FindByNameAsync(roleName);
        if (existingRole != null) {
            return BadRequest("Role with this name already exists");
        }
        IdentityResult result = await roleManager.CreateAsync(new IdentityRole(roleName));
        if (result.Succeeded) {
            return Ok();
        }
        return BadRequest(result.Errors);
    }
    
    [HttpPut("ModificationsRoleEdit")]
    public async Task<IActionResult> ModificationsRoleEdit(UserRoleModification modification) {
        foreach (string userId in modification.AddIds ?? Array.Empty<string>()) {
            AppUser user = await userManager.FindByIdAsync(userId);
            if (user != null) {
                IdentityResult result = await userManager.AddToRoleAsync(user, modification.RoleName);
                if (!result.Succeeded) {
                    return BadRequest(result.Errors);
                }
            }
        }
        foreach (string userId in modification.DeleteIds ?? Array.Empty<string>()) {
            AppUser user = await userManager.FindByIdAsync(userId);
            if (user != null) {
                IdentityResult result = await userManager.RemoveFromRoleAsync(user, modification.RoleName);
                if (!result.Succeeded) {
                    AddModelErrors(result);
                }
            }
        }
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        return Ok();
    }
    private void AddModelErrors(IdentityResult result) {
        foreach (var error in result.Errors) {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
        
    [HttpDelete("DeleteRole/{id}")]
    public async Task<IActionResult> DeleteRole(string id) {
        IdentityRole foundRole = await roleManager.FindByIdAsync(id);
        if (foundRole != null) {
            IdentityResult result = await roleManager.DeleteAsync(foundRole);
            if (result.Succeeded) {
                return Ok(new {message = "Role deleted successfully"});
            } else {
                return BadRequest(new {errors = result.Errors, message = "Error deleting role"});
            }
        } else {
            return NotFound(new {message = "Role not found"});
        }
    }
}