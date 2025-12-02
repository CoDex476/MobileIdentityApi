using MobileIdentityApi.Models;

namespace MobileIdentityApi.Services
{
    public interface ICustomerService
    {
        Task<Customer?> GetCustomerByPhoneAsync(string phoneNumber);
        Task<CustomerInfo?> GetCustomerInfoAsync(string kycId);
        Task<AccountDetail?> GetAccountDetailAsync(string kycId);
    }
}
