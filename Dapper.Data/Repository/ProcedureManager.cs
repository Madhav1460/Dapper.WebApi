using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Data.Interfaces;
using Dapper.Core.Enum;
using Microsoft.Extensions.Configuration;

namespace Dapper.Data.Repository
{
    public class ProcedureManager : IProcedureManager
    {
        private readonly IConfiguration configuration;
        public ProcedureManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        private SqlConnection SqlConnection()
        {
            string ConnectionString = configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(ConnectionString);
        }
        private IDbConnection CreateConnection()
        {
            var conn = SqlConnection();
            conn.Open();
            return conn;
        }
        public async Task<dynamic> QueryMultipleAsync(string storedProcedure, object parameters = null, IEnumerable<MapItem> mapItems = null)
        {
            //var data = new ExpandoObject();
            dynamic data = new ExpandoObject();
            if (mapItems == null) return data;

            using (var connection = CreateConnection())
            {
                var multi = await connection.QueryMultipleAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

                foreach (var item in mapItems)
                {
                    if (item.DataRetriveType == EnumDataRetriveType.FirstOrDefault)
                    {
                        var singleItem = multi.Read(item.Type).FirstOrDefault();
                        ((IDictionary<string, object>)data).Add(item.PropertyName, singleItem);
                    }
                    else if (item.DataRetriveType == EnumDataRetriveType.List)
                    {
                        var listItem = multi.Read(item.Type).ToList();
                        ((IDictionary<string, object>)data).Add(item.PropertyName, listItem);
                    }
                }
                return data;
            }
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string storedProcedure, object parameters = null)
        {
            using (var connection = CreateConnection())
            {
                if (parameters != null)
                {
                    return await connection.QueryAsync<T>(storedProcedure, parameters,
                          commandType: CommandType.StoredProcedure);
                }
                else
                {
                    return await connection.QueryAsync<T>(storedProcedure,
                        commandType: CommandType.StoredProcedure);
                }
            }
        }
        public async Task<T> QueryFirstOrDefaultAsync<T>(string storedProcedure, object parameters = null)
        {
            using (var connection = CreateConnection())
            {
                if (parameters != null)
                {
                    return await connection.QueryFirstOrDefaultAsync<T>(storedProcedure, parameters,
                          commandType: CommandType.StoredProcedure);
                }
                else
                {
                    return await connection.QueryFirstOrDefaultAsync<T>(storedProcedure,
                        commandType: CommandType.StoredProcedure);
                }
            }
        }
        public async Task<int> ExecuteCommandAsync(string storedProcedure, object parameters)
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(storedProcedure, parameters,
                      commandType: CommandType.StoredProcedure);
            }
        }
    }
}