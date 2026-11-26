using System.ComponentModel.DataAnnotations;

namespace Laboratorio14A.DTOs
{
    public class DetailItemRequest
    {
        [Range(1, int.MaxValue)]
        public int IdProduct { get; set; }

        [Range(1, int.MaxValue)]
        public int Amount { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999.99")]
        public decimal Price { get; set; }

        public decimal? SubTotal { get; set; }
    }

    public class CreateInvoiceDetailsRequest
    {
        [Range(1, int.MaxValue)]
        public int IdInvoice { get; set; }

        [Required]
        [MinLength(1)]
        public List<DetailItemRequest> Details { get; set; }
            = new();
    }
}