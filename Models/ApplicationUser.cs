using Microsoft.AspNetCore.Identity;

namespace MiniStationery.Mvc.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}