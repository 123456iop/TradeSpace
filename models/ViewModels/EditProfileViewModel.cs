using System.ComponentModel.DataAnnotations;

namespace TradeSpace.Models.ViewModels;

public class EditProfileViewModel
{
    [Required(ErrorMessage = "Введіть ім'я")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введіть прізвище")]
    public string LastName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Введіть коректний номер телефону")]
    public string? PhoneNumber { get; set; }
}