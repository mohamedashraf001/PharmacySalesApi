

using BusinessLayer.Responses;
using DataAccessLayer.Data;
using DataLayer.Entities;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerResponse>> GetAllCustomersAsync()
        {
            return await _context.Set<Customer>()
                .Select(c => new CustomerResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone,
                    CompanyName = c.CompanyName,
                    Address = c.Address,
                    CustomerMoney=c.CustomerMoney,
                }).ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _context.Set<Customer>().FindAsync(id);
        }

        public async Task<CustomerResponse> CreateCustomerAsync(Customer customer)
        {
            _context.Set<Customer>().Add(customer);
            await _context.SaveChangesAsync();

            return new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                CompanyName = customer.CompanyName,
                Address = customer.Address,
                CustomerMoney=customer.CustomerMoney,
            };
        }

        public async Task<CustomerResponse> UpdateCustomerAsync(int id, Customer customer)
        {
            var existingCustomer = await GetCustomerByIdAsync(id);
            if (existingCustomer == null) return null;

            existingCustomer.Name = customer.Name;
            existingCustomer.Phone = customer.Phone;
            existingCustomer.CompanyName = customer.CompanyName;
            existingCustomer.Address = customer.Address;
            existingCustomer.CustomerMoney = customer.CustomerMoney;

            await _context.SaveChangesAsync();

            return new CustomerResponse
            {
                Id = existingCustomer.Id,
                Name = existingCustomer.Name,
                Phone = existingCustomer.Phone,
                CompanyName = existingCustomer.CompanyName,
                Address = existingCustomer.Address,
                  CustomerMoney = existingCustomer.CustomerMoney
            };
        }

        public async Task<Customer> DeleteCustomerAsync(Customer customer)
        {
            _context.Set<Customer>().Remove(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<CustomerResponse> GetCustomerByPhoneAsync(int phone)
        {
            var customer = await _context.Set<Customer>().FirstOrDefaultAsync(c => c.Phone == phone);
            if (customer == null) return null;

            return new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                CompanyName = customer.CompanyName,
                Address = customer.Address,
                CustomerMoney = customer.CustomerMoney,
            };
        }


    }
}
