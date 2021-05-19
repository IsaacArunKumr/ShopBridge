using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShopBridge.Controllers;
using ShopBridge.Data;
using ShopBridge.Models;
using System;
using System.Linq;

namespace ShopBridgeTest
{
    [TestClass]
    public class ProductTest
    {

        [TestMethod]
        public void TestGet()
        {
            using (var context = new DataContext())
            {
                ProductsController controller = new ProductsController(context);
                var products = controller.GetProducts();
                Assert.AreEqual(true, products.AnyAsync<Product>().Result);
            }
        }
        [TestMethod]
        public void TestPostPatch()
        {
            using (var context = new DataContext())
            {
                ProductsController controller = new ProductsController(context);
                var guid = Guid.NewGuid();
                var product = new Product {
                    Name = "TestProd" + guid,
                      Description= "TestProd" + guid,
                      Price= 40,
                      CategoryId= 1
                };
                OkObjectResult res=(OkObjectResult)controller.Post(product).Result;

                Assert.IsNotNull(res);
                Assert.AreEqual(200, res.StatusCode);
                int id = (int)res.Value;
                
                var delta = new Delta<Product>();
                delta.TrySetPropertyValue("Price", 50);
                IActionResult res1 = controller.Patch(id, delta).Result;
                Assert.IsNotNull(res1);
            }
        }
        [TestCleanup]
        public void cleanup()
        {
            using (var context = new DataContext())
            {
                var testprods=context.Products.Where(x=>x.Name.Contains("TestProd"));
                context.Products.RemoveRange(testprods);
                context.SaveChanges();
            }
        }
    }
}
