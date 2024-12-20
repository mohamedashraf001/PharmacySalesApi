using AutoMapper;
using BusinessLayer.Interfaces;
using BusinessLayer.Requests;
using BusinessLayer.Responses;
using DataAccessLayer.Data;
using DataLayer.Entities;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedClasses.Exceptions;

namespace BusinessLayer.Services
{
    public class OperationService : IOperationService
    {
        private readonly IOperationRepository _operationRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public OperationService(IOperationRepository operationRepository, IMapper mapper, AppDbContext context)
        {
            _operationRepository = operationRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<OperationResponse> CreateOperationAsync(OperationRequest request)
        {
            var operation = _mapper.Map<Operation>(request);
            return await _operationRepository.CreateOperationAsync(operation, request.CustomerPhone);
        }

        public async Task<OperationResponse> UpdateOperationAsync(int id, OperationRequest request)
        {
            var existingOperation = await _operationRepository.GetOperationByIdAsync(id);
            if (existingOperation == null)
                throw new NotFoundException("Operation not found.");

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Phone == request.CustomerPhone);
            if (customer == null)
                throw new KeyNotFoundException("Customer not found.");

            existingOperation.CustomerId = customer.Id;  // Update CustomerId from phone number
            _mapper.Map(request, existingOperation);

            var updatedOperation = await _operationRepository.UpdateOperationAsync(id, existingOperation);
            return _mapper.Map<OperationResponse>(updatedOperation);
        }

        public async Task<OperationResponse> GetOperationByIdAsync(int id)
        {
            var operation = await _operationRepository.GetOperationByIdAsync(id);
            if (operation == null)
                throw new NotFoundException("Operation not found.");

            return _mapper.Map<OperationResponse>(operation);
        }

        public async Task<IEnumerable<OperationResponse>> GetAllOperationsAsync()
        {
            var operations = await _operationRepository.GetAllOperationsAsync();
            return _mapper.Map<IEnumerable<OperationResponse>>(operations);
        }

        public async Task<OperationResponse> DeleteOperationAsync(int id)
        {
            var operation = await _operationRepository.GetOperationByIdAsync(id);
            if (operation == null)
                throw new NotFoundException("Operation not found.");

            var deletedOperation = await _operationRepository.DeleteOperationAsync(operation);
            return _mapper.Map<OperationResponse>(deletedOperation);
        }
    }
}
