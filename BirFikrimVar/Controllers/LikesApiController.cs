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
    public class LikesApiController : ControllerBase
    {
        private readonly MyDBcontext _context;

        public LikesApiController(MyDBcontext context)
        {
            _context = context;
        }

        // GET: api/Likes1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeResponseDto>>> GetLikes()
        {
            var res = await _context.Likes
        .Include(like => like.User)
        .Select(like => new LikeResponseDto
        {
            LikeId = like.LikeId,
            IdeaId = like.IdeaId,
            UserId = like.UserId,
            FullName = like.User.FullName,
            CreatedDate = like.CreatedDate ?? DateTime.UtcNow
        })
        .ToListAsync();

            return res;
        }

        [HttpGet("count/{ideaId}")]
        public async Task<ActionResult<int>> GetLikeCount(int ideaId)
        {
            var count = await _context.Likes
                .Where(like => like.IdeaId == ideaId)
                .CountAsync();

            return count;
        }

        // POST: api/Likes1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Like>> PostLike(CreateLikeDto dto)
        {
            var like = dto.Adapt<Like>();
            like.CreatedDate = DateTime.Now;

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            var result = like.Adapt<LikeResponseDto>();
            return Ok(result);
        }

        // DELETE: api/Likes1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLike(int id)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like == null)
            {
                return NotFound();
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("check/{ideaId}/{userId}")]
        public async Task<ActionResult<bool>> CheckUserLiked(int ideaId, int userId)
        {
            var hasLiked = await _context.Likes
                .AnyAsync(like => like.IdeaId == ideaId && like.UserId == userId);

            return hasLiked;
        }
        private bool LikeExists(int id)
        {
            return _context.Likes.Any(e => e.LikeId == id);
        }
    }
}
