using System.ComponentModel.DataAnnotations;

namespace Laboratorio14A.DTOs
{
    public class CreateProductRequest
    {
        [Required]
        [MaxLength(45)]
        public string Name { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "9999999999.99")]
        public decimal Price { get; set; }
    }

    public class UpdateProductPriceRequest
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999.99")]
        public decimal Precio { get; set; }

        public string? LastName { get; set; }
    }

    public class ProductIdRequest
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
    }

    public class DeleteProductsRequest
    {
        [Required]
        [MinLength(1)]
        public List<ProductIdRequest> Products { get; set; }
            = new();
    }
}