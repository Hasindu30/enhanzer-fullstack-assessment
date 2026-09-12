using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("Location_Details")]
public class LocationDetail
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string LocationCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string LocationName { get; set; } = string.Empty;
}
