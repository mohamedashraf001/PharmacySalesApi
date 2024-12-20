using BusinessLayer.Interfaces;
using BusinessLayer.Requests;
using BusinessLayer.Responses;
using Microsoft.AspNetCore.Mvc;
using SharedClasses.Responses;
using SharedClasses.Exceptions;
using Microsoft.Extensions.Logging;

namespace MusicAppApi.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OperationsController : ControllerBase
    {
        private readonly IOperationService _operationService;
        private readonly ILogger<OperationsController> _logger;

        public OperationsController(IOperationService operationService, ILogger<OperationsController> logger)
        {
            _operationService = operationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IApiResponse>> GetAllOperations()
        {
            try
            {
                var operations = await _operationService.GetAllOperationsAsync();
                return Ok(ApiResponseFactory.Create(operations, "Operations fetched successfully", 200, true));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An error occurred while fetching operations.");
                return StatusCode(500, ApiResponseFactory.Create(ex.Message, 500, false));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<IApiResponse>> GetOperationById(int id)
        {
            try
            {
                var operation = await _operationService.GetOperationByIdAsync(id);
                return Ok(ApiResponseFactory.Create(operation, "Operation fetched successfully", 200, true));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Operation not found for ID {Id}", id);
                return NotFound(ApiResponseFactory.Create(ex.Message, 404, false));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error occurred while fetching operation with ID {Id}.", id);
                return StatusCode(500, ApiResponseFactory.Create(ex.Message, 500, false));
            }
        }

        [HttpPost]
        public async Task<ActionResult<IApiResponse>> CreateOperation(OperationRequest request)
        {
            try
            {
                var createdOperation = await _operationService.CreateOperationAsync(request);
                return CreatedAtAction(nameof(GetOperationById), new { id = createdOperation.Id }, ApiResponseFactory.Create(createdOperation, "Operation created successfully", 201, true));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error occurred while creating operation.");
                return StatusCode(500, ApiResponseFactory.Create(ex.Message, 500, false));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<IApiResponse>> UpdateOperation(int id, OperationRequest request)
        {
            try
            {
                var updatedOperation = await _operationService.UpdateOperationAsync(id, request);
                return Ok(ApiResponseFactory.Create(updatedOperation, "Operation updated successfully", 200, true));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Operation not found for update ID {Id}", id);
                return NotFound(ApiResponseFactory.Create(ex.Message, 404, false));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error occurred while updating operation.");
                return StatusCode(500, ApiResponseFactory.Create(ex.Message, 500, false));
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<IApiResponse>> DeleteOperation(int id)
        {
            try
            {
                var deletedOperation = await _operationService.DeleteOperationAsync(id);
                return Ok(ApiResponseFactory.Create(deletedOperation, "Operation deleted successfully", 200, true));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Operation not found for deletion ID {Id}", id);
                return NotFound(ApiResponseFactory.Create(ex.Message, 404, false));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error occurred while deleting operation.");
                return StatusCode(500, ApiResponseFactory.Create(ex.Message, 500, false));
            }
        }
    }
}
