using System.ComponentModel.DataAnnotations;

namespace DailyWorkReport.ViewModels.User;

public class UserCreateViewModel
{
    [Required]
    public string UserName { get; init; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; init; } = string.Empty;
    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; init; } = string.Empty;
    [Required]
    public string RoleName { get; init; } = string.Empty;
}