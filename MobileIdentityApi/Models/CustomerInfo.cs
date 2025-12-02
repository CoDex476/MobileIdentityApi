namespace MobileIdentityApi.Models
{
    public class CustomerInfo
    {
        public string? KycId { get; set; }
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        //public string? OtherName { get; set; }
        public string? GenderId { get; set; }
        public string? MaritalStatusId { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string? FullName => $"{FirstName} {Surname}";
    }
}
