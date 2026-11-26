using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laboratorio14A.Models
{
    public class Detail
    {
        [Key]
        public int Id { get; set; }

        public int IdProduct { get; set; }

        public int IdInvoice { get; set; }

        public int Amount { get; set; }

        public decimal Price { get; set; }

        public decimal SubTotal { get; set; }

        [ForeignKey(nameof(IdProduct))]
        public Product Product { get; set; } = null!;

        [ForeignKey(nameof(IdInvoice))]
        public Invoice Invoice { get; set; } = null!;
    }
}