using System.ComponentModel.DataAnnotations;

namespace TradeSpace.Models.ViewModels;

public class CreateStoreViewModel
{
    [Required(ErrorMessage = "Назва магазину є обов'язковою")]
    [StringLength(100, ErrorMessage = "Назва не повинна перевищувати 100 символів")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Опис магазину є обов'язковим")]
    [StringLength(500, ErrorMessage = "Опис не повинен перевищувати 500 символів")]
    public string Description { get; set; } = string.Empty;

    [Url(ErrorMessage = "Некоректне посилання на логотип")]
    public string? LogoUrl { get; set; }
}