using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildTrackerApi.Models;
using Microsoft.AspNetCore.Authorization;
using BuildTrackerApi.Services;
using BuildTrackerApi.Models.Dtos;
using AutoMapper;

namespace BuildTrackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildsController : ControllerBase
    {
        private readonly BuildTrackerContext _context;

        private readonly IMapper _mapper;

        public BuildsController(BuildTrackerContext context,
            IMapper mapper)
        {
            _context = context;

            _mapper = mapper;
        }

        // GET: api/Builds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuildDto>>> GetBuilds()
        {
            var builds = await _context.Builds.Include(b => b.BuildPerson).Include(b => b.UpdatePerson).ToListAsync();
            var buildDto = _mapper.Map<List<BuildDto>>(builds);
            return buildDto;
        }

        // GET: api/Builds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BuildDto>> GetBuild(int id)
        {
            var build = await _context.Builds.Include(b => b.BuildPerson).Include(b => b.UpdatePerson).Include(b=> b.Tests).FirstOrDefaultAsync(b=> b.Id == id);

            if (build == null)
            {
                return NotFound();
            }

            var buildDto = _mapper.Map<BuildDto>(build);
            return buildDto;
        }

        // PUT: api/Builds/5
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN, DEVELOPER")]
        public async Task<IActionResult> PutBuild(int id, BuildDto buildDto)
        {

            Build build = _mapper.Map<Build>(buildDto);

            if (id != build.Id)
            {
                return BadRequest();
            }


            var uid = int.Parse(HttpContext.User.Identity.Name);
            User user = await _context.FindAsync<User>(uid);

            User buildUser = await _context.FindAsync<User>(build.BuildPerson?.Id);
            if (user == null || buildUser == null)
            {
                return BadRequest(new { message = "User not found" });
            }
            build.BuildPerson = buildUser;

            build.UpdatePerson = user;

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
        [Authorize(Roles = "ADMIN, DEVELOPER")]
        public async Task<ActionResult<BuildDto>> PostBuild(BuildDto buildDto)
        {
            Build build = _mapper.Map<Build>(buildDto);

            var id = int.Parse(HttpContext.User.Identity.Name);
            User user = await _context.FindAsync<User>(id);
            User buildUser = await _context.FindAsync<User>(build.BuildPerson?.Id);
            if(user == null || buildUser== null)
            {
                return BadRequest(new { message = "User not found" });
            }
            build.BuildPerson = buildUser;
            build.UpdatePerson = user;
            await _context.Builds.AddAsync(build);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<BuildDto>(build);

            return CreatedAtAction("GetBuild", new { id = build.Id }, dto);
        }

        [HttpGet("query")]
        public async Task<ActionResult<IEnumerable<BuildDto>>> Query
            ([FromQuery]string Product, [FromQuery]Platform? Platform, [FromQuery]DateTime? from, [FromQuery]DateTime? to)
        {
            var builds = _context.Builds.AsQueryable();
            if(Platform != null)
            {
                builds = builds.Where(b => b.Platform == Platform);
            }
            if(Product != null)
            {
                builds = builds.Where(b => b.ProductName == Product);
            }
            if(from != null)
            {
                builds = builds.Where(b => b.BuildDate >= from);
            }
            if(to != null)
            {
                builds = builds.Where(b => b.BuildDate <= to);
            }
            var result = await builds.Include(b=> b.BuildPerson).Include(b=> b.UpdatePerson).ToListAsync();
            return _mapper.Map<List<BuildDto>>(result);
        }

        // DELETE: api/Builds/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN, DEVELOPER")]
        public async Task<ActionResult<BuildDto>> DeleteBuild(int id)
        {
            var build = await _context.Builds.FindAsync(id);
            if (build == null)
            {
                return NotFound();
            }

            _context.Builds.Remove(build);
            await _context.SaveChangesAsync();

            return _mapper.Map<BuildDto>(build);
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<BuildDto>>> GetUserBuilds(int id){
            var builds = await _context.Builds.Include(u=> u.BuildPerson).Where(u => u.BuildPerson.Id == id).ToListAsync();
            return _mapper.Map<List<BuildDto>>(builds);
        }
        private bool BuildExists(int id)
        {
            return _context.Builds.Any(e => e.Id == id);
        }
    }
}
