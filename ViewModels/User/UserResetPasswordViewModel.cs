using System.ComponentModel.DataAnnotations;

namespace DailyWorkReport.ViewModels.User;

public class UserResetPasswordViewModel
{
    public string Id { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; init; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; init; } = string.Empty;
}