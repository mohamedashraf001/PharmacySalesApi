using BusinessLayer.Responses;
using DataLayer.Entities;
using SharedClasses.Responses;

namespace DataLayer.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<CustomerResponse>> GetAllCustomersAsync();
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<CustomerResponse> CreateCustomerAsync(Customer customer);
        Task<CustomerResponse> UpdateCustomerAsync(int id, Customer customer);
        Task<Customer> DeleteCustomerAsync(Customer customer);

        // Modified to fetch customer by Phone instead of CustomerId
        Task<CustomerResponse> GetCustomerByPhoneAsync(int phone);
    }
}
