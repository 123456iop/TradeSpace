using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TradeSpace.Models.ViewModels;

public class ProductFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Назва товару є обов'язковою")]
    [StringLength(100, ErrorMessage = "Назва не повинна перевищувати 100 символів")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Опис товару є обов'язковим")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажіть ціну")]
    [Range(0.01, 1000000, ErrorMessage = "Ціна повинна бути більшою за 0")]
    public decimal Price { get; set; }

    [Url(ErrorMessage = "Некоректний URL зображення")]
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Оберіть категорію")]
    public Guid CategoryId { get; set; }

    public IEnumerable<SelectListItem>? Categories { get; set; }
}