using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BankingSystem.Models;
using BankingSystem.DbControllers;

namespace BankingSystem.Services
{
    public class ClientService : IClientService
    {
        private readonly ClientRepository _clientRepository;
        private readonly BankRepository _bankRepository;

        public ClientService(string connectionString)
        {
            _clientRepository = new ClientRepository(connectionString);
            _bankRepository = new BankRepository(connectionString);
        }

        public async Task<int> RegisterClientAsync(string firstName, string lastName, int age, string address, string passportSeries, string passportNumber, int bankId)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty", nameof(lastName));
            if (age < 18)
                throw new ArgumentException("Client must be at least 18 years old", nameof(age));
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty", nameof(address));
            if (string.IsNullOrWhiteSpace(passportSeries))
                throw new ArgumentException("Passport series cannot be empty", nameof(passportSeries));
            if (string.IsNullOrWhiteSpace(passportNumber))
                throw new ArgumentException("Passport number cannot be empty", nameof(passportNumber));
            if (bankId <= 0)
                throw new ArgumentException("Invalid bank ID", nameof(bankId));

            var bank = await _bankRepository.GetBankByIdAsync(bankId);
            if (bank == null)
                throw new ArgumentException("Bank not found", nameof(bankId));

            return await _clientRepository.RegisterClientAsync(firstName, lastName, age, address, passportSeries, passportNumber, bankId);
        }

        public async Task<Client> GetClientByIdAsync(int clientId)
        {
            if (clientId <= 0)
                throw new ArgumentException("Invalid client ID", nameof(clientId));

            return await _clientRepository.GetClientByIdAsync(clientId);
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _clientRepository.GetAllClientsAsync();
        }
    }
} 