using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BirFikrimVar.Models;
using Mapster;

namespace BirFikrimVar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdeasApiController : ControllerBase
    {
        private readonly MyDBcontext _context;

        public IdeasApiController(MyDBcontext context)
        {
            _context = context;
        }

        // GET: api/Ideas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IdeaDto>>> GetIdeas()
        {
            var res = await _context.Ideas.ToListAsync();
            return res.Adapt<List<IdeaDto>>();
        }

        // GET: api/Ideas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IdeaDto>> GetIdea(int id)
        {
            var idea = await _context.Ideas.FindAsync(id);

            if (idea == null)
            {
                return NotFound();
            }

            return idea.Adapt<IdeaDto>();
        }

        // PUT: api/Ideas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIdea(int id, UpdateIdeaDto dto)
        {
            var idea = await _context.Ideas.FindAsync(id);
            if (id != idea.IdeaId)
            {
                return BadRequest();
            }
            dto.Adapt(idea);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IdeaExists(id))
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

        // POST: api/Ideas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Idea>> PostIdea(CreateIdeaDto dto)
        {
            var idea = dto.Adapt<Idea>();
            idea.CreatedDate = DateTime.Now;
            idea.LikeCount = 0;

            _context.Ideas.Add(idea);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIdea", new { id = idea.IdeaId }, idea);
        }

        // DELETE: api/Ideas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIdea(int id)
        {
            var idea = await _context.Ideas.FindAsync(id);
            if (idea == null)
            {
                return NotFound();
            }

            _context.Ideas.Remove(idea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IdeaExists(int id)
        {
            return _context.Ideas.Any(e => e.IdeaId == id);
        }
    }
}
