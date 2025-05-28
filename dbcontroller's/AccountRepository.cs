using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Npgsql;
using BankingSystem.Models;

namespace BankingSystem.DbControllers
{
    public class AccountRepository : DbController
    {
        public AccountRepository(string connectionString) : base(connectionString) { }

        public async Task<bool> CreateAccountAsync(int clientId, string accountType, decimal initialAmount)
        {
            try
            {
                await OpenConnectionAsync();
                using var cmd = new NpgsqlCommand(
                    "INSERT INTO accounts (client_id, account_type, balance, status) VALUES (@clientId, @accountType, @initialAmount, 'active')",
                    _connection);
                
                cmd.Parameters.AddWithValue("clientId", clientId);
                cmd.Parameters.AddWithValue("accountType", accountType);
                cmd.Parameters.AddWithValue("initialAmount", initialAmount);
                
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<bool> BlockAccountAsync(int accountId)
        {
            try
            {
                await OpenConnectionAsync();
                using var cmd = new NpgsqlCommand(
                    "UPDATE accounts SET status = 'blocked' WHERE account_id = @accountId",
                    _connection);
                
                cmd.Parameters.AddWithValue("accountId", accountId);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<bool> UnblockAccountAsync(int accountId)
        {
            try
            {
                await OpenConnectionAsync();
                using var cmd = new NpgsqlCommand(
                    "UPDATE accounts SET status = 'active' WHERE account_id = @accountId",
                    _connection);
                
                cmd.Parameters.AddWithValue("accountId", accountId);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<bool> DepositMoneyAsync(int accountId, decimal amount)
        {
            try
            {
                await OpenConnectionAsync();
                using var cmd = new NpgsqlCommand(
                    "UPDATE accounts SET balance = balance + @amount WHERE account_id = @accountId AND status = 'active'",
                    _connection);
                
                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("amount", amount);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<bool> WithdrawMoneyAsync(int accountId, decimal amount)
        {
            try
            {
                await OpenConnectionAsync();
                using var cmd = new NpgsqlCommand(
                    "UPDATE accounts SET balance = balance - @amount WHERE account_id = @accountId AND status = 'active' AND balance >= @amount",
                    _connection);
                
                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("amount", amount);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<IEnumerable<Account>> GetClientAccountsAsync(int clientId)
        {
            var accounts = new List<Account>();
            try
            {
                await OpenConnectionAsync();
                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM accounts WHERE client_id = @clientId",
                    _connection);
                
                cmd.Parameters.AddWithValue("clientId", clientId);
                using var reader = await cmd.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    accounts.Add(new Account
                    {
                        AccountId = reader.GetInt32(0),
                        ClientId = reader.GetInt32(1),
                        AccountType = reader.GetString(2),
                        Balance = reader.GetDecimal(3),
                        Status = reader.GetString(4)
                    });
                }
                return accounts;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }
    }
} 