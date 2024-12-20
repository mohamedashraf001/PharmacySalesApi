using BusinessLayer.Responses;

namespace BusinessLayer.Responses
{
    public class OperationResponse
    {
        public int Id { get; set; }
        public string Operator { get; set; }  // Updated
        public string BranchName { get; set; }
        public string OperationType { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public string OperationCustomerName { get; set; }  // Updated
        public DateTime CreatedAt { get; set; }

        // Include customer details in the operation response
        public CustomerResponse Customer { get; set; }
    }
}
