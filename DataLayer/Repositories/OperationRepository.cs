using BusinessLayer.Responses;
using DataAccessLayer.Data;
using DataLayer.Entities;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedClasses.Responses;

namespace DataLayer.Repositories
{
    public class OperationRepository : IOperationRepository
    {
        private readonly AppDbContext _context;

        public OperationRepository(AppDbContext context)
        {
            _context = context;
        }

        private async Task<CustomerResponse> GetCustomerResponseByIdAsync(int customerId)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == customerId);
            return customer == null ? null : new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                CompanyName = customer.CompanyName,
                Address = customer.Address
            };
        }

        public async Task<OperationResponse> CreateOperationAsync(Operation operation, int customerPhone)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Phone == customerPhone);

            if (customer == null)
                throw new KeyNotFoundException("Customer not found.");

            operation.CustomerId = customer.Id;  // Set the CustomerId from the found customer

            if (operation.OperationType.ToLower() == "add")
            {
                customer.CustomerMoney += operation.Price;  // Increase money
            }
            else if (operation.OperationType.ToLower() == "deduct")
            {
                if (customer.CustomerMoney < operation.Price)
                {
                    throw new InvalidOperationException("Insufficient funds to perform deduction.");
                }
                customer.CustomerMoney -= operation.Price;  // Decrease money
            }
            else
            {
                throw new InvalidOperationException("Invalid operation type.");
            }

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();  // Save changes for the customer entity first

            _context.Operations.Add(operation);  // Add the operation record
            await _context.SaveChangesAsync();  // Save the operation

            return new OperationResponse
            {
                Id = operation.Id,
                Operator = operation.Operator,  // Added
                BranchName = operation.BranchName,
                Price = operation.Price,
                Description = operation.Description,
                OperationType = operation.OperationType,
                OperationCustomerName = operation.OperationCustomerName,  // Added
                CreatedAt = operation.CreatedAt,
                Customer = new CustomerResponse
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Phone = customer.Phone,
                    CompanyName = customer.CompanyName,
                    Address = customer.Address,
                    CustomerMoney = customer.CustomerMoney,
                }
            };
        }

        public async Task<Operation> DeleteOperationAsync(Operation operation)
        {
            _context.Operations.Remove(operation);
            await _context.SaveChangesAsync();
            return operation;
        }

        public async Task<IEnumerable<OperationResponse>> GetAllOperationsAsync()
        {
            var operations = await _context.Operations
                .AsNoTracking()
                .ToListAsync();

            var operationResponses = new List<OperationResponse>();
            foreach (var operation in operations)
            {
                var customerResponse = await GetCustomerResponseByIdAsync(operation.CustomerId);
                operationResponses.Add(new OperationResponse
                {
                    Id = operation.Id,
                    Operator = operation.Operator,  // Added
                    BranchName = operation.BranchName,
                    Price = operation.Price,
                    Description = operation.Description,
                    OperationType = operation.OperationType,
                    OperationCustomerName = operation.OperationCustomerName,  // Added
                    CreatedAt = operation.CreatedAt,
                    Customer = customerResponse
                });
            }

            return operationResponses;
        }

        public async Task<Operation> GetOperationByIdAsync(int id)
        {
            return await _context.Operations
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<OperationResponse> UpdateOperationAsync(int id, Operation operation)
        {
            var existingEntity = await _context.Operations.FindAsync(id);
            if (existingEntity == null)
                throw new KeyNotFoundException("Operation not found.");

            existingEntity.Operator = operation.Operator;  // Updated
            existingEntity.BranchName = operation.BranchName;
            existingEntity.Price = operation.Price;
            existingEntity.Description = operation.Description;
            existingEntity.OperationType = operation.OperationType;
            existingEntity.OperationCustomerName = operation.OperationCustomerName;  // Updated

            await _context.SaveChangesAsync();

            var customerResponse = await GetCustomerResponseByIdAsync(existingEntity.CustomerId);

            return new OperationResponse
            {
                Id = existingEntity.Id,
                Operator = existingEntity.Operator,  // Updated
                BranchName = existingEntity.BranchName,
                Price = existingEntity.Price,
                Description = existingEntity.Description,
                OperationType = existingEntity.OperationType,
                OperationCustomerName = existingEntity.OperationCustomerName,  // Updated
                CreatedAt = existingEntity.CreatedAt,
                Customer = customerResponse
            };
        }
    }
}
