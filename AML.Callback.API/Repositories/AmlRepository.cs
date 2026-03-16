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
        public async Task<List<CustomerResponse>> UpdateCustomerScreening(List<AmlUpdateRequest> requests)
        {
            using var connection = _connectionFactory.CreateConnection();

            var table = new DataTable();
            table.Columns.Add("CustomerId", typeof(int));
            table.Columns.Add("AlertId", typeof(string));
            table.Columns.Add("ProscribedStatus", typeof(int));
            table.Columns.Add("EmpUsername", typeof(string));
            table.Columns.Add("FinalComments", typeof(string));
            table.Columns.Add("ModuleType", typeof(string));

            foreach (var request in requests)
            {
                table.Rows.Add(
                    request.CustomerId,
                    request.AlertId,
                    request.ProscribedStatus,
                    request.EmpUsername,
                    request.FinalComments,
                    request.ModuleType
                );
            }

            var parameters = new DynamicParameters();

            parameters.Add(
                "@Customer",
                table.AsTableValuedParameter("AMLCustomerUpdateType")
            );

            var result = await connection.QueryAsync<CustomerResponse>(
                "SPME_AML_UPDATE_WS2_RESULT",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
    }
}
