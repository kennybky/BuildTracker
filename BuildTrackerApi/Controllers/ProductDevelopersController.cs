using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildTrackerApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace BuildTrackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDevelopersController : ControllerBase
    {
        private readonly BuildTrackerContext _context;

        public ProductDevelopersController(BuildTrackerContext context)
        {
            _context = context;
        }

        // GET: api/ProductDevelopers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDeveloper>>> GetProductDevelopers()
        {
            return await _context.ProductDevelopers.Include(c=> c.Developer).Include(d=> d.Product).ToListAsync();
        }

        // GET: api/ProductDevelopers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDeveloper>> GetProductDeveloper(int id)
        {
            var productDeveloper = await _context.ProductDevelopers.FindAsync(id);

            if (productDeveloper == null)
            {
                return NotFound();
            }

            return productDeveloper;
        }

        // PUT: api/ProductDevelopers/5
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN, PROJECT_MANAGER")]
        public async Task<IActionResult> PutProductDeveloper(int id, ProductDeveloper productDeveloper)
        {
            if (id != productDeveloper.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(productDeveloper).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductDeveloperExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProductDevelopers
        [HttpPost]
        [Authorize(Roles = "ADMIN, PROJECT_MANAGER")]
        public async Task<ActionResult<ProductDeveloper>> PostProductDeveloper(ProductDeveloper productDeveloper)
        {
            _context.ProductDevelopers.Add(productDeveloper);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductDeveloperExists(productDeveloper.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProductDeveloper", new { id = productDeveloper.ProductId }, productDeveloper);
        }

        // DELETE: api/ProductDevelopers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN, PROJECT_MANAGER")]
        public async Task<ActionResult<ProductDeveloper>> DeleteProductDeveloper(int id)
        {
            var productDeveloper = await _context.ProductDevelopers.FindAsync(id);
            if (productDeveloper == null)
            {
                return NotFound();
            }

            _context.ProductDevelopers.Remove(productDeveloper);
            await _context.SaveChangesAsync();

            return productDeveloper;
        }

        private bool ProductDeveloperExists(int id)
        {
            return _context.ProductDevelopers.Any(e => e.ProductId == id);
        }
    }
}
