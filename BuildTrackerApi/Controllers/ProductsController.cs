using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildTrackerApi.Models;
using Microsoft.AspNetCore.Authorization;
using BuildTrackerApi.Models.Dtos;
using AutoMapper;
using BuildTrackerApi.Services.Authorization;

namespace BuildTrackerApi.Controllers
{

    [Authorize]
    [DenyNotConfirmed]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly BuildTrackerContext _context;
        private readonly IMapper _mapper;

        public ProductsController(BuildTrackerContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Products
        [HttpGet]
        [AllowNotConfirmed]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        [AllowNotConfirmed]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return _mapper.Map<ProductDto>(product);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN, PROJECT_MANAGER")]
        public async Task<IActionResult> PutProduct(int id, ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        [HttpPost]
        [Authorize(Roles = "ADMIN, PROJECT_MANAGER")]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            productDto = _mapper.Map<ProductDto>(product);
            return CreatedAtAction("GetProduct", new { id = product.Id }, productDto);
        }

        // DELETE: api/Products/5
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductDto>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
