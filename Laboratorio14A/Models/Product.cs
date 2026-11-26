using System.ComponentModel.DataAnnotations;

namespace Laboratorio14A.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(45)]
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public ICollection<Detail> Details { get; set; }
            = new List<Detail>();
    }
}