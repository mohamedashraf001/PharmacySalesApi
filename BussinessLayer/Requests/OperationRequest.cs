using BusinessLayer.Requests;

namespace BusinessLayer.Requests
{
    public class OperationRequest
    {
        public string Operator { get; set; }  // Added
        public string BranchName { get; set; }
        public string OperationType { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public string OperationCustomerName { get; set; }  // Added
        public int CustomerPhone { get; set; }  // Phone number remains
    }
}
