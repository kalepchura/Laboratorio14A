using Laboratorio14A.DTOs;
using Laboratorio14A.Services;
using Microsoft.AspNetCore.Mvc;

namespace Laboratorio14A.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductsController(ProductService service)
        {
            _service = service;
        }

        // 1. Insertar producto
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductRequest request)
        {
            var result = await _service.CreateAsync(request);

            return StatusCode(result.StatusCode, result.Body);
        }

        // 2. Eliminar producto
        [HttpDelete]
        public async Task<IActionResult> Delete(
            [FromBody] ProductIdRequest request)
        {
            var result = await _service.DeleteAsync(request);

            return StatusCode(result.StatusCode, result.Body);
        }

        // 7. Actualizar precio
        [HttpPut("price")]
        public async Task<IActionResult> UpdatePrice(
            [FromBody] UpdateProductPriceRequest request)
        {
            var result = await _service.UpdatePriceAsync(request);

            return StatusCode(result.StatusCode, result.Body);
        }

        // 9. Eliminar lista de productos
        [HttpDelete("batch")]
        public async Task<IActionResult> DeleteList(
            [FromBody] DeleteProductsRequest request)
        {
            var result = await _service.DeleteListAsync(request);

            return StatusCode(result.StatusCode, result.Body);
        }
    }
}