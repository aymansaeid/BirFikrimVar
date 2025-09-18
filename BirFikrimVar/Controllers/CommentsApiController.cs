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
    public class CommentsApiController : ControllerBase
    {
        private readonly MyDBcontext _context;

        public CommentsApiController(MyDBcontext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments()
        {
            var res = await _context.Comments.ToListAsync();
            return res.Adapt<List<CommentDto>>();
        }
        // GET: api/CommentsApi/idea/{ideaId}
        [HttpGet("idea/{ideaId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByIdea(int ideaId)
        {
            var comments = await _context.Comments
                .Where(c => c.IdeaId == ideaId)
                .Include(c => c.User) 
                .Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    IdeaId = c.IdeaId,
                    UserId = c.UserId,
                    authorName = c.User.FullName, 
                    Content = c.Content,
                    CreatedDate = c.CreatedDate
                })
                .ToListAsync();

            return Ok(comments);
        }
        // GET: api/CommentsApi/count/{ideaId}
        [HttpGet("count/{ideaId}")]
        public async Task<ActionResult<int>> GetCommentCount(int ideaId)
        {
            var count = await _context.Comments
                .Where(comment => comment.IdeaId == ideaId)
                .CountAsync();

            return count;
        }
        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }
             var res = comment.Adapt<CommentDto>(); 

            return res;
        }

        // PUT: api/CommentsApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, UpdateCommentDto dto)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();

            if (comment.UserId != dto.UserId)
                return Forbid(); 

            comment.Content = dto.Content;
            comment.CreatedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(CreateCommentDto dto)
        {
            var comment = dto.Adapt<Comment>(); 
            comment.CreatedDate = DateTime.Now;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.CommentId }, comment);
        }

        // DELETE: api/CommentsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id, [FromQuery] int userId)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();

            if (comment.UserId != userId)
                return Forbid(); 

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}
