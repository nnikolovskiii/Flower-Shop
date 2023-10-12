/*using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var newUser = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                // User created successfully, you might redirect to a success page or log the user in.
            }
            else
            {
                // Handle errors from result.Errors
            }
        }

        return View(model); // Return the view with validation errors if any.
    }
}
*/