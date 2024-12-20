namespace DataLayer.Entities
{
    public class Customer
    {
        public int Id { get; set; }  
        public string Name { get; set; }  
        public int Phone { get; set; }
        public string CompanyName { get; set; }
        public int CustomerMoney { get; set; } 
        public string Address { get; set; }

        public List<Operation> Operations { get; set; }  
    }
}
