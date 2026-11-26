using Laboratorio14A.DTOs;
using Laboratorio14A.Services;
using Microsoft.AspNetCore.Mvc;

namespace Laboratorio14A.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceService _service;

        public InvoicesController(InvoiceService service)
        {
            _service = service;
        }

        // 5. Insertar factura
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateInvoiceRequest request)
        {
            var result = await _service.CreateAsync(request);

            return StatusCode(result.StatusCode, result.Body);
        }

        // 8. Insertar lista de facturas
        [HttpPost("batch")]
        public async Task<IActionResult> CreateList(
            [FromBody] CreateInvoiceListRequest request)
        {
            var result = await _service.CreateListAsync(request);

            return StatusCode(result.StatusCode, result.Body);
        }

        // 10. Insertar detalles de factura
        [HttpPost("details")]
        public async Task<IActionResult> CreateDetails(
            [FromBody] CreateInvoiceDetailsRequest request)
        {
            var result = await _service.CreateDetailsAsync(request);

            return StatusCode(result.StatusCode, result.Body);
        }
    }
}