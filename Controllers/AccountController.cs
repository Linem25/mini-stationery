using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniStationery.Mvc.Models;
using MiniStationery.Mvc.Services;
using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuditLogService _auditLogService;

    public AccountController(SignInManager<ApplicationUser> signInManager, IAuditLogService auditLogService)
    {
        _signInManager = signInManager;
        _auditLogService = auditLogService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new AccountLoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AccountLoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            await _auditLogService.LogAsync("Login", "User", model.Email, "Success");
            return Redirect(returnUrl ?? "/");
        }

        await _auditLogService.LogAsync("Login", "User", model.Email, "Failed");
        ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}