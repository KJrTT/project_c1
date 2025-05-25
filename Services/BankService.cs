using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using BankingSystem.Models;

namespace BankingSystem
{
    public class BankService : IBankService
    {
        private readonly string _connectionString;

        public BankService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> AddBankAsync(string name, decimal depositRate, decimal creditFee)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(
                    @"INSERT INTO bank (name_bank, depositrate, creditfee) 
                      VALUES (@name, @depositRate, @creditFee)", conn))
                {
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("depositRate", depositRate);
                    cmd.Parameters.AddWithValue("creditFee", creditFee);

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при добавлении банка: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public async Task<bool> DeleteBankAsync(int bankId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var checkClientsCmd = new NpgsqlCommand("SELECT COUNT(*) FROM client WHERE bank_id = @bankId", conn))
                {
                    checkClientsCmd.Parameters.AddWithValue("bankId", bankId);
                    if ((long)await checkClientsCmd.ExecuteScalarAsync() > 0)
                    {
                        Console.WriteLine("Невозможно удалить банк: есть связанные клиенты.");
                        return false;
                    }
                }
                using (var checkManagersCmd = new NpgsqlCommand("SELECT COUNT(*) FROM manager WHERE bank_id = @bankId", conn))
                {
                    checkManagersCmd.Parameters.AddWithValue("bankId", bankId);
                    if ((long)await checkManagersCmd.ExecuteScalarAsync() > 0)
                    {
                        Console.WriteLine("Невозможно удалить банк: есть связанные менеджеры.");
                        return false;
                    }
                }

                using (var deleteCmd = new NpgsqlCommand("DELETE FROM bank WHERE bank_id = @bankId", conn))
                {
                    deleteCmd.Parameters.AddWithValue("bankId", bankId);
                    try
                    {
                         return await deleteCmd.ExecuteNonQueryAsync() > 0;
                    }
                    catch (Exception ex)
                    {
                         Console.WriteLine($"Ошибка при удалении банка: {ex.Message}");
                         return false;
                    }
                }
            }
        }

        public async Task<Bank> GetBankByIdAsync(int bankId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM bank WHERE bank_id = @bankId", conn))
                {
                    cmd.Parameters.AddWithValue("bankId", bankId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Bank
                            {
                                BankId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                DepositRate = reader.GetDecimal(2),
                                CreditFee = reader.GetDecimal(3),
                                CreatedAt = reader.GetDateTime(4),
                                UpdatedAt = reader.GetDateTime(5)
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public async Task<IEnumerable<Bank>> GetAllBanksAsync()
        {
            var banks = new List<Bank>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM bank ORDER BY bank_id", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            banks.Add(new Bank
                            {
                                BankId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                DepositRate = reader.GetDecimal(2),
                                CreditFee = reader.GetDecimal(3),
                                CreatedAt = reader.GetDateTime(4),
                                UpdatedAt = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            return banks;
        }
    }
} 