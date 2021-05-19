using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopBridge.Data;
using ShopBridge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopBridge.Controllers
{
    public class ProductsController : ODataController
    {
        private DataContext db;
        public ProductsController(DataContext context)
        {
            db = context;
        }
        [EnableQuery(PageSize =50)]
        [ODataRoute("Products")]
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }
        [ODataRoute("Products")]
        public async Task<IActionResult> Post([FromBody] Product data)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Products.Add(data);
            
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(data.Id);
        }
        [ODataRoute("Products({id})")]
        public async Task<IActionResult> Patch(int Id, [FromBody] Delta<Product> delta)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await db.Products.FindAsync(Id);
            if (entity == null)
            {
                return NotFound();
            }

            delta.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Updated(delta);
        }
        [ODataRoute("Products({id})")]
        public async Task<IActionResult> Delete(int Id)
        {
            var product = await db.Products.FindAsync(Id);
            if (product == null)
            {
                return NotFound();
            }
            db.Products.Remove(product);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
    }
}
