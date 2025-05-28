using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Npgsql;
using BankingSystem.Models;

namespace BankingSystem.DbControllers
{
    public class BankRepository : DbController
    {
        public BankRepository(string connectionString) : base(connectionString) { }

        public async Task<bool> AddBankAsync(string name, decimal depositRate, decimal creditFee)
        {
            try
            {
                await OpenConnectionAsync();
                using var cmd = new NpgsqlCommand(
                    "INSERT INTO banks (name, deposit_rate, credit_fee) VALUES (@name, @depositRate, @creditFee)",
                    _connection);
                
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("depositRate", depositRate);
                cmd.Parameters.AddWithValue("creditFee", creditFee);
                
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<bool> DeleteBankAsync(int bankId)
        {
            try
            {
                await OpenConnectionAsync();
                using var cmd = new NpgsqlCommand(
                    "DELETE FROM banks WHERE bank_id = @bankId",
                    _connection);
                
                cmd.Parameters.AddWithValue("bankId", bankId);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<Bank> GetBankByIdAsync(int bankId)
        {
            try
            {
                await OpenConnectionAsync();
                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM banks WHERE bank_id = @bankId",
                    _connection);
                
                cmd.Parameters.AddWithValue("bankId", bankId);
                
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Bank
                    {
                        BankId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        DepositRate = reader.GetDecimal(2),
                        CreditFee = reader.GetDecimal(3)
                    };
                }
                return null;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<IEnumerable<Bank>> GetAllBanksAsync()
        {
            var banks = new List<Bank>();
            try
            {
                await OpenConnectionAsync();
                using var cmd = new NpgsqlCommand("SELECT * FROM banks", _connection);
                using var reader = await cmd.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    banks.Add(new Bank
                    {
                        BankId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        DepositRate = reader.GetDecimal(2),
                        CreditFee = reader.GetDecimal(3)
                    });
                }
                return banks;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }
    }
}
