using Dapper.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Data.Interfaces
{
    public interface IProcedureManager
    {
        Task<IEnumerable<T>> QueryAsync<T>(string storedProcedure, object parameters = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string storedProcedure, object parameters = null);
        Task<dynamic> QueryMultipleAsync(string storedProcedure, object parameters = null, IEnumerable<MapItem> mapItems = null);
        Task<int> ExecuteCommandAsync(string storedProcedure, object parameters);
    }
}
