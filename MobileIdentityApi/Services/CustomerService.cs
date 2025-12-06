using Dapper;
using Microsoft.Data.SqlClient;
using MobileIdentityApi.Models;

namespace MobileIdentityApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IConfiguration _configuration;

        public CustomerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<Customer?> GetCustomerByPhoneAsync(string mobileNumber)
        {
            using var connection = GetConnection();

            var query = @"
                SELECT 
                    CUSTOMER_ID AS CustomerId,
                    MOBILE_NO AS MobileNo,
                    KYC_ID AS KycId
                FROM MOBAPP_USERS
                WHERE MOBILE_NO = @MobileNumber";

            return await connection.QueryFirstOrDefaultAsync<Customer>(
                query, new { MobileNumber = mobileNumber });
        }

        public async Task<CustomerInfo?> GetCustomerInfoAsync(string kycId)
        {
            using var connection = GetConnection();

            var query = @"
                SELECT
                    KYC_ID AS KycId,
                    FIRST_NAME AS FirstName,
                    SURNAME AS Surname,
                    GENDER_ID AS GenderId,
                    MARITAL_STATUS_ID AS MaritalStatusId,
                    DATE_OF_BIRTH AS DateOfBirth
                FROM KYC
                WHERE KYC_ID = @KycId";

            return await connection.QueryFirstOrDefaultAsync<CustomerInfo>(
                query, new { KycId = kycId });
        }

        public async Task<AccountDetail?> GetAccountDetailAsync(string kycId)
        {
            using var connection = GetConnection();

            var query = @"
                SELECT
                    ACCOUNT_NO AS AccountNo,
                    CUSTOMER_ID AS CustomerId,
                    USER_STATUS AS Status
                FROM MOBAPP_USERS
                WHERE KYC_ID = @KycId";

            return await connection.QueryFirstOrDefaultAsync<AccountDetail>(
                query, new { KycId = kycId });
        }

        public async Task<IEnumerable<Account>> GetAccountsByCustomerIdAsync(string customerId)
        {
            using var connection = GetConnection();

            var query = @"
                SELECT
                    CUSTOMER_ID AS CustomerId,
                    ACCT_NAME AS AccountName,
                    ACCOUNT_NO AS AccountNumber,
                    ACCOUNT_STATUS AS AccountStatus,
                    ACT_OPEN_DATE AS DateCreated
                FROM ACCOUNT_MASTER
                WHERE CUSTOMER_ID = @CustomerId";

            var accounts = await connection.QueryAsync<Account>(
                query, new { CustomerId = customerId });

            return accounts;
        }
    }
}
