using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AML.Shared.Infrastructure;
using AML.Shared.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace AML.Worker.Repositories
{
    public class CustomerRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;
        public CustomerRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<ScreeningRequest>> GetCustomersAsync()
        {
            

            using var connection = _connectionFactory.CreateConnection();

            var customers = await connection.QueryAsync<ScreeningRequest>(
                "SPME_GET_AML_PENDING_CUSTOMERS_SCREENING_RDS1",
                commandType: System.Data.CommandType.StoredProcedure);

            return customers.ToList();
        }

        public async Task SaveAMLResultAsync(string customerId,string description,string message)
        {
            using var connection = _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(
                "sp_SaveAMLResult",
                new
                {
                    CustomerId = customerId,
                    Description = description,
                    StatusMessage = message
                },
            commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
