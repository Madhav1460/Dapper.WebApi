using Dapper.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Data
{
    public interface IDatabaseContext
    {
        IProductRepository Products { get; }
    }
}
