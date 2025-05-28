using System.Threading.Tasks;
using BankingSystem.Models;

namespace BankingSystem
{
    public interface IBankService
    {
        Task<bool> AddBankAsync(string name, decimal depositRate, decimal creditFee);
        Task<bool> DeleteBankAsync(int bankId);
        Task<Bank> GetBankByIdAsync(int bankId);
        Task<IEnumerable<Bank>> GetAllBanksAsync();
    }

    public interface IAccountService
    {
        Task<bool> CreateAccountAsync(int clientId, string accountType, decimal initialAmount);
        Task<bool> BlockAccountAsync(int accountId);
        Task<bool> UnblockAccountAsync(int accountId);
        Task<bool> DepositMoneyAsync(int accountId, decimal amount);
        Task<bool> WithdrawMoneyAsync(int accountId, decimal amount);
        Task<IEnumerable<Account>> GetClientAccountsAsync(int clientId);
    }

    public interface IClientService
    {
        Task<int> RegisterClientAsync(string firstName, string lastName, int age, string address, string passportSeries, string passportNumber, int bankId);
        Task<Client> GetClientByIdAsync(int clientId);
        Task<IEnumerable<Client>> GetAllClientsAsync();
    }
} 