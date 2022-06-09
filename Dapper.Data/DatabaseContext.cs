using Dapper.Data.Interfaces;
using Dapper.Data.Repository;
using Dapper.Core.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Data
{
    public class DatabaseContext : IDatabaseContext
    {
        private readonly IConfiguration configuration;
        public DatabaseContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private IProductRepository products;
        public IProductRepository Products
        {
            get
            {
                if (products == null)
                    products = new ProductRepository(configuration, "Products");
                return products;
            }
        }
    }
}
