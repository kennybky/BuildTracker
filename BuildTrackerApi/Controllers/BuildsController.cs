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
    public class BuildsController : ControllerBase
    {
        private readonly BuildTrackerContext _context;

        public BuildsController(BuildTrackerContext context)
        {
            _context = context;
        }

        // GET: api/Builds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Build>>> GetBuilds()
        {
            return await _context.Builds.ToListAsync();
        }

        // GET: api/Builds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Build>> GetBuild(int id)
        {
            var build = await _context.Builds.FindAsync(id);

            if (build == null)
            {
                return NotFound();
            }

            return build;
        }

        // PUT: api/Builds/5
        [HttpPut("{id}")]
        [Authorize("ADMIN, DEVELOPER")]
        public async Task<IActionResult> PutBuild(int id, Build build)
        {
            if (id != build.Id)
            {
                return BadRequest();
            }

            _context.Entry(build).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuildExists(id))
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

        // POST: api/Builds
        [HttpPost]
        public async Task<ActionResult<Build>> PostBuild(Build build)
        {
            _context.Builds.Add(build);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBuild", new { id = build.Id }, build);
        }

        // DELETE: api/Builds/5
        [HttpDelete("{id}")]
        [Authorize("ADMIN, DEVELOPER")]
        public async Task<ActionResult<Build>> DeleteBuild(int id)
        {
            var build = await _context.Builds.FindAsync(id);
            if (build == null)
            {
                return NotFound();
            }

            _context.Builds.Remove(build);
            await _context.SaveChangesAsync();

            return build;
        }

        private bool BuildExists(int id)
        {
            return _context.Builds.Any(e => e.Id == id);
        }
    }
}
