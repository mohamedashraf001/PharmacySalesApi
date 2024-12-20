using BusinessLayer.Responses;
using DataLayer.Entities;
using SharedClasses.Responses;

namespace DataLayer.Interfaces
{
    public interface IOperationRepository
    {
        Task<OperationResponse> CreateOperationAsync(Operation operation, int customerPhone);
        Task<Operation> DeleteOperationAsync(Operation operation);
        Task<IEnumerable<OperationResponse>> GetAllOperationsAsync();
        Task<Operation> GetOperationByIdAsync(int id);
        Task<OperationResponse> UpdateOperationAsync(int id, Operation operation);
    }
}
