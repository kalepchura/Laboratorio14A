using Laboratorio14A.Data;
using Laboratorio14A.DTOs;
using Laboratorio14A.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Laboratorio14A.Services
{
    public class CustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 3. Insertar cliente
        public async Task<ServiceResult> CreateAsync(
            CreateCustomerRequest request)
        {
            var document = request.DocumentNumber.Trim();

            bool exists = await _context.Customers
                .AnyAsync(c => c.DocumentNumber == document);

            if (exists)
            {
                return new(409, new
                {
                    message = "El número de documento ya está registrado."
                });
            }

            var customer = new Customer
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                DocumentNumber = document
            };

            _context.Customers.Add(customer);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException pg &&
                pg.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return new(409, new
                {
                    message = "El número de documento ya existe."
                });
            }

            return new(201, new
            {
                message = "Cliente registrado correctamente.",
                data = new
                {
                    customer.Id,
                    customer.FirstName,
                    customer.LastName,
                    customer.DocumentNumber,
                    customer.Email
                }
            });
        }

        // 4. Eliminar cliente
        public async Task<ServiceResult> DeleteAsync(
            CustomerIdRequest request)
        {
            var customer = await _context.Customers
                .FindAsync(request.Id);

            if (customer == null)
            {
                return new(404, new
                {
                    message = "Cliente no encontrado."
                });
            }

            bool hasInvoices = await _context.Invoices
                .AnyAsync(i => i.IdCustomer == request.Id);

            if (hasInvoices)
            {
                return new(409, new
                {
                    message = "El cliente tiene facturas registradas."
                });
            }

            _context.Customers.Remove(customer);

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
                    message = "El cliente tiene registros asociados."
                });
            }

            return new(200, new
            {
                message = "Cliente eliminado correctamente.",
                id = request.Id
            });
        }

        // 6. Actualizar documento y correo
        public async Task<ServiceResult> UpdateDocumentAsync(
            UpdateCustomerDocumentRequest request)
        {
            var customer = await _context.Customers
                .FindAsync(request.Id);

            if (customer == null)
            {
                return new(404, new
                {
                    message = "Cliente no encontrado."
                });
            }

            var document = request.DocumentNumber.Trim();

            bool exists = await _context.Customers
                .AnyAsync(c =>
                    c.DocumentNumber == document &&
                    c.Id != request.Id);

            if (exists)
            {
                return new(409, new
                {
                    message = "El documento pertenece a otro cliente."
                });
            }

            customer.DocumentNumber = document;
            customer.Email = request.Email.Trim();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException pg &&
                pg.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return new(409, new
                {
                    message = "El número de documento ya existe."
                });
            }

            return new(200, new
            {
                message = "Cliente actualizado correctamente.",
                data = new
                {
                    customer.Id,
                    customer.FirstName,
                    customer.LastName,
                    customer.DocumentNumber,
                    customer.Email
                }
            });
        }
    }
}