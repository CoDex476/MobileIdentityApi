namespace MobileIdentityApi.Models
{
    public class Account
    {
        public string? CustomerId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountStatus { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
