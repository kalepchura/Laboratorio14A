using Laboratorio14A.Data;
using Laboratorio14A.DTOs;
using Laboratorio14A.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Laboratorio14A.Services
{
    public class InvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 5. Insertar factura
        public async Task<ServiceResult> CreateAsync(
            CreateInvoiceRequest request)
        {
            return await SaveInvoicesAsync(
                request.IdCustomer,
                new List<InvoiceItemRequest> { request });
        }

        // 8. Insertar lista de facturas por cliente
        public async Task<ServiceResult> CreateListAsync(
            CreateInvoiceListRequest request)
        {
            return await SaveInvoicesAsync(
                request.IdCliente,
                request.Invoices);
        }

        // Lógica reutilizable para registrar facturas
        private async Task<ServiceResult> SaveInvoicesAsync(
            int customerId,
            List<InvoiceItemRequest> requests)
        {
            bool customerExists = await _context.Customers
                .AnyAsync(c => c.Id == customerId);

            if (!customerExists)
            {
                return new(404, new
                {
                    message = "Cliente no encontrado."
                });
            }

            if (requests.Any(r =>
                r == null ||
                string.IsNullOrWhiteSpace(r.InvoiceNumber) ||
                r.Date == default ||
                decimal.Round(r.Total, 2) != r.Total))
            {
                return new(400, new
                {
                    message = "La solicitud contiene facturas inválidas."
                });
            }

            var numbers = requests
                .Select(r => r.InvoiceNumber.Trim())
                .ToList();

            if (numbers.Distinct().Count() != numbers.Count)
            {
                return new(400, new
                {
                    message = "Existen números de factura repetidos."
                });
            }

            bool exists = await _context.Invoices
                .AnyAsync(i => numbers.Contains(i.InvoiceNumber));

            if (exists)
            {
                return new(409, new
                {
                    message = "Uno o más números de factura ya existen."
                });
            }

            var invoices = requests.Select(r => new Invoice
            {
                IdCustomer = customerId,
                Date = r.Date.UtcDateTime,
                InvoiceNumber = r.InvoiceNumber.Trim(),
                Total = r.Total
            }).ToList();

            _context.Invoices.AddRange(invoices);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException pg &&
                (pg.SqlState == PostgresErrorCodes.UniqueViolation ||
                 pg.SqlState == PostgresErrorCodes.ForeignKeyViolation))
            {
                return new(409, new
                {
                    message = "Conflicto al registrar las facturas."
                });
            }

            return new(201, new
            {
                message = "Facturas registradas correctamente.",
                totalCreated = invoices.Count,
                data = invoices.Select(i => new
                {
                    i.Id,
                    i.IdCustomer,
                    i.Date,
                    i.InvoiceNumber,
                    i.Total
                }).ToList()
            });
        }

        // 10. Insertar detalles de factura
        public async Task<ServiceResult> CreateDetailsAsync(
            CreateInvoiceDetailsRequest request)
        {
            if (request.Details.Any(d => d == null))
            {
                return new(400, new
                {
                    message = "La lista contiene detalles inválidos."
                });
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                var invoice = await _context.Invoices
                    .FindAsync(request.IdInvoice);

                if (invoice == null)
                {
                    return new(404, new
                    {
                        message = "Factura no encontrada."
                    });
                }

                var productIds = request.Details
                    .Select(d => d.IdProduct)
                    .Distinct()
                    .ToList();

                var existingIds = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync();

                var missingIds = productIds
                    .Except(existingIds)
                    .ToList();

                if (missingIds.Count > 0)
                {
                    return new(404, new
                    {
                        message = "Algunos productos no existen.",
                        missingIds
                    });
                }

                var details = new List<Detail>();

                foreach (var item in request.Details)
                {
                    if (item.Amount <= 0 ||
                        item.Price <= 0 ||
                        decimal.Round(item.Price, 2) != item.Price)
                    {
                        return new(400, new
                        {
                            message = "Cantidad o precio inválido."
                        });
                    }

                    decimal subtotal = item.Amount * item.Price;

                    if (subtotal > 9999999999.99m)
                    {
                        return new(400, new
                        {
                            message = "El subtotal supera el máximo permitido."
                        });
                    }

                    if (item.SubTotal.HasValue &&
                        item.SubTotal.Value != subtotal)
                    {
                        return new(400, new
                        {
                            message = "El subtotal enviado es incorrecto.",
                            expected = subtotal
                        });
                    }

                    details.Add(new Detail
                    {
                        IdInvoice = request.IdInvoice,
                        IdProduct = item.IdProduct,
                        Amount = item.Amount,
                        Price = item.Price,
                        SubTotal = subtotal
                    });
                }

                decimal previousTotal = await _context.Details
                    .Where(d => d.IdInvoice == request.IdInvoice)
                    .SumAsync(d => d.SubTotal);

                decimal newTotal = details.Sum(d => d.SubTotal);
                decimal accumulatedTotal = previousTotal + newTotal;

                if (accumulatedTotal > invoice.Total)
                {
                    return new(400, new
                    {
                        message = "Los detalles superan el total de la factura.",
                        invoiceTotal = invoice.Total,
                        accumulatedTotal
                    });
                }

                _context.Details.AddRange(details);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new(201, new
                {
                    message = "Detalles registrados correctamente.",
                    invoiceId = invoice.Id,
                    totalCreated = details.Count,
                    invoiceTotal = invoice.Total,
                    accumulatedTotal,
                    pendingAmount = invoice.Total - accumulatedTotal,
                    data = details.Select(d => new
                    {
                        d.Id,
                        d.IdProduct,
                        d.IdInvoice,
                        d.Amount,
                        d.Price,
                        d.SubTotal
                    }).ToList()
                });
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException pg &&
                (pg.SqlState == PostgresErrorCodes.ForeignKeyViolation ||
                 pg.SqlState == PostgresErrorCodes.SerializationFailure))
            {
                await transaction.RollbackAsync();

                return new(409, new
                {
                    message = "Conflicto al registrar los detalles."
                });
            }
            catch (PostgresException ex) when (
                ex.SqlState == PostgresErrorCodes.SerializationFailure)
            {
                await transaction.RollbackAsync();

                return new(409, new
                {
                    message = "Conflicto concurrente. Intente nuevamente."
                });
            }
        }
    }
}