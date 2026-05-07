using System.ComponentModel.DataAnnotations;
using Graduation_Project.DAL.Models.Enums;

namespace Graduation_Project.BLL.DTOs.Cart
{
public class CartCustomizationDto
{
    [Required]
    public CustomizationZone Zone { get; set; }

    [Required]
    public int TechniqueId { get; set; }

    public string? DesignImageUrl { get; set; }

    public string? DesignText { get; set; }
}}