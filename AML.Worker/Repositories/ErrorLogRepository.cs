using AML.Shared.Infrastructure;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AML.Worker.Repositories
{
    public class ErrorLogRepository
    {
        private readonly IDbConnection _db;
       private readonly SqlConnectionFactory _connectionFactory;

        public ErrorLogRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task LogErrorAsync(string process, string message, string stackTrace)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new
            {
                ProcessName = process,
                ErrorMessage = message,
                StackTrace = stackTrace
            };

            await connection.ExecuteAsync(
                "sp_InsertAMLWorkerErrorLog",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
