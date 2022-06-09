using Dapper.Data.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Data.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly string _tableName;
        private readonly IConfiguration configuration;
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
        private string GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {_tableName} ");
            insertQuery.Append("(");

            var properties = GenerateListOfProperties();
            properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");

            properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");

            return insertQuery.ToString();
        }
        private string GenerateUpdateQuery()
        {
            var updateQuery = new StringBuilder($"UPDATE {_tableName} SET ");
            var properties = GenerateListOfProperties();

            properties.ForEach(property =>
            {
                updateQuery.Append($"{property}=@{property},");
            });

            updateQuery.Remove(updateQuery.Length - 1, 1); //remove last comma
            updateQuery.Append(" WHERE Id=@Id");

            return updateQuery.ToString();
        }
        private static List<string> GenerateListOfProperties()
        {
            var properties = (from prop in typeof(T)
                    .GetProperties()
                              let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                              where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                              && !prop.PropertyType.GetTypeInfo().IsGenericType //for generics
                              select prop.Name).ToList();

            //using Id or ClassName+Id as primary key
            string[] excludeList = new string[] { "Id", typeof(T).Name.ToUpper() + "ID" };
            return properties.Where(prop => !excludeList.Contains(prop)).ToList();
        }
        public GenericRepository(IConfiguration configuration, string tableName)
        {
            this.configuration = configuration;
            _tableName = tableName;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<T>($"SELECT * FROM {_tableName}");
            }
        }
        public async Task<T> GetAsync(object id)
        {
            using (var connection = CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<T>($"SELECT * FROM {_tableName} WHERE Id=@Id", new { Id = id });
                if (result == null)
                    throw new KeyNotFoundException($"{_tableName} with id [{id}] could not be found.");

                return result;
            }
        }
        public async Task AddAsync(T entity)
        {
            var insertQuery = GenerateInsertQuery();
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(insertQuery, entity);
            }
        }
        public async Task<int> AddRangeAsync(IEnumerable<T> list)
        {
            var inserted = 0;
            var query = GenerateInsertQuery();
            using (var connection = CreateConnection())
            {
                inserted += await connection.ExecuteAsync(query, list);
            }
            return inserted;
        }
        public async Task UpdateAsync(T entity)
        {
            var updateQuery = GenerateUpdateQuery();
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(updateQuery, entity);
            }
        }
        public async Task DeleteAsync(object id)
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync($"DELETE FROM {_tableName} WHERE Id=@Id", new { Id = id });
            }
        }

        //For query
        public async Task<IEnumerable<T>> QueryAsync(string sql, object parameters = null)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<T>(sql, parameters);
            }
        }
        public async Task<T> QueryFirstOrDefaultAsync(string sql, object parameters = null)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
            }
        }
        //For command
        public async Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(sql, parameters);
            }
        }
    }
}