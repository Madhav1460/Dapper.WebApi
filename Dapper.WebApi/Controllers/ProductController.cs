
using System.Threading.Tasks;
using Dapper.Data;
using Dapper.Data.Interfaces;
using Dapper.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Transactions;

namespace Dapper.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IDatabaseContext db;
        public ProductController(IDatabaseContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //var data = await unitOfWork.Products.GetAllAsync();

            var data = await db.Products.QueryAsync("Select * from Products");

            return Ok(data);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            //var data = await unitOfWork.Products.GetAsync(id);

            DynamicParameters param = new DynamicParameters();
            param.Add("@ProductId", id);
            var data = await db.Products.QueryFirstOrDefaultAsync("Select * from Products where Id=@ProductId", param);

            if (data == null)
                return Ok();
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> Add(Product product)
        {
            using (var transactionScope = new TransactionScope())
            {
                try
                {
                    // await db.Products.AddAsync(product);
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Name", product.Name);
                    param.Add("@Barcode", product.Barcode);
                    param.Add("@Description", product.Description);
                    param.Add("@Rate", product.Rate);
                    param.Add("@AddedOn", product.AddedOn);

                    var result = await db.Products.ExecuteAsync("Insert into Products(Name,Barcode,Description,Rate,AddedOn) Values(@Name,@Barcode,@Description,@Rate,@AddedOn)", param);

                    //commit db changes
                    transactionScope.Complete();

                    if (result == 0)
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    return Ok();
                }
                catch (Exception exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await db.Products.DeleteAsync(id);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Update(Product product)
        {
            await db.Products.UpdateAsync(product);
            return Ok();
        }
    }
}