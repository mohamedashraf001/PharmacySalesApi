using BusinessLayer.Requests;
using BusinessLayer.Responses;
using BussinessLayer.Interfaces;
using DataLayer.Entities;
using DataLayer.Interfaces;
using SharedClasses.Responses;

namespace BussinessLayer.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerResponse> CreateCustomerAsync(CustomerRequest customerRequest)
        {
            var customer = new Customer
            {
                Name = customerRequest.Name,
                Phone = customerRequest.Phone,
                CompanyName = customerRequest.CompanyName,
                CustomerMoney = customerRequest.CustomerMoney,
                Address = customerRequest.Address
            };

            var createdCustomer = await _customerRepository.CreateCustomerAsync(customer);
            return createdCustomer;
        }

        public async Task<CustomerResponse> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
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

        public async Task<CustomerResponse> GetCustomerByPhoneAsync(int phone)
        {
            var customer = await _customerRepository.GetCustomerByPhoneAsync(phone);
            if (customer == null) return null;

            return customer;
        }

        public async Task<IEnumerable<CustomerResponse>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllCustomersAsync();
            return customers;
        }

        public async Task<CustomerResponse> UpdateCustomerAsync(int id, CustomerRequest customerRequest)
        {
            var customer = new Customer
            {
                Name = customerRequest.Name,
                Phone = customerRequest.Phone,
                CompanyName = customerRequest.CompanyName,
                CustomerMoney = customerRequest.CustomerMoney,
                Address = customerRequest.Address
            };

            var updatedCustomer = await _customerRepository.UpdateCustomerAsync(id, customer);
            return updatedCustomer;
        }

        public async Task<CustomerResponse> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null) return null;

            await _customerRepository.DeleteCustomerAsync(customer);
            return new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                CompanyName = customer.CompanyName,
                Address = customer.Address
            };
        }
    }
}
