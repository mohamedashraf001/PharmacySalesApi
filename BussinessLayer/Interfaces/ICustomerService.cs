using BusinessLayer.Requests;
using BusinessLayer.Responses;
using SharedClasses.Responses;

namespace BussinessLayer.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponse> CreateCustomerAsync(CustomerRequest customerRequest);
        Task<CustomerResponse> GetCustomerByIdAsync(int id);
        Task<IEnumerable<CustomerResponse>> GetAllCustomersAsync();
        Task<CustomerResponse> UpdateCustomerAsync(int id, CustomerRequest customerRequest);
        Task<CustomerResponse> DeleteCustomerAsync(int id);

        // Modified to use Phone instead of CustomerId
        Task<CustomerResponse> GetCustomerByPhoneAsync(int phone);
    }
}
