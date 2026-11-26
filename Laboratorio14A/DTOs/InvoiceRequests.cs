using System.ComponentModel.DataAnnotations;

namespace Laboratorio14A.DTOs
{
    public class InvoiceItemRequest
    {
        public required DateTimeOffset Date { get; set; }

        [Required]
        [MaxLength(45)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "9999999999.99")]
        public required decimal Total { get; set; }
    }

    public class CreateInvoiceRequest : InvoiceItemRequest
    {
        [Range(1, int.MaxValue)]
        public int IdCustomer { get; set; }
    }

    public class CreateInvoiceListRequest
    {
        [Range(1, int.MaxValue)]
        public int IdCliente { get; set; }

        [Required]
        [MinLength(1)]
        public List<InvoiceItemRequest> Invoices { get; set; }
            = new();
    }
}