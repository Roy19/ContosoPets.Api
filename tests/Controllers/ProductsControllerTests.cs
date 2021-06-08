using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using ContosoPets.Api.Controllers;
using ContosoPets.Api.Models;
using ContosoPets.Api.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPets.Api.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private Mock<IContosoPetsContext> mockContosoPetsContext;
        private readonly Mock<ILogger<ProductsController>> mockLogger;
        private readonly List<Product> mockProducts;
        
        public ProductsControllerTests()
        {
            mockContosoPetsContext = new Mock<IContosoPetsContext>();
            mockLogger = new Mock<ILogger<ProductsController>>();
            mockProducts = new List<Product>(
                new Product[] 
                {
                    new Product {
                        Id = 1,
                        Name = "mock product",
                        Price = 1
                    },
                }
            );
        }

        [Fact]
        public void TestGetAll()
        {
            var mockDbSet = GetQueryableMockDbSet<Product>(mockProducts);
            mockContosoPetsContext.Setup(cp => cp.Products).Returns(mockDbSet);
            var productsController = new ProductsController(mockContosoPetsContext.Object, mockLogger.Object);

            var result = productsController.GetAll();

            Assert.IsType<ActionResult<List<Product>>>(result);
            Assert.Equal(mockProducts, result.Value);
        }

        private DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();
            var mockDbSet = new Mock<DbSet<T>>();
            
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            mockDbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));

            return mockDbSet.Object;
        }
    }
}