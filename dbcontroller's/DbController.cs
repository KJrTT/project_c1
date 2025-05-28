using System;
using System.Data;
using Npgsql;
using BankingSystem.Models;

namespace BankingSystem.DbControllers
{
    public class DbController : IDisposable
    {
        protected readonly string _connectionString;
        protected NpgsqlConnection _connection;

        public DbController(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new NpgsqlConnection(connectionString);
        }

        protected async Task OpenConnectionAsync()
        {
            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }
        }

        protected async Task CloseConnectionAsync()
        {
            if (_connection.State != ConnectionState.Closed)
            {
                await _connection.CloseAsync();
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
