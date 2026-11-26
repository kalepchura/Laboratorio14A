using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laboratorio14A.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        public int IdCustomer { get; set; }

        public DateTime Date { get; set; }

        [Required]
        [MaxLength(45)]
        public string InvoiceNumber { get; set; } = string.Empty;

        public decimal Total { get; set; }

        [ForeignKey(nameof(IdCustomer))]
        public Customer Customer { get; set; } = null!;

        public ICollection<Detail> Details { get; set; }
            = new List<Detail>();
    }
}