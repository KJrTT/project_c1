using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using BankingSystem.Models;

namespace BankingSystem
{
    public class ClientService : IClientService
    {
        private readonly string _connectionString;

        public ClientService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> RegisterClientAsync(string firstName, string lastName, int age, string address, string passportSeries, string passportNumber, int bankId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(
                    @"INSERT INTO client (first_nameclient, lastnameclient, ageclient, address, passportseries, passportnumber, bank_id) 
                      VALUES (@firstName, @lastName, @age, @address, @passportSeries, @passportNumber, @bankId) 
                      RETURNING client_id", conn))
                {
                    cmd.Parameters.AddWithValue("firstName", firstName);
                    cmd.Parameters.AddWithValue("lastName", lastName);
                    cmd.Parameters.AddWithValue("age", age);
                    cmd.Parameters.AddWithValue("address", address);
                    cmd.Parameters.AddWithValue("passportSeries", passportSeries);
                    cmd.Parameters.AddWithValue("passportNumber", passportNumber);
                    cmd.Parameters.AddWithValue("bankId", bankId);

                    try
                    {
                        return (int)await cmd.ExecuteScalarAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при регистрации клиента: {ex.Message}");
                        return -1; 
                    }
                }
            }
        }

        public async Task<Client> GetClientByIdAsync(int clientId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM client WHERE client_id = @clientId", conn))
                {
                    cmd.Parameters.AddWithValue("clientId", clientId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Client
                            {
                                ClientId = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Age = reader.GetInt32(3),
                                Address = reader.GetString(4),
                                PassportSeries = reader.GetString(5),
                                PassportNumber = reader.GetString(6),
                                BankId = reader.GetInt32(7),
                                CreatedAt = reader.GetDateTime(8),
                                UpdatedAt = reader.GetDateTime(9)
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            var clients = new List<Client>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM client ORDER BY client_id", conn))
                {                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
            
                        while (await reader.ReadAsync())
                        {
                            clients.Add(new Client
                            {
                                ClientId = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Age = reader.GetInt32(3),
                                Address = reader.GetString(4),
                                PassportSeries = reader.GetString(5),
                                PassportNumber = reader.GetString(6),
                                BankId = reader.GetInt32(7),
                                CreatedAt = reader.GetDateTime(8),
                                UpdatedAt = reader.GetDateTime(9)
                            });
                        }
                    }
                }
            }
            return clients;
        }
    }
} 