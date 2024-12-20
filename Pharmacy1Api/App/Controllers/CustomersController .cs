using AutoMapper;
using BusinessLayer.Requests;
using BusinessLayer.Responses;
using BussinessLayer.Interfaces;
using HMS.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using SharedClasses.Exceptions;
using SharedClasses.Responses;

namespace MusicAppApi.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger, IMapper mapper)
        {
            _customerService = customerService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IApiResponse>> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                var customerResponses = _mapper.Map<List<CustomerResponse>>(customers);
                return Ok(ApiResponseFactory.Create(customerResponses, "Customers fetched successfully", 200, true));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An error occurred while fetching customers.");
                return StatusCode(500, ApiResponseFactory.Create(ex.Message, 500, false));
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<IApiResponse>> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                var customerResponse = _mapper.Map<CustomerResponse>(customer);
                return Ok(ApiResponseFactory.Create(customerResponse, "Customer fetched successfully", 200, true));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Customer with ID: {Id} not found.", id);
                return NotFound(ApiResponseFactory.Create(ex.Message, 404, false));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An error occurred while fetching customer with ID: {Id}", id);
                return StatusCode(500, ApiResponseFactory.Create(ex.Message, 500, false));
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<IApiResponse>> Add(CustomerRequest request)
        {
            try
            {
                var addedCustomer = await _customerService.CreateCustomerAsync(request);
                var customerResponse = _mapper.Map<CustomerResponse>(addedCustomer);
                return Ok(ApiResponseFactory.Create(customerResponse, "Customer added successfully", 200, true));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An error occurred while adding customer.");
                return StatusCode(500, ApiResponseFactory.Create(ex.Message, 500, false));
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<IApiResponse>> Update(int id, CustomerRequest request)
        {
            try
            {
                var updatedCustomer = await _customerService.UpdateCustomerAsync(id, request);
                var customerResponse = _mapper.Map<CustomerResponse>(updatedCustomer);
                return Ok(ApiResponseFactory.Create(customerResponse, "Customer updated successfully", 200, true));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Customer with ID: {Id} not found for update.", id);
                return NotFound(ApiResponseFactory.Create(ex.Message, 404, false));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An error occurred while updating customer with ID: {Id}", id);
                return StatusCode(500, ApiResponseFactory.Create(ex.Message, 500, false));
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<IApiResponse>> Delete(int id)
        {
            try
            {
                var deletedCustomer = await _customerService.DeleteCustomerAsync(id);
                return Ok(ApiResponseFactory.Create(deletedCustomer, "Customer deleted successfully", 200, true));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Customer with ID: {Id} not found for deletion.", id);
                return NotFound(ApiResponseFactory.Create(ex.Message, 404, false));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An error occurred while deleting customer with ID: {Id}", id);
                return StatusCode(500, ApiResponseFactory.Create(ex.Message, 500, false));
            }
        }

        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<IApiResponse>> GetCustomerByPhone([FromQuery] int phone)
        {
            try
            {
                var customer = await _customerService.GetCustomerByPhoneAsync(phone);
                if (customer == null)
                {
                    return NotFound(ApiResponseFactory.Create("Customer with this phone number not found.", 404, false));
                }

                return Ok(ApiResponseFactory.Create(customer, "Customer fetched successfully", 200, true));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error occurred while fetching customer with phone: {Phone}", phone);
                return StatusCode(500, ApiResponseFactory.Create(ex.Message, 500, false));
            }
        }
    }
}
