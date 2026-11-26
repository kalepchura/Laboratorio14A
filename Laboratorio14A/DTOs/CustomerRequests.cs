using System.ComponentModel.DataAnnotations;

namespace Laboratorio14A.DTOs
{
    public class CreateCustomerRequest
    {
        [Required]
        [MaxLength(45)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(45)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(45)]
        public string DocumentNumber { get; set; } = string.Empty;
    }

    public class UpdateCustomerDocumentRequest
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Required]
        [MaxLength(45)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;
    }

    public class CustomerIdRequest
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
    }
}