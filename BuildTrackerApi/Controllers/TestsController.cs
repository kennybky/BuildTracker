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

namespace BuildTrackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly BuildTrackerContext _context;
        private readonly IMapper _mapper;

        public TestsController(BuildTrackerContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Tests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestDto>>> GetTests()
        {
            var tests =  await _context.Tests.Include(t=> t.TestPerson).Include(t=> t.Build).ToListAsync();
            return _mapper.Map<List<TestDto>>(tests);
        }

        // GET: api/Tests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TestDto>> GetTest(int id)
        {
            var test = await _context.Tests.Include(t => t.TestPerson).Include(t => t.Build).FirstOrDefaultAsync(t=> t.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            return _mapper.Map<TestDto>(test);
        }

        // PUT: api/Tests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTest(int id, TestDto testDto)
        {
            var test = _mapper.Map<Test>(testDto);

            if (id != test.Id)
            {
                return BadRequest();
            }

            Build build = await _context.FindAsync<Build>(test.Build?.Id);
            if (build == null)
            {
                return NotFound(new { message = "Invalid Build" });
            }
            test.Build = build;

            User person = await _context.FindAsync<User>(test.TestPerson?.Id);
            if(person == null)
            {
                return NotFound(new { message = "Invalid Build" });
            }

            test.TestPerson = person;

            _context.Entry(test).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestExists(id))
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

        // POST: api/Tests
        [HttpPost]
        public async Task<ActionResult<Test>> PostTest(TestDto testDto)
        {
            var test = _mapper.Map<Test>(testDto);

            Build build = await _context.FindAsync<Build>(test.Build?.Id);
            if(build == null)
            {
                return NotFound(new { message = "Invalid Build" });
            }
            test.Build = build;

            User person = await _context.FindAsync<User>(test.TestPerson?.Id);
            if (person == null)
            {
                return NotFound(new { message = "Invalid Build" });
            }

            test.TestPerson = person;
            _context.Tests.Add(test);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTest", new { id = test.Id }, test);
        }

        // DELETE: api/Tests/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN, DEVELOPER")]
        public async Task<ActionResult<TestDto>> DeleteTest(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                return NotFound();
            }

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();

            return _mapper.Map<TestDto>(test);
        }

        private bool TestExists(int id)
        {
            return _context.Tests.Any(e => e.Id == id);
        }
    }
}
