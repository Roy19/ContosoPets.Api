using ContosoPets.Api.Data;
using ContosoPets.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ContosoPets.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IContosoPetsContext _context;
        private readonly ILogger<ProductsController> _logger;
        
        public ProductsController(IContosoPetsContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<Product>> GetAll() => _context.Products.ToList();
            
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(long id)
        {
            var product = await _context.Products.FindAsync(id);

            if(product == null)
            {
                return NotFound();
            }
            else
            {
                return product;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(long id)
        {
            var product = await _context.Products.FindAsync(id);

            if(product == null)
            {
                return NotFound();
            }
            else
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, Product product)
        {
            if(id != product.Id)
            {
                return BadRequest();
            }
            else
            {
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
    }
}