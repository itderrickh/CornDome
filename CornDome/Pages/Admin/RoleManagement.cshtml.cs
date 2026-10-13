using CornDome.Models;
using CornDome.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Pages.Admin;

[Authorize(Policy = "admin")]
public class RoleManagementModel(RoleManager<Role> roleManager, UserManager<User> userManager) : BasePageModel
{

    public IList<Role> Roles { get; private set; } = new List<Role>();

    [BindProperty]
    public string RoleName { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        await LoadRolesAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        RoleName = RoleName.Trim();

        if (string.IsNullOrWhiteSpace(RoleName))
        {
            ModelState.AddModelError(
                nameof(RoleName),
                "Please enter a role name.");

            await LoadRolesAsync();
            return Page();
        }

        if (await roleManager.RoleExistsAsync(RoleName))
        {
            ModelState.AddModelError(
                nameof(RoleName),
                "A role with this name already exists.");

            await LoadRolesAsync();
            return Page();
        }

        var role = new Role
        {
            Name = RoleName
        };

        var result = await roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await LoadRolesAsync();
            return Page();
        }

        TempData["SuccessMessage"] =
            $"Role '{RoleName}' was created successfully.";

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            TempData["ErrorMessage"] = "Invalid role ID.";
            return RedirectToPage();
        }

        var role = await roleManager.FindByIdAsync(id);

        if (role == null)
        {
            TempData["ErrorMessage"] = "Role not found.";
            return RedirectToPage();
        }

        if (string.Equals(
            role.Name,
            "Admin",
            StringComparison.OrdinalIgnoreCase))
        {
            TempData["ErrorMessage"] =
                "The Admin role cannot be deleted.";
            return RedirectToPage();
        }

        if (!string.IsNullOrEmpty(role.Name))
        {
            var usersInRole =
                await userManager.GetUsersInRoleAsync(role.Name);

            if (usersInRole.Count > 0)
            {
                TempData["ErrorMessage"] =
                    $"Cannot delete '{role.Name}' because it is assigned to " +
                    $"{usersInRole.Count} user(s). Remove those assignments first.";

                return RedirectToPage();
            }
        }

        var result = await roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join(
                " ",
                result.Errors.Select(error => error.Description));

            return RedirectToPage();
        }

        TempData["SuccessMessage"] =
            $"Role '{role.Name}' was deleted successfully.";

        return RedirectToPage();
    }

    private async Task LoadRolesAsync()
    {
        Roles = await roleManager.Roles
            .OrderBy(role => role.Name)
            .ToListAsync();
    }
}
