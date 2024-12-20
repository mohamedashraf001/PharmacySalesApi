using BusinessLayer.Requests;
using BusinessLayer.Responses;

namespace BusinessLayer.Interfaces
{
    public interface IOperationService
    {
        Task<OperationResponse> CreateOperationAsync(OperationRequest request);
        Task<OperationResponse> UpdateOperationAsync(int id, OperationRequest request);
        Task<OperationResponse> GetOperationByIdAsync(int id);
        Task<IEnumerable<OperationResponse>> GetAllOperationsAsync();
        Task<OperationResponse> DeleteOperationAsync(int id);
    }
}
