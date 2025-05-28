using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BankingSystem.Models;
using BankingSystem.DbControllers;

namespace BankingSystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly AccountRepository _accountRepository;
        private readonly ClientRepository _clientRepository;

        public AccountService(string connectionString)
        {
            _accountRepository = new AccountRepository(connectionString);
            _clientRepository = new ClientRepository(connectionString);
        }

        public async Task<bool> CreateAccountAsync(int clientId, string accountType, decimal initialAmount)
        {
            if (clientId <= 0)
                throw new ArgumentException("Invalid client ID", nameof(clientId));
            if (string.IsNullOrWhiteSpace(accountType))
                throw new ArgumentException("Account type cannot be empty", nameof(accountType));
            if (initialAmount < 0)
                throw new ArgumentException("Initial amount cannot be negative", nameof(initialAmount));

            var client = await _clientRepository.GetClientByIdAsync(clientId);
            if (client == null)
                throw new ArgumentException("Client not found", nameof(clientId));

            return await _accountRepository.CreateAccountAsync(clientId, accountType, initialAmount);
        }

        public async Task<bool> BlockAccountAsync(int accountId)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID", nameof(accountId));

            return await _accountRepository.BlockAccountAsync(accountId);
        }

        public async Task<bool> UnblockAccountAsync(int accountId)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID", nameof(accountId));

            return await _accountRepository.UnblockAccountAsync(accountId);
        }

        public async Task<bool> DepositMoneyAsync(int accountId, decimal amount)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID", nameof(accountId));
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be positive", nameof(amount));

            return await _accountRepository.DepositMoneyAsync(accountId, amount);
        }

        public async Task<bool> WithdrawMoneyAsync(int accountId, decimal amount)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID", nameof(accountId));
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be positive", nameof(amount));

            return await _accountRepository.WithdrawMoneyAsync(accountId, amount);
        }

        public async Task<IEnumerable<Account>> GetClientAccountsAsync(int clientId)
        {
            if (clientId <= 0)
                throw new ArgumentException("Invalid client ID", nameof(clientId));

            var client = await _clientRepository.GetClientByIdAsync(clientId);
            if (client == null)
                throw new ArgumentException("Client not found", nameof(clientId));

            return await _accountRepository.GetClientAccountsAsync(clientId);
        }
    }
} 