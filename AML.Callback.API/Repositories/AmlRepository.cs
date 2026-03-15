using AML.Callback.API.Models;
using AML.Shared.Infrastructure;
using Dapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AML.Callback.API.Repositories
{
    public class AmlRepository : IAmlRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
        public AmlRepository(SqlConnectionFactory connectionFactory, IConfiguration configuration)
        {
            _connectionFactory = connectionFactory;
            _configuration = configuration;
        }
        public async Task<bool> UpdateCustomerScreening(AmlHitUpdateRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();

            // Create DataTable for TVP
            var table = new DataTable();
            table.Columns.Add("CustomerId", typeof(int));
            table.Columns.Add("AlertId", typeof(string));
            table.Columns.Add("ProscribedStatus", typeof(int));
            table.Columns.Add("EmpUsername", typeof(string));
            table.Columns.Add("FinalComments", typeof(string));
            table.Columns.Add("ModuleType", typeof(string));

            table.Rows.Add(
                request.CustomerId,
                request.AlertId,
                request.ProscribedStatus,
                request.EmpUsername,
                request.FinalComments,
                request.ModuleType
            );

            var parameters = new DynamicParameters();

            // Pass TVP
            parameters.Add(
                "@Customer",
                table.AsTableValuedParameter("AMLCustomerUpdateType")
            );

            // Output parameter
            parameters.Add(
                "@Result",
                dbType: System.Data.DbType.Int32,
                direction: System.Data.ParameterDirection.Output
            );

            await connection.ExecuteAsync(
                "sp_AML_UpdateCustomerScreening",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var result = parameters.Get<int>("@Result");

            return result == 1;
        }
    }
}
