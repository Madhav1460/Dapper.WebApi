using Dapper.Data.Interfaces;
using Dapper.Core.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Data.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly IConfiguration configuration;
        public ProductRepository(IConfiguration configuration, string tableName): base(configuration, tableName)
        {
            this.configuration = configuration;
        }
    }
}