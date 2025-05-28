using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BankingSystem.Models;
using BankingSystem.DbControllers;

namespace BankingSystem.Services
{
    public class BankService : IBankService
    {
        private readonly BankRepository _bankRepository;

        public BankService(string connectionString)
        {
            _bankRepository = new BankRepository(connectionString);
        }

        public async Task<bool> AddBankAsync(string name, decimal depositRate, decimal creditFee)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Bank name cannot be empty", nameof(name));
            if (depositRate < 0)
                throw new ArgumentException("Deposit rate cannot be negative", nameof(depositRate));
            if (creditFee < 0)
                throw new ArgumentException("Credit fee cannot be negative", nameof(creditFee));

            return await _bankRepository.AddBankAsync(name, depositRate, creditFee);
        }

        public async Task<bool> DeleteBankAsync(int bankId)
        {
            if (bankId <= 0)
                throw new ArgumentException("Invalid bank ID", nameof(bankId));

            return await _bankRepository.DeleteBankAsync(bankId);
        }

        public async Task<Bank> GetBankByIdAsync(int bankId)
        {
            if (bankId <= 0)
                throw new ArgumentException("Invalid bank ID", nameof(bankId));

            return await _bankRepository.GetBankByIdAsync(bankId);
        }

        public async Task<IEnumerable<Bank>> GetAllBanksAsync()
        {
            return await _bankRepository.GetAllBanksAsync();
        }
    }
} 