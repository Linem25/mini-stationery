using System.ComponentModel.DataAnnotations;

namespace MiniStationery.Mvc.ViewModels;

public class AccountLoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}