using SharedClasses.Responses;

namespace BusinessLayer.Responses
{
    public class CustomerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Phone { get; set; }
        public string CompanyName { get; set; }
        public int CustomerMoney { get; set; }
        public string Address { get; set; }

        // Include the list of operations in the customer response
        public List<OperationResponse> Operations { get; set; }
    }
}
