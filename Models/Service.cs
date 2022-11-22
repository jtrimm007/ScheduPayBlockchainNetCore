namespace ScheduPayBlockchainNetCore.Models
{
    public class Service
    {
        public string Description { get; set; }
        public double Amount { get; set; }

        public Service(string description, double amount)
        {
            Description = description;
            Amount = amount;
        }

        public Service()
        {
        }
    }
}