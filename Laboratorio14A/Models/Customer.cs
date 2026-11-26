using System.ComponentModel.DataAnnotations;

namespace Laboratorio14A.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(45)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(45)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(45)]
        public string DocumentNumber { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
            = new List<Invoice>();
    }
}