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
    public class ProductDevelopersController : ControllerBase
    {
        private readonly BuildTrackerContext _context;
        private IMapper _mapper;

        public ProductDevelopersController(BuildTrackerContext context, IMapper mapper)
        {
            _context = context;
            _mapper =  mapper;
        }

        // GET: api/ProductDevelopers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDeveloperDto>>> GetProductDevelopers()
        {
            var pdos =  await _context.ProductDevelopers.Include(c=> c.Developer).Include(d=> d.Product).ToListAsync();
            return _mapper.Map<List<ProductDeveloperDto>>(pdos);
        }

        // GET: api/ProductDevelopers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDeveloperDto>> GetProductDeveloper(int id)
        {
            var productDeveloper = await _context.ProductDevelopers.FindAsync(id);

            if (productDeveloper == null)
            {
                return NotFound();
            }

            return _mapper.Map<ProductDeveloperDto>(productDeveloper);
        }

        // PUT: api/ProductDevelopers/5
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN, PROJECT_MANAGER")]
        public async Task<IActionResult> PutProductDeveloper(int id, ProductDeveloperDto productDeveloperDto)
        {
            var productDeveloper = _mapper.Map<ProductDeveloper>(productDeveloperDto);

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
        public async Task<ActionResult<ProductDeveloper>> PostProductDeveloper(ProductDeveloperDto productDeveloperDto)
        {

            var productDeveloper = _mapper.Map<ProductDeveloper>(productDeveloperDto);

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

            productDeveloperDto = _mapper.Map<ProductDeveloperDto>(productDeveloper);

            return CreatedAtAction("GetProductDeveloper", new { id = productDeveloper.ProductId }, productDeveloperDto);
        }

        // DELETE: api/ProductDevelopers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN, PROJECT_MANAGER")]
        public async Task<ActionResult<ProductDeveloperDto>> DeleteProductDeveloper(int id)
        {
            var productDeveloper = await _context.ProductDevelopers.FindAsync(id);
            if (productDeveloper == null)
            {
                return NotFound();
            }

            _context.ProductDevelopers.Remove(productDeveloper);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDeveloperDto>(productDeveloper);
        }

        private bool ProductDeveloperExists(int id)
        {
            return _context.ProductDevelopers.Any(e => e.ProductId == id);
        }
    }
}
