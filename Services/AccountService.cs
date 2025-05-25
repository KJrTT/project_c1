using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using BankingSystem.Models;

namespace BankingSystem
{
    public class AccountService : IAccountService
    {
        private readonly string _connectionString;

        public AccountService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> CreateAccountAsync(int clientId, string accountType, decimal initialAmount)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                try
                {
                    using (var cmd = new NpgsqlCommand(
                        @"INSERT INTO account (clientid, balance, interestrate, account_type, status) 
                          VALUES (@clientId, @balance, @interestRate, @type, 'active') 
                          RETURNING accountid", conn))
                    {
                        cmd.Parameters.AddWithValue("clientId", clientId);
                        cmd.Parameters.AddWithValue("balance", initialAmount);
                        cmd.Parameters.AddWithValue("interestRate", 0.0m);
                        cmd.Parameters.AddWithValue("type", accountType);

                        int accountId = (int)await cmd.ExecuteScalarAsync();

                        string specificQuery = accountType switch
                        {
                            "debit" => @"INSERT INTO debitaccount (account_id, statdata, initialamount, monthly_fee) 
                                        VALUES (@accountId, CURRENT_TIMESTAMP, @initialAmount, 0)",
                            "credit" => @"INSERT INTO creditaccount (account_id, creditlimit, overdraftfee, min_payment, payment_due_day) 
                                         VALUES (@accountId, @creditLimit, 0, 0, 1)",
                            "deposit" => @"INSERT INTO depositaccount (account_id, statdata, enddata, initialamount, capitalization, early_withdrawal_penalty) 
                                         VALUES (@accountId, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '12 months', @initialAmount, false, 0)",
                            _ => throw new Exception("Неверный тип счета")
                        };

                        using (var specificCmd = new NpgsqlCommand(specificQuery, conn))
                        {
                            specificCmd.Parameters.AddWithValue("accountId", accountId);
                            specificCmd.Parameters.AddWithValue("initialAmount", initialAmount);
                            if (accountType == "credit")
                                specificCmd.Parameters.AddWithValue("creditLimit", initialAmount * 2);

                            await specificCmd.ExecuteNonQueryAsync();
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при создании счета: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> BlockAccountAsync(int accountId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("UPDATE account SET status = 'frozen' WHERE accountid = @accountId", conn))
                {
                    cmd.Parameters.AddWithValue("accountId", accountId);
                    return await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
        }

        public async Task<bool> UnblockAccountAsync(int accountId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("UPDATE account SET status = 'active' WHERE accountid = @accountId", conn))
                {
                    cmd.Parameters.AddWithValue("accountId", accountId);
                    return await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
        }

        public async Task<bool> DepositMoneyAsync(int accountId, decimal amount)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                try
                {
                    using (var cmd = new NpgsqlCommand(
                        @"UPDATE account SET balance = balance + @amount WHERE accountid = @accountId;\n  INSERT INTO transaction (fromaccountid, toaccountid, amount, status, description, reference_number) \n  VALUES (NULL, @accountId, @amount, 'completed', 'Пополнение счета', @reference)", conn))
                    {
                        cmd.Parameters.AddWithValue("amount", amount);
                        cmd.Parameters.AddWithValue("accountId", accountId);
                        cmd.Parameters.AddWithValue("reference", $"DEP{DateTime.Now:yyyyMMddHHmmss}");

                        await cmd.ExecuteNonQueryAsync();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при пополнении счета: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> WithdrawMoneyAsync(int accountId, decimal amount)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                try
                {
                    using (var checkCmd = new NpgsqlCommand(
                        "SELECT balance, status FROM account WHERE accountid = @accountId", conn))
                    {
                        checkCmd.Parameters.AddWithValue("accountId", accountId);

                        using (var reader = await checkCmd.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                            {
                                Console.WriteLine("Счет не найден.");
                                return false;
                            }

                            decimal balance = reader.GetDecimal(0);
                            string status = reader.GetString(1);

                            if (status != "active")
                            {
                                Console.WriteLine("Счет не активен.");
                                return false;
                            }

                            if (balance < amount)
                            {
                                Console.WriteLine("Недостаточно средств.");
                                return false;
                            }
                        }
                    }

                    using (var cmd = new NpgsqlCommand(
                        @"UPDATE account SET balance = balance - @amount WHERE accountid = @accountId;\n  INSERT INTO transaction (fromaccountid, toaccountid, amount, status, description, reference_number) \n  VALUES (@accountId, NULL, @amount, 'completed', 'Снятие средств', @reference)", conn))
                    {
                        cmd.Parameters.AddWithValue("amount", amount);
                        cmd.Parameters.AddWithValue("accountId", accountId);
                        cmd.Parameters.AddWithValue("reference", $"WD{DateTime.Now:yyyyMMddHHmmss}");

                        await cmd.ExecuteNonQueryAsync();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при снятии средств: {ex.Message}");
                    return false;
                }
            }
        }

        

        public async Task<Account> GetAccountByIdAsync(int accountId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(
                    "SELECT * FROM account WHERE accountid = @accountId", conn))
                {
                    cmd.Parameters.AddWithValue("accountId", accountId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Account
                            {
                                AccountId = reader.GetInt32(0),
                                ClientId = reader.GetInt32(1),
                                Balance = reader.GetDecimal(2),
                                InterestRate = reader.GetDecimal(3),
                                AccountType = reader.GetString(4),
                                Status = reader.GetString(5),
                                CreatedAt = reader.GetDateTime(6),
                                UpdatedAt = reader.GetDateTime(7)
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public async Task<IEnumerable<Account>> GetClientAccountsAsync(int clientId)
        {
            var accounts = new List<Account>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM account WHERE clientid = @clientId", conn))
                {
                    cmd.Parameters.AddWithValue("clientId", clientId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            accounts.Add(new Account
                            {
                                AccountId = reader.GetInt32(0),
                                ClientId = reader.GetInt32(1),
                                Balance = reader.GetDecimal(2),
                                InterestRate = reader.GetDecimal(3),
                                AccountType = reader.GetString(4),
                                Status = reader.GetString(5),
                                CreatedAt = reader.GetDateTime(6),
                                UpdatedAt = reader.GetDateTime(7)
                            });
                        }
                    }
                }
            }
            return accounts;
        }
    }
} 