using Dapper.Data.Interfaces;
using Dapper.Core.Entities;
using Dapper.Core.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProcedureManager procedureManager;
        public ProductsController(IProcedureManager procedureManager)
        {
            this.procedureManager = procedureManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
             var data = await procedureManager.QueryAsync<Product>("usp_getProducts");
            return Ok(data);
        }

        [HttpGet("{id}/{category}")]
        public async Task<IActionResult> GetById(int id, int category)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ProductId", id);
                var mapItems = new List<MapItem>()
                {
                    new MapItem(typeof(Product), EnumDataRetriveType.FirstOrDefault, "Product"),
                    new MapItem(typeof(Category), EnumDataRetriveType.List, "Category")
                };
                //multiple result set
                var result = await procedureManager.QueryMultipleAsync("usp_getProductCategories", param, mapItems);

                Product product = result.Product;
                IEnumerable<Category> categories = ((IList<dynamic>)result.Category).Cast<Category>();

                var obj = new { product = product, categories = categories };
               // var obj = new { categories = categories };
                return Ok(obj);
            }
            catch (System.Exception ex)
            {

                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@ProductId", id);
            var data = await procedureManager.QueryFirstOrDefaultAsync<Product>("usp_getProduct", param);

            if (data == null)
                return Ok();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@Name", product.Name);
            param.Add("@Barcode", product.Barcode);
            param.Add("@Description", product.Description);
            param.Add("@Rate", product.Rate);
            param.Add("@AddedOn", product.AddedOn);

            int result = await procedureManager.ExecuteCommandAsync("usp_addProduct", param);
            if (result == 0)
                return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok();
        }
    }
}
