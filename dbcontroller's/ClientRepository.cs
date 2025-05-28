using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Npgsql;
using BankingSystem.Models;

namespace BankingSystem.DbControllers
{
    public class ClientRepository : DbController
    {
        public ClientRepository(string connectionString) : base(connectionString) { }

        public async Task<int> RegisterClientAsync(string firstName, string lastName, int age, string address, string passportSeries, string passportNumber, int bankId)
        {
            try
            {
                await OpenConnectionAsync();
                using var cmd = new NpgsqlCommand(
                    @"INSERT INTO clients (first_name, last_name, age, address, passport_series, passport_number, bank_id) 
                      VALUES (@firstName, @lastName, @age, @address, @passportSeries, @passportNumber, @bankId)
                      RETURNING client_id",
                    _connection);
                
                cmd.Parameters.AddWithValue("firstName", firstName);
                cmd.Parameters.AddWithValue("lastName", lastName);
                cmd.Parameters.AddWithValue("age", age);
                cmd.Parameters.AddWithValue("address", address);
                cmd.Parameters.AddWithValue("passportSeries", passportSeries);
                cmd.Parameters.AddWithValue("passportNumber", passportNumber);
                cmd.Parameters.AddWithValue("bankId", bankId);
                
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<Client> GetClientByIdAsync(int clientId)
        {
            try
            {
                await OpenConnectionAsync();
                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM clients WHERE client_id = @clientId",
                    _connection);
                
                cmd.Parameters.AddWithValue("clientId", clientId);
                
                using var reader = await cmd.ExecuteReaderAsync();
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
                        BankId = reader.GetInt32(7)
                    };
                }
                return null;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            var clients = new List<Client>();
            try
            {
                await OpenConnectionAsync();
                using var cmd = new NpgsqlCommand("SELECT * FROM clients", _connection);
                using var reader = await cmd.ExecuteReaderAsync();
                
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
                        BankId = reader.GetInt32(7)
                    });
                }
                return clients;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }
    }
} 