using Laboratorio14A.Data;
using Laboratorio14A.DTOs;
using Laboratorio14A.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Laboratorio14A.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Insertar producto
        public async Task<ServiceResult> CreateAsync(
            CreateProductRequest request)
        {
            if (decimal.Round(request.Price, 2) != request.Price)
            {
                return new(400, new
                {
                    message = "El precio admite máximo dos decimales."
                });
            }

            var product = new Product
            {
                Name = request.Name.Trim(),
                Price = request.Price
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new(201, new
            {
                message = "Producto registrado correctamente.",
                data = new
                {
                    product.Id,
                    product.Name,
                    product.Price
                }
            });
        }

        // 2. Eliminar producto
        public async Task<ServiceResult> DeleteAsync(
            ProductIdRequest request)
        {
            var product = await _context.Products
                .FindAsync(request.Id);

            if (product == null)
            {
                return new(404, new
                {
                    message = "Producto no encontrado."
                });
            }

            bool hasDetails = await _context.Details
                .AnyAsync(d => d.IdProduct == request.Id);

            if (hasDetails)
            {
                return new(409, new
                {
                    message = "El producto está asociado a una factura."
                });
            }

            _context.Products.Remove(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException pg &&
                pg.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                return new(409, new
                {
                    message = "El producto tiene registros asociados."
                });
            }

            return new(200, new
            {
                message = "Producto eliminado correctamente.",
                id = request.Id
            });
        }

        // 7. Actualizar precio del producto
        public async Task<ServiceResult> UpdatePriceAsync(
            UpdateProductPriceRequest request)
        {
            var product = await _context.Products
                .FindAsync(request.Id);

            if (product == null)
            {
                return new(404, new
                {
                    message = "Producto no encontrado."
                });
            }

            if (decimal.Round(request.Precio, 2) != request.Precio)
            {
                return new(400, new
                {
                    message = "El precio admite máximo dos decimales."
                });
            }

            decimal previousPrice = product.Price;

            product.Price = request.Precio;

            await _context.SaveChangesAsync();

            return new(200, new
            {
                message = "Precio actualizado correctamente.",
                data = new
                {
                    product.Id,
                    product.Name,
                    previousPrice,
                    currentPrice = product.Price
                }
            });
        }

        // 9. Eliminar lista de productos
        public async Task<ServiceResult> DeleteListAsync(
            DeleteProductsRequest request)
        {
            if (request.Products.Any(p => p == null || p.Id <= 0))
            {
                return new(400, new
                {
                    message = "La lista contiene productos inválidos."
                });
            }

            var ids = request.Products
                .Select(p => p.Id)
                .Distinct()
                .ToList();

            var products = await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            var missingIds = ids
                .Except(products.Select(p => p.Id))
                .ToList();

            if (missingIds.Count > 0)
            {
                return new(404, new
                {
                    message = "Algunos productos no existen.",
                    missingIds
                });
            }

            var blockedIds = await _context.Details
                .Where(d => ids.Contains(d.IdProduct))
                .Select(d => d.IdProduct)
                .Distinct()
                .ToListAsync();

            if (blockedIds.Count > 0)
            {
                return new(409, new
                {
                    message = "Existen productos asociados a facturas.",
                    blockedIds
                });
            }

            _context.Products.RemoveRange(products);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException pg &&
                pg.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                return new(409, new
                {
                    message = "Existen productos con registros asociados."
                });
            }

            return new(200, new
            {
                message = "Productos eliminados correctamente.",
                totalDeleted = products.Count,
                deletedIds = ids
            });
        }
    }
}