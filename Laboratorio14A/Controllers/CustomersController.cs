using Laboratorio14A.DTOs;
using Laboratorio14A.Services;
using Microsoft.AspNetCore.Mvc;

namespace Laboratorio14A.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService _service;

        public CustomersController(CustomerService service)
        {
            _service = service;
        }

        // 3. Insertar cliente
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCustomerRequest request)
        {
            var result = await _service.CreateAsync(request);

            return StatusCode(result.StatusCode, result.Body);
        }

        // 4. Eliminar cliente
        [HttpDelete]
        public async Task<IActionResult> Delete(
            [FromBody] CustomerIdRequest request)
        {
            var result = await _service.DeleteAsync(request);

            return StatusCode(result.StatusCode, result.Body);
        }

        // 6. Actualizar documento y correo
        [HttpPut("document")]
        public async Task<IActionResult> UpdateDocument(
            [FromBody] UpdateCustomerDocumentRequest request)
        {
            var result = await _service.UpdateDocumentAsync(request);

            return StatusCode(result.StatusCode, result.Body);
        }
    }
}