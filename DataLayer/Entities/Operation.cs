namespace DataLayer.Entities
{
    public class Operation
    {
        public int Id { get; set; }
        public string Operator { get; set; }
        public string BranchName { get; set; }
        public string OperationType { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public string OperationCustomerName { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
