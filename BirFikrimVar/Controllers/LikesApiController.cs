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
        [HttpPost]
        public async Task<ActionResult<LikeResponseDto>> PostLike(CreateLikeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user already liked this idea
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.IdeaId == dto.IdeaId && l.UserId == dto.UserId);

            if (existingLike != null)
            {
                return Conflict("User has already liked this idea.");
            }

            // Check if the idea exists
            var ideaExists = await _context.Ideas.AnyAsync(i => i.IdeaId == dto.IdeaId);
            if (!ideaExists)
            {
                return BadRequest("Idea does not exist.");
            }

            // Check if the user exists
            var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
            if (!userExists)
            {
                return BadRequest("User does not exist.");
            }

            var like = dto.Adapt<Like>();
            like.CreatedDate = DateTime.Now;

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            // Get the created like with user information
            var createdLike = await _context.Likes
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.LikeId == like.LikeId);

            var result = new LikeResponseDto
            {
                LikeId = createdLike.LikeId,
                IdeaId = createdLike.IdeaId,
                UserId = createdLike.UserId,
                FullName = createdLike.User.FullName,
                CreatedDate = createdLike.CreatedDate ?? DateTime.Now
            };

            return CreatedAtAction(nameof(GetLikes), new { id = like.LikeId }, result);
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

        // NEW: DELETE: api/LikesApi/unlike/5/10 (ideaId/userId)
        [HttpDelete("unlike/{ideaId}/{userId}")]
        public async Task<IActionResult> UnlikeIdea(int ideaId, int userId)
        {
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.IdeaId == ideaId && l.UserId == userId);

            if (like == null)
            {
                return NotFound("Like not found.");
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